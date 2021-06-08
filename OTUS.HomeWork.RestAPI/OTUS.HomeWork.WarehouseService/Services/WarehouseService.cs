using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.DeliveryService.Contract.Messages;
using OTUS.HomeWork.NotificationService.Contract.Messages;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.WarehouseService.Contract.Messages;
using OTUS.HomeWork.WarehouseService.DAL;
using OTUS.HomeWork.WarehouseService.Domain;
using OTUS.HomeWork.WarehouseService.Messages;
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
        private readonly RabbitMQMessageSender _mqSender;
        private IOptions<WarehouseRabbitMQOption> _mqOptions;        
        public WarehouseService(WarehouseContext warehouseContext
            , RabbitMQMessageSender mqSender
            , IOptions<WarehouseRabbitMQOption> mqOptions)
        {
            _warehouseContext = warehouseContext;
            _mqSender = mqSender;
            _mqOptions = mqOptions;
        }

        // TODO выделить минифункци reserve, код станет чище
        public async Task<ReserveProduct[]> ReserveProductsAsync(ReserveProduct[] products, string orderNumber)
        {
            var reserveProductsList = new List<ReserveProduct>();
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                DateTime reserveDate = DateTime.UtcNow;
                foreach(var product in products)
                {
                    var remainProductCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == product.ProductId);
                    if (remainProductCounter == null)
                    {
                        reserveProductsList.Add(new ReserveProduct
                        {
                            Count = -1,
                            OrderNumber = orderNumber,
                            ProductId = product.ProductId,
                            ReserveDate = reserveDate,
                        });
                        continue;
                    }
                    var existReserve = await _warehouseContext.Reserves.FirstOrDefaultAsync(g => g.OrderNumber == orderNumber && g.ProductId == product.ProductId);
                    if (remainProductCounter.RemainCount < product.Count)
                    {
                        // нет достаточного количества товаров
                        reserveProductsList.Add(new ReserveProduct
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
                        Count = product.Count,
                        OrderNumber = orderNumber,
                        ProductId = product.ProductId,
                        ReserveDate = reserveDate,
                    };
                    reserveProductsList.Add(reserveProduct);
                    if (existReserve != null)
                    {
                        existReserve.Count = product.Count;
                        existReserve.ReserveDate = reserveDate;
                    }
                    else
                    {
                        _warehouseContext.Reserves.Add(reserveProduct);
                    }
                }

                // посылаем информацию о резервировании товаров
                await _mqSender.SendMessageAsync(new UpdateProductCounterByReserveRequest(orderNumber)
                {
                    Products = reserveProductsList.Where(g => g.Count > 0).Select(g => new UpdateProductCounterByReserveRequest.ReserveProduct
                    {
                        Count = g.Count,
                        ProductId = g.ProductId
                    }).ToList()
                }, _mqOptions.Value.WarehouseRouteKey);

                await _warehouseContext.SaveChangesAsync();
                scope.Complete();
            }
            return reserveProductsList.ToArray();
        }
        
        public async Task<bool> ResetReserveProductsAsync(string orderNumber)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                var reservers = await _warehouseContext.Reserves.Where(g => g.OrderNumber == orderNumber).ToArrayAsync();
                foreach(var res in reservers)
                {
                    _warehouseContext.Reserves.Remove(res);
                }
                await _mqSender.SendMessageAsync(new UpdateProductCounterByResetReserveRequest(orderNumber)
                {
                    Products = reservers.Where(g => g.Count > 0).Select(g => new UpdateProductCounterByResetReserveRequest.ReserveProduct
                    {
                        Count = g.Count,
                        ProductId = g.ProductId
                    }).ToList()
                }, _mqOptions.Value.WarehouseRouteKey);

                await _warehouseContext.SaveChangesAsync();
                scope.Complete();
            }
            return true;
        }

        public async Task<bool> ShipmentProductsAsync(string orderNumber, string deliveryAddress, Guid userId)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    // 1. снимаем с резервирования
                    var reservers = await _warehouseContext.Reserves.Include(g => g.Product).Where(g => g.OrderNumber == orderNumber).ToArrayAsync();
                    if (!reservers.Any())
                        return false;

                    foreach (var res in reservers)
                    {
                        _warehouseContext.Reserves.Remove(res);
                    }

                    // 2. формируем заявку на отгрузку
                    var allProductIds = reservers.Select(g => g.ProductId).ToArray();
                    var readyToShipment = DateTime.UtcNow.AddHours(4);
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

                    // 3. проверяем, что требуемое количество товаров существует
                    foreach (var reserver in reservers)
                    {
                        var remainProductCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == reserver.ProductId);
                        if (remainProductCounter == null)
                            // нет такого товара
                            return false;
                        if (remainProductCounter.RemainCount < reserver.Count)
                            // нет нужного количества на складе
                            return false;
                    }
                    // 4. посылаем сообщение об измении количества товаров
                    await _mqSender.SendMessageAsync(new UpdateProductCounterByShipmentRequest(orderNumber)
                    {
                        Products = reservers.Where(g => g.Count > 0).Select(g => new UpdateProductCounterByShipmentRequest.ReserveProduct
                        {
                            Count = g.Count,
                            ProductId = g.ProductId
                        }).ToList()
                    }, _mqOptions.Value.WarehouseRouteKey);

                    // 5. сообщаем сервису доставки о необходимости забирания товара(в будущем это будет делаться через шину данных)
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

        public async Task<bool> UpdateReserveCounterAsync(IEnumerable<ReserveProduct> reserves)
        {
            foreach (var res in reserves)
            {
                var existCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == res.ProductId);
                if (existCounter != null)
                {
                    existCounter.ReserveCount += res.Count;
                    if (existCounter.RemainCount < existCounter.ReserveCount)
                        return false;
                }
                _warehouseContext.Counters.Update(existCounter);
            }
            await _warehouseContext.SaveChangesAsync();
            return false;
        }

        public async Task UpdateUnReserveCounterAsync(IEnumerable<ReserveProduct> reserves)
        {
            foreach (var res in reserves)
            {
                var existCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == res.ProductId);
                if (existCounter != null)
                {
                    existCounter.ReserveCount -= res.Count;
                }
                _warehouseContext.Counters.Update(existCounter);
            }
            await _warehouseContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateCounterByShipmentAsync(IEnumerable<ReserveProduct> reserves)
        {
            foreach (var res in reserves)
            {
                var existCounter = await _warehouseContext.Counters.FirstOrDefaultAsync(g => g.ProductId == res.ProductId);
                if (existCounter != null)
                {
                    existCounter.ReserveCount -= res.Count;
                    existCounter.RemainCount -= res.Count;
                    if (existCounter.RemainCount < 0)
                        return false;
                }
                _warehouseContext.Counters.Update(existCounter);
            }
            await _warehouseContext.SaveChangesAsync();
            return true;
        }
    }
}
