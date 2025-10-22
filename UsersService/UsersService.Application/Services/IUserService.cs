using UsersService.Domain.Models;

namespace UsersService.Application.Services
{
    public interface IUserService
    {
        Task<User> AddUser(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(Guid email);
        bool VerifyPassword(User user, string password);
    }
}
