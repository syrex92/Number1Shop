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
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public const string User = "user";
        public RoleRepository(DataBaseContext dataContext) : base(dataContext)
        {
        }

        public async Task<Role> GetDefaultRoleAsync()
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == User);
        }
    }
}
