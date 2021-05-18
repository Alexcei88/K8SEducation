using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.DeliveryService.Contract.Messages;
using OTUS.HomeWork.NotificationService.Contract.Messages;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.WarehouseService.Contract.Messages;
using OTUS.HomeWork.WarehouseService.DAL;
using OTUS.HomeWork.WarehouseService.Domain;
using OTUS.HomeWork.WarehouseService.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace OTUS.HomeWork.WarehouseService.Services
{
    public class WarehouseService
    {
        private readonly WarehouseContext _warehouseContext;
        private readonly DeliveryServiceClient _deliveryServiceClient;
        private readonly RabbitMQMessageSender _mqSender;
        private IOptions<WarehouseRabbitMQOption> _mqOptions;

        public WarehouseService(WarehouseContext warehouseContext, DeliveryServiceClient deliveryServiceClient, RabbitMQMessageSender mqSender, IOptions<WarehouseRabbitMQOption> mqOptions)
        {
            _warehouseContext = warehouseContext;
            _deliveryServiceClient = deliveryServiceClient;
            _mqSender = mqSender;
            _mqOptions = mqOptions;
        }

        // TODO выделить минифункци reserve, код станет чище
        public async Task<ReserveProduct[]> ReserveProductsAsync(ReserveProduct[] products, string orderNumber)
        {
            var reserveProducts = new List<ReserveProduct>();
            // TODO выставляю Serializable, чтобы однопоточно менять значения счетчиков, но на самом деле схема по изменению счетчика и количества должна быть на очередях,
            // а здесь мы просто должны регать завяку и просто проверять, что нужное количество на данный момент имеется
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                DateTime reserveDate = DateTime.UtcNow;
                foreach(var product in products)
                {
                    var remainProductCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == product.ProductId);
                    if (remainProductCounter == null)
                    {
                        reserveProducts.Add(new ReserveProduct
                        {
                            Count = -1,
                            OrderNumber = orderNumber,
                            ProductId = product.ProductId,
                            ReserveDate = reserveDate,
                        });
                        continue;
                    }
                    var existReserve = await _warehouseContext.Reserves.FirstOrDefaultAsync(g => g.OrderNumber == orderNumber && g.ProductId == product.ProductId);
                    long queryCount = product.Count;
                    long existReserveCount = existReserve == null ? 0 : existReserve.Count;
                    long newReserverCount = remainProductCounter.ReserveCount - existReserveCount + queryCount;
                    if (remainProductCounter.RemainCount < newReserverCount)
                    {
                        reserveProducts.Add(new ReserveProduct
                        {
                            Count = -1,
                            OrderNumber = orderNumber,
                            ProductId = product.ProductId,
                            ReserveDate = reserveDate,
                        });
                        continue;
                    }
                    var reserveProduct = new ReserveProduct
                    {
                        Count = queryCount,
                        OrderNumber = orderNumber,
                        ProductId = product.ProductId,
                        ReserveDate = reserveDate,
                    };
                    reserveProducts.Add(reserveProduct);

                    remainProductCounter.ReserveCount = newReserverCount;                    
                    if (existReserve != null)
                    {
                        existReserve.Count = queryCount;
                        existReserve.ReserveDate = reserveDate;
                    }
                    else
                    {
                        _warehouseContext.Reserves.Add(reserveProduct);
                    }
                    _warehouseContext.Update(remainProductCounter);
                }

                await _warehouseContext.SaveChangesAsync();
                scope.Complete();
            }
            return reserveProducts.ToArray();
        }
        
        public async Task<bool> ResetReserveProductsAsync(string orderNumber)
        {
            // TODO выставляю Serializable, чтобы однопоточно менять значения счетчиков, но на самом деле схема по изменению счетчика и количества должна быть на очередях,
            // а здесь мы просто должны регать завяку и просто проверять, что нужное количество на данный момент имеется
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable
            }, TransactionScopeAsyncFlowOption.Enabled))
            {
                var reservers = await _warehouseContext.Reserves.Where(g => g.OrderNumber == orderNumber).ToArrayAsync();
                foreach (var reserve in reservers)
                {
                    await UnReserveProduct(reserve);
                }
                await _warehouseContext.SaveChangesAsync();
                scope.Complete();
            }
            return true;
        }

        public async Task<bool> ShipmentProductsAsync(string orderNumber, string deliveryAddress, Guid userId)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable
                }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    // 1. снимаем с резервирования
                    var reservers = await _warehouseContext.Reserves.Include(g => g.Product).Where(g => g.OrderNumber == orderNumber).ToArrayAsync();
                    if (!reservers.Any())
                        return false;

                    var allProductIds = reservers.Select(g => g.ProductId).ToArray();
                    var products = _warehouseContext.Products.Where(g => allProductIds.Contains(g.Id));
                    foreach (var reserve in reservers)
                    {
                        await UnReserveProduct(reserve);
                    }
                    var readyToShipment = DateTime.UtcNow.AddHours(4);
                    // 2. формируем заявку на отгрузку
                    _warehouseContext.Shipments.Add(new ShipmentOrder
                    {
                        DeliveryAddress = deliveryAddress,
                        OrderNumber = orderNumber,
                        ProductIds = reservers.Select(g => g.ProductId).ToList(),
                        Status = ShipmentOrderStatus.Created,
                        WasCancelled = false,
                        ReadyToShipmentDate = readyToShipment,
                        UserId = userId,
                    });

                    // 3. снимаем с остатков
                    foreach (var reserver in reservers)
                    {
                        var remainProductCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == reserver.ProductId);
                        if (remainProductCounter == null)
                            // нет такого товара
                            return false;
                        remainProductCounter.RemainCount -= reserver.Count;
                        if (remainProductCounter.RemainCount < 0)
                            // нет нужного количества на складе
                            return false;
                    }

                    // 4. сообщаем сервису доставки о необходимости забирания товара(в будуем это будет делаться через шину данных)
                    await _mqSender.SendMessageAsync(new DeliveryOrderRequest
                    {
                        DeliveryAddress = deliveryAddress,
                        OrderNumber = orderNumber,
                        Products  = reservers.Select(g => new DeliveryOrderRequest.DeliveryProduct
                        {
                            ProductId = g.ProductId,
                            Space = g.Product.Weight,
                            Weight = g.Product.Weight,                            
                        }).ToList(),
                        ReadyToShipmentDate = readyToShipment
                    }, _mqOptions.Value.DeliveryRouteKey);
                    
                    await _warehouseContext.SaveChangesAsync();
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task ConfirmDeliveryOrderAsync(DeliveryOrderResponse response)
        {
            var shipment = await _warehouseContext.Shipments.FirstOrDefaultAsync(g => g.OrderNumber == response.OrderNumber);
            if (shipment == null)
                throw new Exception($"Order with number={response.OrderNumber} is not exist");

            if(response.IsCanDelivery)
            {
                shipment.Status = ShipmentOrderStatus.DeliveryConfirmed;
                shipment.ShipmentDate = response.ShipmentDate;

                await _mqSender.SendMessageAsync(new OrderReadyToDelivery
                {
                    UserId = shipment.UserId,
                    OrderNumber = shipment.OrderNumber
                });
            }
            else
            {
                shipment.WasCancelled = false;
                shipment.Status = ShipmentOrderStatus.ErrorShipment;
                shipment.ErrorDescription = response.ErrorDescription;

                // здесь нужна логика по возврату заказа пользователю
            }
            _warehouseContext.Shipments.Update(shipment);
            await _warehouseContext.SaveChangesAsync();
        }

        private async Task<ReserveProduct> ReserveProduct(Guid productId)
        {
            throw new NotImplementedException("Потом инкапсулирую");
            return null;
        }

        private async Task UnReserveProduct(ReserveProduct reserve)
        {
            var existCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == reserve.ProductId);
            if (existCounter != null)
            {
                existCounter.ReserveCount -= reserve.Count;
            }
            _warehouseContext.Reserves.Remove(reserve);
        }
    }
}
