using System;
using System.Threading.Tasks;
using OTUS.HomeWork.BillingService.DAL;
using OTUS.HomeWork.BillingService.Domain;

namespace OTUS.HomeWork.BillingService.Services
{
    public interface IBillingService
    {
        Task<decimal> CreateBalance(Guid userId);
        
        Task<decimal> ChangeBalance(Guid userId, decimal balance);

        Task<decimal> GetBalance(Guid userId);
    }

    public class BillingService : IBillingService
    {
        private readonly BillingContext _context;
        
        public BillingService(BillingContext context)
        {
            _context = context;
        }

        public async Task<decimal> CreateBalance(Guid userId)
        {
            await _context.Users.AddAsync(new User
            {
                Id = userId,
                Balance = 0,
            });
            await _context.SaveChangesAsync();
            return 0.0m;
        }
        
        public async Task<decimal> ChangeBalance(Guid userId, decimal balance)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Balance += balance;
                await _context.SaveChangesAsync();
                return user.Balance;
            }
            return -1.0m;
        }

        public async Task<decimal> GetBalance(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Balance ?? 0.0m;
        }
    }
}