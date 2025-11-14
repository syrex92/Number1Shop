using UsersService.Application.Persistence.Common;
using UsersService.Domain.Models;

namespace UsersService.Application.Persistence
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetDefaultRoleAsync();
    }
}
