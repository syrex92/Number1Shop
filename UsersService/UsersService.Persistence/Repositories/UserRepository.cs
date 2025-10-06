using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
