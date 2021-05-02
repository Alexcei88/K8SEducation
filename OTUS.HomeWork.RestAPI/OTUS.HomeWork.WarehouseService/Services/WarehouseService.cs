using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.WarehouseService.DAL;
using OTUS.HomeWork.WarehouseService.Domain;
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

        public WarehouseService(WarehouseContext warehouseContext)
        {
            _warehouseContext = warehouseContext;
        }

        // TODO выделить минифункци reserve, код станет чище
        public async Task<ReserveProduct[]> ReserveProducts(ReserveProduct[] products, string orderNumber)
        {
            var reserveProducts = new List<ReserveProduct>();
            // TODO выставляю Serializable, чтобы однопоточно менять значения счетчиков, но на самом деле схема по изменению счетчика и количества должна быть на очередях,
            // а здесь мы просто должны регать завяку и просто проверять, что нужное количество на данный момент имеется
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable
            }))
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
                        long more = newReserverCount - remainProductCounter.RemainCount;
                        newReserverCount -= more;
                        queryCount -= more;
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
                }

                scope.Complete();
            }
            return reserveProducts.ToArray();
        }
        
        public async Task<bool> ResetReserveProducts(string orderNumber)
        {
            // TODO выставляю Serializable, чтобы однопоточно менять значения счетчиков, но на самом деле схема по изменению счетчика и количества должна быть на очередях,
            // а здесь мы просто должны регать завяку и просто проверять, что нужное количество на данный момент имеется
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable
            }))
            {
                var reservers = await _warehouseContext.Reserves.Where(g => g.OrderNumber == orderNumber).ToArrayAsync();
                foreach (var reserve in reservers)
                {
                    await UnReserveProduct(reserve);
                }
                scope.Complete();
            }
            return true;
        }

        public async Task<bool> ShipmentProducts(string orderNumber, string deliveryAddress)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable
            }))
            {
                // 1. снимаем с резервирования
                var reservers = await _warehouseContext.Reserves.Where(g => g.OrderNumber == orderNumber).ToArrayAsync();
                if(reservers.Any())
                    return false;

                foreach (var reserve in reservers)
                {
                    await UnReserveProduct(reserve);
                }
                // 2. формируем заявку на отгрузку
                _warehouseContext.Shipments.Add(new ShipmentOrder
                {
                    DeliveryAddress = deliveryAddress,
                    OrderNumber = orderNumber,
                    ProductIds = reservers.Select(g => g.ProductId).ToList(),
                    Status = ShipmentOrderStatus.Created,
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
                scope.Complete();

                //4. нужна доставка сервисом доставки

            }
            return true;
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
