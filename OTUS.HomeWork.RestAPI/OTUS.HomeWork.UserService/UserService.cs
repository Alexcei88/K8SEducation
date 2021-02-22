using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OTUS.HomeWork.UserService.DAL;
using OTUS.HomeWork.UserService.Domain;

namespace OTUS.HomeWork.UserService
{
    public interface IUserService
    {
        Task<User> Authenticate(Guid userId, string password);
        Task<User> GetUserAsync(Guid userId);
        Task<User> CreateUserAsync(User user);
        Task<int> DeleteUserAsync(Guid userId);
        Task<User> UpdateUserAsync(Guid userId, User user);
    }

    public class UserService 
        : IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly IPasswordHasher<User> _pwdHasher;

        public UserService(UserRepository userRepository, IPasswordHasher<User> pwdHasher)
        {
            _userRepository = userRepository;
            _pwdHasher = pwdHasher;
        }

        public async Task<User> Authenticate(Guid userId, string password)
        {
            var user = await _userRepository.GetUserAsync(userId);
            if (user == null)
                return null;
            if(_pwdHasher.VerifyHashedPassword(user, user.Password, password) != PasswordVerificationResult.Failed)
                return user;

            return null;
        }

        public Task<User> CreateUserAsync(User user)
        {
            user.Password = _pwdHasher.HashPassword(user, user.Password);
            return _userRepository.CreateUserAsync(user);
        }

        public Task<int> DeleteUserAsync(Guid userId)
        {
            return _userRepository.DeleteUserAsync(userId);
        }
      
        public Task<User> GetUserAsync(Guid userId)
        {
            return _userRepository.GetUserAsync(userId);
        }

        public Task<User> UpdateUserAsync(Guid userId, User user)
        {
            return _userRepository.UpdateUserAsync(userId, user);
        }
    }
}
