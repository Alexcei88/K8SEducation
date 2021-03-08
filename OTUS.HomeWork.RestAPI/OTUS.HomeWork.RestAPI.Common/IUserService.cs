using System;
using System.Threading.Tasks;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;

namespace OTUS.HomeWork.RestAPI.Abstraction
{
    public interface IUserService
    {
        Task<User> Authenticate(Guid userId, string password);
        Task<User> GetUserAsync(Guid userId);
        Task<User> CreateUserAsync(User user);
        Task<int> DeleteUserAsync(Guid userId);
        Task<User> UpdateUserAsync(Guid userId, User user);
    }
}