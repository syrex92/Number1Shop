using Microsoft.EntityFrameworkCore;
using UsersService.Application.Persistence;
using UsersService.Domain.Models;
using UsersService.Persistence.DataContext;
using UsersService.Persistence.Repositories.Common;

namespace UsersService.Persistence.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DataBaseContext dataContext) : base(dataContext)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(x => x.UserRoles).ThenInclude(r => r.Role).FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.Include(x => x.UserRoles).ThenInclude(r => r.Role).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
