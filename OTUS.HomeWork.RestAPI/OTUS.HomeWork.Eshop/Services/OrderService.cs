using OTUS.HomeWork.Clients;
using OTUS.HomeWork.EShop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTUS.HomeWork.EShop.Domain;
using OTUS.HomeWork.EShop.Domain.DTO;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.NotificationService.Contract.Messages;
using OTUS.HomeWork.EShop.Monitoring;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.Clients.Warehouse;
using OTUS.HomeWork.RestAPI.Abstraction.DAL;
using Microsoft.Extensions.Caching.Distributed;

namespace OTUS.HomeWork.EShop.Services
{
    public class OrderService
    {
        private readonly OrderContext _orderContext;
        private readonly PriceServiceClient _pricingClient;
        private readonly WarehouseServiceClient _warehouseServiceClient;
        private readonly BucketRepository _bucketRepository;
        private readonly RabbitMQMessageSender _mqSender;
        private readonly MetricReporter _metricReporter;
        private readonly UserContext _userContext;
        private readonly IDistributedCache _distributedCache;

        public OrderService(OrderContext orderContext
            , UserContext userContext
            , BucketRepository bucketRepository
            , PriceServiceClient pricingClient
            , WarehouseServiceClient warehouseServiceClient
            , RabbitMQMessageSender mqSender
            , MetricReporter metricReporter
            , IDistributedCache distributedCache)
        {
            _orderContext = orderContext;
            _userContext = userContext;
            _pricingClient = pricingClient;
            _warehouseServiceClient = warehouseServiceClient;
            _bucketRepository = bucketRepository;
            _mqSender = mqSender;
            _metricReporter = metricReporter;
            _distributedCache = distributedCache;
        }

