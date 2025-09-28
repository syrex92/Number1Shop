using UsersService.Domain.Models;

namespace UsersService.Application.Services
{
    public interface IUserService
    {
        public Task<User?> AuthenticateAsync(string email, string password);
        public Task<User?> GetByIdAsync(Guid id);
        public Task<List<User>> GetUsersByRoleAsync(string roleName);
    }
}
