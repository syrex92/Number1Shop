using UsersService.Domain.Models;

namespace UsersService.Application.Services
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user);
        Task<User> CreateUserAsync(string name, string email, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(Guid email);
        bool VerifyPassword(User user, string password);
    }
}
