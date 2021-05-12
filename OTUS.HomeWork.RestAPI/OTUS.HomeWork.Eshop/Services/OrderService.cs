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

namespace OTUS.HomeWork.EShop.Services
{
    public class OrderService
    {
        private readonly OrderContext _orderContext;
        private readonly PaymentGatewayClient _paymentGatewayClient;
        private readonly PriceServiceClient _pricingClient;
        private readonly WarehouseServiceClient _warehouseServiceClient;
        private readonly BucketRepository _bucketRepository;
        private readonly RabbitMQMessageSender _mqSender;

        public OrderService(OrderContext orderContext
            , BucketRepository bucketRepository
            , PaymentGatewayClient paymentGatewayClient
            , PriceServiceClient pricingClient
            , WarehouseServiceClient warehouseServiceClient
            , RabbitMQMessageSender mqSender)
        {
            _orderContext = orderContext;
            _paymentGatewayClient = paymentGatewayClient;
            _pricingClient = pricingClient;
            _warehouseServiceClient = warehouseServiceClient;
            _bucketRepository = bucketRepository;
            _mqSender = mqSender;
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
                var products = await _bucketRepository.GetBucketForUserAsync(userId);
                if (!products.Any())
                    return new Order
                    {
                        Status = OrderStatus.Error
                    };

                var orderItems = products.Select(g => new OrderItem
                {
                    ProductId = g.ProductId,
                    Quantity = g.Quantity
                }).ToList();

                // 1. расчитываем стоимость
                var totalPrice = await CalculateTotalPriceAsync(orderItems, userId);

                // 2. сохраняем заказ в БД
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    order = new Order
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DeliveryAddress = orderRequest.DeliveryAddress,
                        Items = orderItems,
                        Status = OrderStatus.Pending,
                        UserId = userId,
                        TotalPrice = totalPrice
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
                        await _orderContext.SaveChangesAsync();
                    }
                }
                catch (Exception inEx)
                {
                    // логируем, что не удалось изменить статус
                }
            }
            return order;
        }

        private async Task<decimal> CalculateTotalPriceAsync(List<OrderItem> items, Guid userId)
        {
            var response = await _pricingClient.PriceAsync(userId, new PriceRequestDTO
            {
            });
            return response.SummaryPrice;
        }

        public async Task<Order> OrderWasPaid(PaymentResultDTO paymentResultDTO, Guid orderId)
        {
            await PaymentWasCompleted(paymentResultDTO, orderId);
            return await SendRequestOnShipment(orderId);
        }

        private async Task PaymentWasCompleted(PaymentResultDTO paymentResultDTO, Guid orderId)
        {
            var order = await _orderContext.Orders.FirstOrDefaultAsync(g => g.OrderNumber == orderId);
            if (order == null)
                throw new Exception($"Заказ c id={orderId} не существует");

            if (paymentResultDTO.IsSuccessfully)
            {
                // 1. обновляем заказ 
                order.BillingId = paymentResultDTO.BillingId.ToString();
                order.PaidDateUtc = paymentResultDTO.PaymentDateUtc;
                order.Status = OrderStatus.Processing;

                // 2. отправляем уведомление об успешной оплате
                await _mqSender.SendMessageAsync(new OrderWasPayment
                {
                    UserId = paymentResultDTO.UserId,
                    BillingId = order.BillingId.ToString(),
                    OrderNumber = order.OrderNumber.ToString(),
                    Price = order.TotalPrice,
                });
            }
            else
            {
                order.Status = OrderStatus.Error;
                // 1. снимаем с резервирования
                await _warehouseServiceClient.CancelAsync(orderId.ToString());
                // 2. отправляем уведомление о плохой оплате
                await _mqSender.SendMessageAsync(new OrderCreatedError
                {
                    UserId = paymentResultDTO.UserId,
                    Message = $"Не удалось оплатить товар по причине {paymentResultDTO.ErrorDescription ?? string.Empty}. Заказ товара отменен. Попробуйте выполнить заказ повторно!",
                });
            }

            _orderContext.Update(order);
            await _orderContext.SaveChangesAsync();
        }

        private async Task<Order> SendRequestOnShipment(Guid orderId)
        {
            var order = await _orderContext.Orders.FirstOrDefaultAsync(g => g.OrderNumber == orderId);
            if (order == null)
                throw new Exception($"Заказ c id={orderId} не существует");
            if (order.Status == OrderStatus.Processing)
            {
                try
                {
                    // отправляем заявку на отгрузку товара
                    await _warehouseServiceClient.ShipmentAsync(new ShipmentRequestDTO
                    {
                        DeliveryAddress = order.DeliveryAddress,
                        OrderNumber = order.OrderNumber.ToString()
                    });
                    order.Status = OrderStatus.Complete;

                    _orderContext.Orders.Update(order);
                    await _orderContext.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    order.Status = OrderStatus.Error;
                    // что-то пошло не так
                    // возвращаем деньги пользователю
                    await _paymentGatewayClient.RefundAsync(order.UserId, new RefundRequestDTO
                    {
                        BillingId = Guid.Parse(order.BillingId)
                    });

                    // отправляем уведомление пользователю об отмене денег
                    await _mqSender.SendMessageAsync(new OrderRefundPayment
                    {
                         BillingId = order.BillingId,
                         OrderNumber = order.OrderNumber.ToString(),
                         Price = order.TotalPrice,
                         UserId = order.UserId
                    });

                    // отправляем уведомление пользователю об отмене заказа
                    await _mqSender.SendMessageAsync(new OrderCreatedError
                    {
                        Message = "Заказ отменен. Попробуйте повторить заказ позже",
                        UserId = order.UserId
                    });

                    _orderContext.Orders.Update(order);
                    await _orderContext.SaveChangesAsync();
                }
            }
            else
            {
                // просто логируем, что заказ еще не оплачен, ничего не делаем
            }
            return order;
        }
    }
}
