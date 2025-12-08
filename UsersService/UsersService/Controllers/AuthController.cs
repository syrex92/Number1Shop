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
        private readonly IAppLogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAppLogger<AuthController> logger, IAuthService authService, IUserService userService)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        /// Аутентификация пользователя и выдача токенов доступа
        /// </summary>
        /// <param name="request">Данные для входа (email и пароль)</param>
        /// <returns>Access token и refresh token при успешной аутентификации</returns>
        /// <response code="200">Успешный вход, возвращены токены</response>
        /// <response code="400">Не заполнены обязательные поля</response>
        /// <response code="401">Неверный email или пароль</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
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
                if (!_userService.VerifyPassword(user, request.Password))
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
                        UserId = user.Id,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenType = "Bearer",
                        ExpiresIn = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        ExpiresAt = accessTokenExpiration,
                        RefreshTokenExpiresIn = (int)TimeSpan.FromDays(45).TotalSeconds,
                        RefreshTokenExpiresAt = refreshTokenExpiration,
                        Username = user.UserName,
                        Email = user.Email,
                        Role = user.UserRoles.Select(x => x.Role).FirstOrDefault()?.RoleName ?? string.Empty
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error during login for email {0}", request.Email);

                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Error = "Internal server error",
                    Message = "Произошла внутренняя ошибка сервера"
                });
            }
        }

        /// <summary>
        /// Выход пользователя из системы и отзыв токенов
        /// </summary>
        /// <param name="request">Данные для выхода (refresh token и флаг отзыва всех сессий)</param>
        /// <returns>Результат операции выхода</returns>
        /// <response code="200">Успешный выход из системы</response>
        /// <response code="400">Отсутствует refresh token</response>
        /// <response code="401">Невалидный токен аутентификации</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                // Извлечение идентификатора пользователя
                var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;
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

                // Дополнительные действия для отзыва всех сессий
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

                _logger.LogError(ex.Message, "Unexpected error during logout for user {UserId}", userId);

                return StatusCode(500, new
                {
                    message = "An error occurred during logout",
                    code = "LOGOUT_ERROR"
                });
            }
        }

        /// <summary>
        /// Обновление access token с использованием refresh token
        /// </summary>
        /// <param name="request">Текущие access token и refresh token</param>
        /// <returns>Новая пара токенов доступа</returns>
        /// <response code="200">Токены успешно обновлены</response>
        /// <response code="400">Отсутствуют обязательные токены</response>
        /// <response code="401">Невалидные или просроченные токены</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
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
                var userIdClaim = principal.FindFirst(ClaimTypes.Sid)?.Value;
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
                var newAccessToken = _authService.GetAccessToken(user);

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

                // 8. Возврат новой пары токенов
                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new AuthResponse
                    {
                        UserId = userId,
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken,
                        TokenType = "Bearer",
                        ExpiresIn = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        ExpiresAt = accessTokenExpiration,
                        RefreshTokenExpiresIn = (int)TimeSpan.FromDays(45).TotalSeconds,
                        RefreshTokenExpiresAt = refreshTokenExpiration,
                        Username = user.UserName,
                        Email = user.Email,
                        Role = user.UserRoles.Select(x => x.Role).FirstOrDefault()?.RoleName ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error during token refresh.");

                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "An error occurred during token refresh",
                    ErrorCode = "REFRESH_ERROR"
                });
            }
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="request">Данные для регистрации</param>
        /// <returns>Токены доступа при успешной регистрации</returns>
        /// <response code="200">Успешная регистрация, возвращены токены</response>
        /// <response code="400">Неверные данные регистрации</response>
        /// <response code="409">Пользователь с таким email уже существует</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            try
            {
                // Валидация входных данных
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest(new RegistrationResponse
                    {
                        Success = false,
                        Message = "Все поля обязательны для заполнения",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                // Проверка валидности email
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new RegistrationResponse
                    {
                        Success = false,
                        Message = "Некорректный формат email",
                        ErrorCode = "INVALID_EMAIL"
                    });
                }

                // Проверка сложности пароля
                if (request.Password.Length < 6)
                {
                    return BadRequest(new RegistrationResponse
                    {
                        Success = false,
                        Message = "Пароль должен содержать минимум 6 символов",
                        ErrorCode = "WEAK_PASSWORD"
                    });
                }

                // Проверка существования пользователя
                var existingUser = await _userService.GetUserByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Conflict(new RegistrationResponse
                    {
                        Success = false,
                        Message = "Пользователь с таким email уже существует",
                        ErrorCode = "USER_ALREADY_EXISTS"
                    });
                }

                // Создание нового пользователя
                var newUser = await _userService.CreateUserAsync(request.Name, request.Email, request.Password);
                if (newUser == null)
                {
                    return StatusCode(500, new RegistrationResponse
                    {
                        Success = false,
                        Message = "Ошибка при создании пользователя",
                        ErrorCode = "USER_CREATION_FAILED"
                    });
                }

                // Генерация токенов
                var accessToken = _authService.GetAccessToken(newUser);
                var refreshToken = _authService.GetRefreshToken(newUser);

                var accessTokenExpiration = DateTime.UtcNow.AddMinutes(60);
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(45);

                // Сохранение refresh токена в БД
                await _authService.AddRefreshTokenAsync(refreshToken, newUser.Id);

                // Формирование ответа
                var response = new RegistrationResponse
                {
                    Success = true,
                    Message = "Регистрация выполнена успешно",
                    Data = new AuthResponse
                    {
                        UserId = newUser.Id,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenType = "Bearer",
                        ExpiresIn = (int)TimeSpan.FromMinutes(60).TotalSeconds,
                        ExpiresAt = accessTokenExpiration,
                        RefreshTokenExpiresIn = (int)TimeSpan.FromDays(45).TotalSeconds,
                        RefreshTokenExpiresAt = refreshTokenExpiration,
                        Username = newUser.UserName,
                        Email = newUser.Email,
                        Role = newUser.UserRoles.Select(x => x.Role).FirstOrDefault()?.RoleName ?? "user"
                    }
                };

                _logger.LogInfo("User registered successfully: {Email}", request.Email);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error during registration for email {Email}", request.Email);

                return StatusCode(500, new RegistrationResponse
                {
                    Success = false,
                    Message = "Произошла внутренняя ошибка сервера",
                    ErrorCode = "INTERNAL_SERVER_ERROR"
                });
            }
        }
        // Вспомогательный метод для валидации email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
