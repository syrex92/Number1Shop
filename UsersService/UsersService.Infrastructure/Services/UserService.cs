using Microsoft.AspNetCore.Identity;
using UsersService.Application.Persistence;
using UsersService.Application.Services;
using UsersService.Domain.Models;

namespace UsersService.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Добавление нового User
        /// </summary>
        public async Task<User> AddUser(User user)
        {
            return await _repository.AddAsync(user);
        }

        /// <summary>
        /// Получение User со списком ролей
        /// </summary>
        /// <param name="email"></param>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _repository.GetByEmailAsync(email);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Проверка пароля через хэш-пароль в БД
        /// </summary>
        public bool VerifyPassword(User user, string password)
        {
            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