        public async Task<Order> CreateOrderAsync(Guid userId, CreateOrderDTO orderRequest)
        {
            // проверяем, не пришел ли второй раз тот же самый запрос
            var existOrder = _orderContext.Orders.FirstOrDefault(g => g.IdempotencyKey == orderRequest.IdempotencyKey);
            if (existOrder != null)
                return existOrder;

            Order order = null;
            bool wasReserveProducts = false;
            try
            {
                var bucket = await _bucketRepository.GetBucketForUserAsync(userId);
                if (bucket == null || !bucket.Items.Any())
                    return new Order
                    {
                        Status = OrderStatus.Error
                    };
                
                // 1. расчитываем стоимость                
                (decimal totalPrice, decimal discount) price = await CalculateTotalPriceAsync(bucket, userId);

                // 2. сохраняем заказ в БД
                var orderItems = bucket.Items.Select(g => new OrderItem
                {
                    ProductId = g.ProductId,
                    Quantity = g.Quantity
                }).ToList();

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    order = new Order
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DeliveryAddress = orderRequest.DeliveryAddress,
                        Items = orderItems,
                        Status = OrderStatus.Pending,
                        UserId = userId,
                        TotalPrice = price.totalPrice
                    };
                    foreach (var item in orderItems)
                    {
                        item.Order = order;
                    }

                    _orderContext.Add(order);
                    await _orderContext.SaveChangesAsync();

                    await _bucketRepository.ClearBucketAsync(userId);
                    scope.Complete();
                }
                // 3. резервируем на складе
                _warehouseServiceClient.AddHeader(Constants.USERID_HEADER, userId.ToString());
                var reserve = await _warehouseServiceClient.ReserveAsync(new ReserveProductRequestDTO
                {
                    OrderNumber = order.OrderNumber.ToString(),
                    Products = order.Items.Select(g => new ReserveProductDTO
                    {
                        Id = g.ProductId,
                        Count = g.Quantity
                    }).ToList()
                });
                wasReserveProducts = true;
                if (reserve.IsSuccess == false)
                {
                    throw new Exception("Не удалось полностью зарезервировать товар");
                }
            }
            catch (Exception ex)
            {
                if (wasReserveProducts)
                {
                    try
                    {
                        await _warehouseServiceClient.CancelAsync(order.OrderNumber.ToString());
                    }
                    catch (Exception inEx)
                    {
                        // логируем, что товар не сняли с резервирования
                    }
                }

                try
                {
                    if (order != null)
                    {
                        existOrder = _orderContext.Orders.FirstOrDefault(g => g.OrderNumber == order.OrderNumber);
                        existOrder.Status = OrderStatus.Error;
                        existOrder.ErrorDescription = ex.Message;
                        await _orderContext.SaveChangesAsync();
                    }
                }
                catch (Exception inEx)
                {
                    // логируем, что не удалось изменить статус
                }
                _metricReporter.RegisterFailedOrder();
            }
            return order;
        }

        public async Task<(decimal summaryPrice, decimal discount)> CalculateTotalPriceAsync(Bucket bucket, Guid userId, bool force = false)
        {
            string priceCacheKey = "bucketPrice" + bucket.Id;
            string discountCacheKey = "bucketDiscount" + bucket.Id;
            var precalculatePrice = await _distributedCache.GetAsync(priceCacheKey);
            var precalculateDiscount = await _distributedCache.GetAsync(discountCacheKey);
            if (!force && precalculatePrice != null && precalculateDiscount != null)
            {
                return (BitconverterExt.ToDecimal(precalculatePrice), BitconverterExt.ToDecimal(precalculateDiscount));
            }
            else
            {
                var response = await _pricingClient.PriceAsync(userId, new PriceRequestDTO
                {
                    Products = bucket.Items.Select(g => new PProductDTO
                    {
                        ProductId = g.ProductId.ToString(),
                        Quantity = g.Quantity
                    }).ToArray()
                });

                _distributedCache.Set(priceCacheKey, BitconverterExt.GetBytes(response.SummaryPrice) , new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 5, 0) });
                _distributedCache.Set(discountCacheKey, BitconverterExt.GetBytes(response.Discount), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 5, 0) });

                return (response.SummaryPrice, response.Discount);
            }
        }

        public async Task<Order> OrderWasPaid(PaymentResultDTO paymentResultDTO, Guid orderId)
        {
            var order = await _orderContext.Orders.FirstOrDefaultAsync(g => g.OrderNumber == orderId);
            if (order == null)
            {
                _metricReporter.RegisterFailedOrder();
                throw new Exception($"Заказ c id={orderId} не существует");
            }
            if (await PaymentWasCompleted(paymentResultDTO, order))
            {
                order = await SendRequestOnShipment(order);
                _metricReporter.RegisterCreateOrder();
            }
            else
            {
                _metricReporter.RegisterFailedOrder();
            }
            return order;
        }

        private async Task<bool> PaymentWasCompleted(PaymentResultDTO paymentResultDTO, Order order)
        {
            if (order.BillingId == paymentResultDTO.BillingId.ToString())
                return false;

            if (order.BillingId != null)
                throw new Exception("Заказ с id={orderId} уже оплачен");

            bool result = true;
            var user = _userContext.Users.First(g => g.Id == paymentResultDTO.UserId);

            if (paymentResultDTO.IsSuccessfully)
            {
                // 1. обновляем заказ 
                order.BillingId = paymentResultDTO.BillingId.ToString();
                order.PaidDateUtc = paymentResultDTO.PaymentDateUtc;
                order.Status = OrderStatus.Processing;

                // 2. отправляем уведомление об успешной оплате
                await _mqSender.SendMessageAsync(new OrderWasPayment
                {
                    UserEmail = user.UserName,
                    BillingId = order.BillingId.ToString(),
                    OrderNumber = order.OrderNumber.ToString(),
                    Price = order.TotalPrice,
                });
            }
            else
            {
                order.Status = OrderStatus.Error;
                // 1. снимаем с резервирования
                await _warehouseServiceClient.CancelAsync(order.OrderNumber.ToString());
                // 2. отправляем уведомление о плохой оплате
                await _mqSender.SendMessageAsync(new OrderCreatedError
                {
                    UserEmail = user.UserName,
                    Message = $"Не удалось оплатить товар по причине {paymentResultDTO.ErrorDescription ?? string.Empty}. Заказ товара отменен. Попробуйте выполнить заказ повторно!",
                });
                result = false;
            }

            _orderContext.Update(order);
            await _orderContext.SaveChangesAsync();
            return result;
        }

        private async Task<Order> SendRequestOnShipment(Order order)
        {
            if (order.Status == OrderStatus.Processing)
            {
                try
                {
                    // отправляем заявку на отгрузку товара
                    _warehouseServiceClient.AddHeader(Constants.USERID_HEADER, order.UserId.ToString());
                    await _warehouseServiceClient.ShipmentAsync(new ShipmentRequestDTO
                    {
                        DeliveryAddress = order.DeliveryAddress,
                        OrderNumber = order.OrderNumber.ToString(),
                        UserId = order.UserId
                    });
                    order.Status = OrderStatus.Complete;

                    _orderContext.Orders.Update(order);
                    await _orderContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // будет некая джоба, которая допинает и отправит заказ на отгрузку, если сейчас не получилась, поэтому мы ничего делаем
                }
            }
            else
            {
                // просто логируем, что заказ еще не оплачен, ничего не делаем
            }
            return order;
        }

        public Task<Order[]> GetOrders(Guid userId, int skip, int limit)
        {
            return _orderContext.Orders.Where(g => g.UserId == userId).Skip(skip).Take(limit).ToArrayAsync();
        }
    }
}
