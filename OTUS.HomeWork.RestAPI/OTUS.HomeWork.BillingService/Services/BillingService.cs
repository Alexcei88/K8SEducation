using System;
using System.Threading.Tasks;
using OTUS.HomeWork.BillingService.DAL;
using OTUS.HomeWork.BillingService.Domain;

namespace OTUS.HomeWork.BillingService.Services
{
    internal interface IBillingService
    {
        Task<decimal> ChangeBalance(Guid userId, decimal balance);

        Task<decimal> GetBalance(Guid userId);
    }

    internal class BillingService : IBillingService
    {
        private readonly BillingContext _context;
        
        public BillingService(BillingContext context)
        {
            _context = context;
        }
        
        public async Task<decimal> ChangeBalance(Guid userId, decimal balance)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                await _context.Users.AddAsync(new User
                {
                    Id = userId,
                    Balance = balance,
                });
                await _context.SaveChangesAsync();
                return balance;
            }

            user.Balance += balance;
            await _context.SaveChangesAsync();
            return user.Balance;
        }

        public async Task<decimal> GetBalance(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Balance ?? 0.0m;
        }
    }
}