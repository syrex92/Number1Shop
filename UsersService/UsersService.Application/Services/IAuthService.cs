using UsersService.Domain.Models;

namespace UsersService.Application.Services
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(LoginRequest request);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetUsersByRoleAsync(string roleName);
        Task RegisterAsync(RegisterUserRequest request);
    }
}
