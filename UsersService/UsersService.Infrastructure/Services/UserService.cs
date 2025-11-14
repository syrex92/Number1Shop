using Microsoft.AspNetCore.Identity;
using UsersService.Application.Persistence;
using UsersService.Application.Services;
using UsersService.Domain.Models;

namespace UsersService.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository repository, IRoleRepository roleRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Добавление нового User
        /// </summary>
        public async Task<User> AddUserAsync(User user)
        {
            return await _repository.AddAsync(user);
        }

        /// <summary>
        /// Регистрация нового пользоватлея
        /// </summary>
        /// <param name="name">имя</param>
        /// <param name="email">почтв</param>
        /// <param name="password">пароль</param>
        /// <returns></returns>
        public async Task<User> CreateUserAsync(string name, string email, string password)
        {
            var role = await _roleRepository.GetDefaultRoleAsync();
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = name,
                Email = email,
                UserRoles = new List<UserRole> { new UserRole() {UserId = userId, RoleId = role.Id} }
            };
            var hashPassword = new PasswordHasher<User>().HashPassword(user, password);
            user.PasswordHash = hashPassword;

            await _repository.AddAsync(user);
            return user;
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
