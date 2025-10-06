using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using UsersService.Application.Services;
using UsersService.Domain.Models;

namespace UsersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IUserService userService)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    return BadRequest("Не заполнены обязательные поля!");

                //находим User по email
                var user = await _userService.GetUserByEmailAsync(request.Email);
                if (user == null)
                    return Unauthorized("Invalid email.");

                // проверяем его пароль
                if (_userService.VerifyPassword(user, request.Password))
                    return Unauthorized("Invalid password");

                var accessToken = _authService.GetAccessToken(user);
                var refreshToken = _authService.GetRefreshToken(user);

                var accessTokenExpiration = DateTime.UtcNow.AddMinutes(60);
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(45);

                // сохраняем refresh токен в БД
                await _authService.AddRefreshTokenAsync(refreshToken, user.Id);

                var response = new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new AuthResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenType = "Bearer",
                        ExpiresIn = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        ExpiresAt = accessTokenExpiration,
                        RefreshTokenExpiresIn = (int)TimeSpan.FromDays(45).TotalSeconds,
                        RefreshTokenExpiresAt = refreshTokenExpiration,
                        Username = user.UserName,
                        Role = user.UserRoles.Select(x => x.Role).FirstOrDefault()?.RoleName ?? string.Empty
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {0}", request.Email);

                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Error = "Internal server error",
                    Message = "Произошла внутренняя ошибка сервера"
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                // Извлечение идентификатора пользователя
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogWarning("Invalid user ID claim in logout request");
                    return Unauthorized(new { message = "Invalid authentication token" });
                }

                // Валидация refresh token
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    _logger.LogWarning("Logout attempt without refresh token from user {UserId}", userId);
                    return BadRequest(new
                    {
                        message = "Refresh token is required",
                        code = "REFRESH_TOKEN_REQUIRED"
                    });
                }

                // Отзыв refresh token
                var isRevoke = await _authService.RevokeRefreshTokenAsync(request.RefreshToken, userId);

                if (!isRevoke)
                    _logger.LogWarning("Failed to revoke refresh token for user {0}.", userId);

                // Дополнительные действия 
                if (request.RevokeAllSessions)
                {
                    await _authService.RevokeAllForUserAsync(userId);
                }

                // 7. Возврат ответа
                return Ok(new LogoutResponse
                {
                    Success = true,
                    Message = "Logged out successfully",
                    LogoutTime = DateTime.UtcNow,
                    SessionsRevoked = request.RevokeAllSessions ? "all" : "current"
                });
            }
            catch (Exception ex)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";

                _logger.LogError(ex, "Unexpected error during logout for user {UserId}", userId);

                return StatusCode(500, new
                {
                    message = "An error occurred during logout",
                    code = "LOGOUT_ERROR"
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            try
            {
                // Валидация входных данных
                if (request == null)
                {
                    return BadRequest(new BaseResponse
                    {
                        Success = false,
                        Message = "Request body is required",
                        ErrorCode = "INVALID_REQUEST"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return BadRequest(new BaseResponse
                    {
                        Success = false,
                        Message = "Access token and refresh token are required",
                        ErrorCode = "MISSING_TOKENS"
                    });
                }

                // 2. Валидация истекшего access token
                ClaimsPrincipal principal;
                try
                {
                    principal = _authService.GetPrincipalFromExpiredToken(request.AccessToken);
                }
                catch (SecurityTokenException ex)
                {
                    _logger.LogWarning("Invalid access token during refresh. Error: {0}", ex.Message);

                    return Unauthorized(new BaseResponse
                    {
                        Success = false,
                        Message = "Invalid access token",
                        ErrorCode = "INVALID_ACCESS_TOKEN"
                    });
                }

                // 3. Извлечение userId из claims
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogWarning("Invalid user ID in token claims. OperationId");
                    return Unauthorized(new BaseResponse
                    {
                        Success = false,
                        Message = "Invalid token claims",
                        ErrorCode = "INVALID_TOKEN_CLAIMS"
                    });
                }

                // 4. Проверка refresh token в базе
                var validationResult = await _authService.ValidateRefreshTokenAsync(request.RefreshToken, userId);
                if (!validationResult)
                {
                    _logger.LogWarning("Invalid refresh token for user {UserId}", userId);

                    return Unauthorized(new BaseResponse
                    {
                        Success = false,
                        Message = "Invalid refresh token",
                        ErrorCode = "INVALID_REFRESH_TOKEN"
                    });
                }

                // 5. Получение пользователя
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found . UserId: {UserId}", userId);

                    return Unauthorized(new BaseResponse
                    {
                        Success = false,
                        Message = "User account not found or inactive",
                        ErrorCode = "USER_INACTIVE"
                    });
                }

                // 6. Генерация новой пары токенов
                var accessTokenExpiration = DateTime.UtcNow.AddMinutes(60);
                var newAccessToken =  _authService.GetAccessToken(user);

                var newRefreshToken = _authService.GetRefreshToken(user);
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(45);

                // 7. Отзыв старого refresh token и сохранение нового
                var revokeResult = await _authService.RevokeRefreshTokenAsync(request.RefreshToken, userId);
                if (!revokeResult)
                {
                    _logger.LogError("Failed to revoke old refresh token for user {UserId}.", userId);
                }

                var storeResult = await _authService.AddRefreshTokenAsync(newRefreshToken, userId);
                if (!storeResult)
                {
                    _logger.LogError("Failed to store new refresh token for user {UserId}.", userId);
                    return StatusCode(500, new BaseResponse
                    {
                        Success = false,
                        Message = "Failed to create new session",
                        ErrorCode = "TOKEN_STORAGE_ERROR"
                    });
                }

                // 9. Возврат новой пары токенов
                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new AuthResponse
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken,
                        TokenType = "Bearer",
                        ExpiresIn = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        ExpiresAt = accessTokenExpiration,
                        RefreshTokenExpiresIn = (int)TimeSpan.FromDays(45).TotalSeconds,
                        RefreshTokenExpiresAt = refreshTokenExpiration,
                        Username = user.UserName,
                        Role = user.UserRoles.Select(x => x.Role).FirstOrDefault()?.RoleName ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token refresh.");

                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred during token refresh",
                    ErrorCode = "REFRESH_ERROR"
                });
            }
        }

    }
}
