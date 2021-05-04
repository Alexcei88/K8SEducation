using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.PaymentGatewayService.DAL;
using OTUS.HomeWork.PaymentGatewayService.Domain;

namespace OTUS.HomeWork.PaymentGatewayService.Services
{
    public interface IBillingService
    {
        Task<Payment> MakePaymentAsync(Guid userId, PaymentRequestDTO paymentRequest);
    }

    public class PaymentService 
        : IBillingService
    {
        private readonly PaymentContext _context;
        
        public PaymentService(PaymentContext context)
        {
            _context = context;
        }

        public async Task<Payment> MakePaymentAsync(Guid userId, PaymentRequestDTO paymentRequest)
        {            
            var existPayment = await _context.Payments.FirstOrDefaultAsync(g => g.IdempotanceKey == paymentRequest.IdempotenceKey);
            if (existPayment != null)
                return existPayment;

            var payment = new Payment()
            {
                Amount = paymentRequest.Amount,
                Date = DateTime.UtcNow,
                UserId = userId,
                IdempotanceKey = paymentRequest.IdempotenceKey,
            };

            try
            {
                _context.Payments.Add(payment);
                var res = await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Оплата уже была произведена", ex);
            }
            return payment;
        }        
    }
}