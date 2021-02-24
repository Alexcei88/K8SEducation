using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.UserService.Domain;

namespace OTUS.HomeWork.UserService.DAL
{
    public class UserRepository
    {
        private readonly UserContext _userContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserContext userContext, ILogger<UserRepository> logger)
        {
            _userContext = userContext;
            _userContext.ChangeTracker.AutoDetectChangesEnabled = false;
            _logger = logger;
        }

        public Task<User> GetUserAsync(Guid userId)
        {
            return _userContext.Users.FirstOrDefaultAsync(g => g.Id == userId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _userContext.Users.Add(user);
            _userContext.ChangeTracker.DetectChanges();
            await _userContext.SaveChangesAsync();
            return user;
        }

        public async Task<int> DeleteUserAsync(Guid userId)
        {
            User user = await GetUserAsync(userId);
            if (user != null)
            {
                _userContext.Users.Remove(user);
                _userContext.ChangeTracker.DetectChanges();
                return await _userContext.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<User> UpdateUserAsync(Guid userId, User user)
        {
            try
            {
                var userInDb = await GetUserAsync(userId);
                if (userInDb == null)
                    return null;

                userInDb.Email = user.Email;
                userInDb.FirstName = user.FirstName;
                userInDb.LastName = user.LastName;
                userInDb.Phone = user.Phone;
                userInDb.UserName = user.UserName;
                _userContext.Entry(userInDb).State = EntityState.Modified;
                var res = await _userContext.SaveChangesAsync();
                if (res > 0)
                    return userInDb;
                else
                    return await GetUserAsync(user.Id);
            }
            catch
            {
                return await GetUserAsync(user.Id);
            }
        }
    }
}
