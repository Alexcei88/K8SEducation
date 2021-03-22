using OTUS.HomeWork.Clients;
using OTUS.HomeWork.Eshop.DAL;
using OTUS.HomeWork.Eshop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.EShop.Services
{
    public class OrderService
    {
        private readonly OrderContext _orderContext;
        private readonly BillingServiceClient _billingClient;

        public OrderService(OrderContext orderContext, BillingServiceClient billingClient)
        {
            _orderContext = orderContext;
            _billingClient = billingClient;
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            try
            {
                var newBalance = _billingClient.Balance2Async(order.UserId, order.TotalPrice);

            }
            catch(Exception ex)
            {
                // нужно исключение прокидывает специальное, упрощаем
                throw new Exception()
            }
            order.BillingId = null;
            order.CreatedOnUtc = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            //order.TotalPrice = order.Items.Sum(k => k.)
            await _orderContext.AddAsync(order);
            await _orderContext.SaveChangesAsync();

            return order.OrderNumber;
        }
    }
}
