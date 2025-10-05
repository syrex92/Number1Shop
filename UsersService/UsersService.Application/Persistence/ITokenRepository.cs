using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Application.Persistence.Common;
using UsersService.Domain.Models;

namespace UsersService.Application.Persistence
{
    public interface ITokenRepository : IRepository<RefreshToken>
    {
        Task<IEnumerable<RefreshToken>> GetAllTokenAsync(Guid userId);
        Task<RefreshToken?> GetTokenAsync(string refreshToken, Guid id);
    }
}
