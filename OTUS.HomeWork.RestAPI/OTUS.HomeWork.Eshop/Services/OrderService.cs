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

                // 3. делаем оплату
                PaymentDTO payment = await _paymentGatewayClient.PaymentAsync(order.UserId, new PaymentRequestDTO
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
                await _orderContext.SaveChangesAsync();

                // 4. отправляем заявку на отгрузку товара
                await _warehouseServiceClient.ShipmentAsync(new ShipmentRequestDTO
                {
                    DeliveryAddress = order.DeliveryAddress,
                    OrderNumber = order.OrderNumber.ToString()
                });
                // 5. отправляем уведомление пользователю

            }
            catch(Exception ex)
            {
                if (wasPayment)
                {
                    try
                    {
                    }
                    catch (Exception ex)
                    {
                        // логируем, что 
                    }
                }
                if(wasReserveProducts)
                {
                    try
                    {
                    }
                    catch (Exception ex)
                    {
                        // логируем, что 
                    }
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
