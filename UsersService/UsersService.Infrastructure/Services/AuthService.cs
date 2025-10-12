using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UsersService.Application.Persistence;
using UsersService.Application.Persistence.Common;
using UsersService.Application.Services;
using UsersService.Domain.Models;

namespace UsersService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly int _refreshTokenSize = 64;
        private readonly int _refreshTokenExpiryDays = 60;
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _repositoryToken;
        private readonly PasswordHasher<object> _passwordHasher;

        public AuthService(IConfiguration configuration, ITokenRepository repositoryToken)
        {
            _configuration = configuration;
            _repositoryToken = repositoryToken;
            _passwordHasher = new PasswordHasher<object>();
        }

        public string GetAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Добавляем роль из связанной таблицы
            if (user.UserRoles.FirstOrDefault() != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.UserRoles.Select(x => x.Role).FirstOrDefault().RoleName));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GetRefreshToken(User user)
        {
            var randomNumber = new byte[_refreshTokenSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<bool> AddRefreshTokenAsync(string refreshToken, Guid id)
        {
            // Используем SHA256 для хэширования refresh token
            var refreshTokenHash = HashRefreshToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = id,
                Expires = DateTime.UtcNow.AddDays(45),
                Created = DateTime.UtcNow
            };

            var tokenDB = await _repositoryToken.AddAsync(refreshTokenEntity);
            return tokenDB != null;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid userId)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var tokenDb = await _repositoryToken.GetTokenAsync(tokenHash, userId);

            return tokenDb != null;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, Guid userId)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var tokenDb = await _repositoryToken.GetTokenAsync(tokenHash, userId);

            if (tokenDb != null && !tokenDb.IsRevoked)
            {
                tokenDb.IsRevoked = true;
                tokenDb.RevokedAt = DateTime.UtcNow;
                await _repositoryToken.UpdateAsync(tokenDb);
                return true;
            }
            return false;
        }
        public async Task RevokeAllForUserAsync(Guid userId)
        {
            var tokensDB = await _repositoryToken.GetAllTokenAsync(userId);
            foreach (var token in tokensDB)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                await _repositoryToken.UpdateAsync(token);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // На этом этапе нам нужен только userId
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)),
                ValidateLifetime = false // Важно: отключаем проверку срока действия
            };

            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            // Дополнительная проверка алгоритма подписи
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            }

            return principal;

        }
        // Единый метод для хэширования refresh token
        private string HashRefreshToken(string refreshToken)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(refreshToken);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
