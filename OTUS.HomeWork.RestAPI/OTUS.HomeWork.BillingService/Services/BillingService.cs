using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.BillingService.DAL;
using OTUS.HomeWork.BillingService.Domain;

namespace OTUS.HomeWork.BillingService.Services
{
    public interface IBillingService
    {
        Task<decimal> CreateBalanceAsync(Guid userId);
        
        Task<decimal> GetBalanceAsync(Guid userId);

        Task<decimal> AddBalanceAsync(Guid userId, BillingTransferRequestDTO billingTransferRequest);

        Task<Payment> MakePaymentAsync(Guid userId, PaymentRequestDTO paymentRequest);

        Task<Payment> RollbackPaymentAsync(Guid userId, PaymentRequestDTO paymentRequest);
    }

    public class BillingService 
        : IBillingService
    {
        private readonly BillingContext _context;
        
        public BillingService(BillingContext context)
        {
            _context = context;
        }

        public async Task<decimal> AddBalanceAsync(Guid userId, BillingTransferRequestDTO billingTransferRequest)
        {            
            var user = await _context.Users.FirstOrDefaultAsync(g => g.Id == userId);
            if (user == null)
                throw new Exception($"User with id {userId} is not found");

            user.Balance += billingTransferRequest.Amount;
            _context.Entry(user).State = EntityState.Modified;
            var res = await _context.SaveChangesAsync();

            return user.Balance;
        }

        public async Task<decimal> CreateBalanceAsync(Guid userId)
        {
            await _context.Users.AddAsync(new User
            {
                Id = userId,
                Balance = 0,
            });
            await _context.SaveChangesAsync();
            return 0.0m;
        }
        
        public async Task<decimal> GetBalanceAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Balance ?? 0.0m;
        }

        public async Task<Payment> MakePaymentAsync(Guid userId, PaymentRequestDTO paymentRequest)
        {            
            var user = await _context.Users.FirstAsync(g => g.Id == userId);
            if (user == null)
                throw new Exception($"User with id {userId} is not found");

            var existPayment = await _context.Payments.FirstOrDefaultAsync(g => g.IdempotanceKey == paymentRequest.IdempotanceKey);
            if (existPayment != null)
                return existPayment;

            if (user.Balance < paymentRequest.Amount)
                throw new Exception("Not enough balance to complete payment");

            var payment = new Payment()
            {
                Amount = paymentRequest.Amount,
                Date = DateTime.UtcNow,
                UserId = userId,
                IdempotanceKey = paymentRequest.IdempotanceKey,
            };

            try
            {
                _context.Payments.Add(payment);

                user.Balance -= paymentRequest.Amount;
                _context.Entry(user).State = EntityState.Modified;
                var res = await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Оплата уже была произведена", ex);
            }
            return payment;
        }

        public async Task<Payment> RollbackPaymentAsync(Guid userId, PaymentRequestDTO paymentRequest)
        {
            var user = await _context.Users.FirstAsync(g => g.Id == userId);
            if (user == null)
                throw new Exception($"User with id {userId} is not found");

            var existPayment = await _context.Payments.FirstOrDefaultAsync(g => g.IdempotanceKey == paymentRequest.IdempotanceKey);
            if (existPayment != null)
                return existPayment;

            var payment = new Payment()
            {
                Amount = paymentRequest.Amount,
                Date = DateTime.UtcNow,
                UserId = userId,
                IdempotanceKey = paymentRequest.IdempotanceKey,
            };

            try
            {
                _context.Payments.Add(payment);

                user.Balance += paymentRequest.Amount;
                _context.Entry(user).State = EntityState.Modified;
                var res = await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Оплата уже была произведена", ex);
            }
            return payment;
        }
    }
}