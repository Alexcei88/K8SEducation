using System;
using System.Threading.Tasks;
using OTUS.HomeWork.BillingService.DAL;
using OTUS.HomeWork.BillingService.Domain;

namespace OTUS.HomeWork.BillingService.Services
{
    public interface IBillingService
    {
        Task<decimal> CreateBalanceAsync(Guid userId);
        
        Task<decimal> GetBalanceAsync(Guid userId);

        Task<Payment> MakePaymentAsync(PaymentRequestDTO payment);

        Task<Payment> RollbackPaymentAsync(PaymentRequestDTO payment);
    }

    public class BillingService 
        : IBillingService
    {
        private readonly BillingContext _context;
        
        public BillingService(BillingContext context)
        {
            _context = context;
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

        public Task<Payment> MakePaymentAsync(PaymentRequestDTO payment)
        {
            throw new NotImplementedException();
        }

        public Task<Payment> RollbackPaymentAsync(PaymentRequestDTO payment)
        {
            throw new NotImplementedException();
        }
    }
}