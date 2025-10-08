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
    public class TokenRepository : Repository<RefreshToken>, ITokenRepository
    {
        public TokenRepository(DataBaseContext dataContext) : base(dataContext)
        {

        }

        public async Task<IEnumerable<RefreshToken>> GetAllTokenAsync(Guid userId)
        {
            return await _context.RefreshTokens
        .Where(rt => rt.UserId == userId && !rt.IsRevoked)
        .ToListAsync();
        }

        public async Task<RefreshToken?> GetTokenAsync(string tokenHash, Guid userId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt =>
                              rt.TokenHash == tokenHash &&
                              rt.UserId == userId &&
                              rt.Expires > DateTime.UtcNow &&
                              !rt.IsRevoked);
        }
    }
}
