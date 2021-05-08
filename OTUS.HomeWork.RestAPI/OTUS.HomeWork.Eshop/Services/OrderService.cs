using OTUS.HomeWork.Clients;
using OTUS.HomeWork.EShop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTUS.HomeWork.EShop.Domain;

namespace OTUS.HomeWork.EShop.Services
{
    public class OrderService
    {
        private readonly OrderContext _orderContext;
        private readonly PaymentGatewayClient _paymentGatewayClient;
        private readonly PriceServiceClient _pricingClient;
        private readonly WarehouseServiceClient _warehouseServiceClient;

        public OrderService(OrderContext orderContext
            , PaymentGatewayClient paymentGatewayClient
            , PriceServiceClient pricingClient
            , WarehouseServiceClient warehouseServiceClient)
        {
            _orderContext = orderContext;
            _paymentGatewayClient = paymentGatewayClient;
            _pricingClient = pricingClient;
            _warehouseServiceClient = warehouseServiceClient;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            // проверяем, не пришел ли второй раз тот же самый запрос
            var existOrder = _orderContext.Orders.FirstOrDefault(g => g.IdempotencyKey == order.IdempotencyKey);
            if (existOrder != null)
                return existOrder;

            // TODO здесь реализация паттерна Сага, лучше реализовать на какойто либе stateMachine типа такой https://github.com/dotnet-state-machine/stateless

            bool wasReserveProducts = false;
            bool wasPayment = false;
            PaymentDTO payment = null;
            try
            {

                // 1. расчитываем стоимость
                var totalPrice = await CalculateTotalPriceAsync(order.Items, order.UserId.ToString());

                // 2. сохраняем заказ в БД
                order.TotalPrice = totalPrice;
                order.CreatedOnUtc = DateTime.UtcNow;
                order.Status = OrderStatus.Pending;

                foreach (var item in order.Items)
                {
                    item.Order = order;
                }

                _orderContext.Add(order);
                await _orderContext.SaveChangesAsync();

                // 2. резервируем товар на складе
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

                // 3. делаем оплату
                payment = await _paymentGatewayClient.PaymentAsync(order.UserId, new PaymentRequestDTO
                {
                    Amount = totalPrice,
                    IdempotenceKey = "MakePayment" + order.IdempotencyKey
                });
                if(payment.IsSuccess == false)
                {
                    throw new Exception("Не удалось произвести оплату");
                }

                wasPayment = true;
                existOrder = _orderContext.Orders.FirstOrDefault(g => g.OrderNumber == order.OrderNumber);
                existOrder.BillingId = payment.BillingId.ToString();
                existOrder.PaidDateUtc = DateTime.UtcNow;
                existOrder.Status = OrderStatus.Processing;
                await _orderContext.SaveChangesAsync();
                order = existOrder;

                // 4. отправляем заявку на отгрузку товара
                await _warehouseServiceClient.ShipmentAsync(new ShipmentRequestDTO
                {
                    DeliveryAddress = order.DeliveryAddress,
                    OrderNumber = order.OrderNumber.ToString()
                });

                // 5. отправляем уведомление пользователю
                existOrder = _orderContext.Orders.FirstOrDefault(g => g.OrderNumber == order.OrderNumber);
                existOrder.Status = OrderStatus.Complete;
                await _orderContext.SaveChangesAsync();
                order = existOrder;
            }
            catch (Exception ex)
            {
                if (wasPayment)
                {
                    try
                    {
                        var response = await _paymentGatewayClient.RefundAsync(order.UserId, new RefundRequestDTO
                        {
                            BillingId = payment.BillingId,
                        });
                        if(!response.IsSuccess)
                        {
                            // логируем, что оплату не смогли отменить
                        }
                    }
                    catch (Exception inEx)
                    {
                        // логируем, что 
                    }
                }
                if(wasReserveProducts)
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
                    existOrder = _orderContext.Orders.FirstOrDefault(g => g.OrderNumber == order.OrderNumber);
                    existOrder.Status = OrderStatus.Error;
                    await _orderContext.SaveChangesAsync();
                }
                catch(Exception inEx)
                {
                    // логируем, что не удалось изменить статус
                }
            }
            /*
            // send notification
            await _mqSender.SendMessageAsync(new OrderCreated
            {
                UserId = userId,
                BillingAddressId = newOrder.BillingId.ToString(),
                OrderNumber = newOrder.OrderNumber.ToString(),
                Price = newOrder.TotalPrice,
            });*/

            return order;
        }

        private async Task<decimal> CalculateTotalPriceAsync(List<OrderItem> items, string userId)
        {
            var response = await _pricingClient.PriceAsync(new PriceRequestDTO
            {
                UserId = userId,
            });
            return response.SummaryPrice;
        }
    }
}
