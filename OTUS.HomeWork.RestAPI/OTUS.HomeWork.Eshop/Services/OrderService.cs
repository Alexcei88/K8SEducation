using OTUS.HomeWork.Clients;
using OTUS.HomeWork.Eshop.DAL;
using OTUS.HomeWork.Eshop.Domain;
using OTUS.HomeWork.EShop.DAL;
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
        private readonly ProductRepository _productRepository;

        public OrderService(OrderContext orderContext, BillingServiceClient billingClient, ProductRepository productRepository)
        {
            _orderContext = orderContext;
            _billingClient = billingClient;
            _productRepository = productRepository;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            var totalPrice = await CalculateTotalPriceAsync(order.Items);
            PaymentDTO payment;
            try
            {
                payment = await _billingClient.PaymentAsync(order.UserId, new PaymentRequestDTO
                {
                    Amount = totalPrice
                });

            }
            catch(Exception ex)
            {
                // нужно исключение прокидывает специальное, упрощаем
                throw new Exception("При оплате произошла ошибка", ex);
            }
            foreach(var item in order.Items)
            {
                item.Order = order;
            }
            order.TotalPrice = totalPrice;
            order.BillingId = payment.Id.ToString();
            order.PaidDateUtc = DateTime.UtcNow;
            order.CreatedOnUtc = DateTime.UtcNow;
            order.Status = OrderStatus.Complete;
            
            _orderContext.Add(order);
            await _orderContext.SaveChangesAsync();

            return order;
        }

        private async Task<decimal> CalculateTotalPriceAsync(List<OrderItem> items)
        {
            decimal totalPrice = 0.0m;
            foreach(var item in items)
            {
                decimal? price = await _productRepository.GetPriceOfProducsAsync(item.ProductId);
                if(price == null)
                    throw new ArgumentException("Передан неизвестный товар");

                totalPrice += price.Value * item.Quantity;
            }
            return totalPrice;
        }
    }
}
