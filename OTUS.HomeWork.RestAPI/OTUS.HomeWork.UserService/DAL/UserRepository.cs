using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.UserService.Domain;

namespace OTUS.HomeWork.UserService.DAL
{
    public class UserRepository
    {
        private readonly UserContext _userContext;

        public UserRepository(UserContext userContext)
        {
            _userContext = userContext;
        }

        public Task<User> GetUserAsync(Guid userId)
        {
            return _userContext.Users.FirstOrDefaultAsync(g => g.Id == userId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();
            return user;
        }

        public async Task<int> DeleteUserAsync(Guid userId)
        {
            User user = await GetUserAsync(userId);
            if (user != null)
            {
                _userContext.Users.Remove(user);
                return await _userContext.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<User> UpdateUserAsync(Guid userId, User user)
        {
            try
            {
                user.Id = userId;
                _userContext.Entry(user).State = EntityState.Modified;
                var res = await _userContext.SaveChangesAsync();
                if (res > 0)
                    return user;
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
