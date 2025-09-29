using Microsoft.AspNetCore.Identity;
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
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;

        public AuthService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> LoginAsync(LoginRequest request)
        {
            var user = await _repository.GetByEmail(request.Email);
            if (user is null)
                return null;

            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Success)
            {
                // TODO: Implement JWT Service
            }
            else
                throw new Exception("UnAuthorized");
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

        public async Task RegisterAsync(RegisterUserRequest request)
        {
            var user = new User()
            {
                UserName = request.UserName,
                Email = request.Email,
            };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            await _repository.AddAsync(user);
        }
    }
}
