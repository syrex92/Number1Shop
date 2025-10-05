using System.Security.Claims;
using UsersService.Domain.Models;

namespace UsersService.Application.Services
{
    public interface IAuthService
    {
        string GetAccessToken(User user);
        string GetRefreshToken(User user);
        Task<bool> AddRefreshTokenAsync(string refreshToken, Guid id);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid id);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, Guid id);
        Task RevokeAllForUserAsync(Guid userId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
    }
}
