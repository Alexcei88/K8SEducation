using System;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.PaymentGatewayService.DAL;
using OTUS.HomeWork.PaymentGatewayService.Domain;

namespace OTUS.HomeWork.PaymentGatewayService.Services
{
    public interface IBillingService
    {
        Task<Payment> MakePaymentAsync(Guid userId, PaymentRequestDTO paymentRequest);

        Task<Refund> RefundAsync(Guid userId, RefundRequestDTO refundPaymentRequest);
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
            catch (UniqueConstraintException)
            {
                return await _context.Payments.FirstAsync(g => g.IdempotanceKey == paymentRequest.IdempotenceKey);
            }
            return payment;
        }

        public async Task<Refund> RefundAsync(Guid userId, RefundRequestDTO refundRequest)
        {
            var existRefund = await _context.Refunds.FirstOrDefaultAsync(g => g.BillingId == refundRequest.BillingId);
            if (existRefund != null)
                return existRefund;

            var refund = new Refund()
            {
                Date = DateTime.UtcNow,
                UserId = userId,
                BillingId = refundRequest.BillingId,
            };

            try
            {
                _context.Refunds.Add(refund);
                var res = await _context.SaveChangesAsync();
            }
            catch (UniqueConstraintException)
            {
                return await _context.Refunds.FirstAsync(g => g.BillingId == refundRequest.BillingId);
            }
            return refund;
        }
    }
}