using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var users = await _repository.GetAllAsync();
            return users.FirstOrDefault(x => x.Email == email && x.PasswordHash == password);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            var users =  await _repository.GetAllAsync();
            return users.Where(x => x.Role.RoleName == roleName).ToList();
        }
    }
}
