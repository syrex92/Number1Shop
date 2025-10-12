using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UsersService.Application.Persistence;
using UsersService.Application.Services;
using UsersService.Domain;
using UsersService.Domain.Models;
using Xunit.Abstractions;

namespace AuthService.Tests;

public class AuthServiceTests : IClassFixture<AuthServiceFixture>
{
    private readonly AuthServiceFixture _authServiceFixture;
    private readonly ITestOutputHelper _outputHelper;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IRoleRepository _roleRepository;

    public AuthServiceTests(AuthServiceFixture authServiceFixture, ITestOutputHelper outputHelper)
    {
        _authServiceFixture = authServiceFixture;
        _outputHelper = outputHelper;
        _authService = _authServiceFixture.GetService<IAuthService>();
        _userService = _authServiceFixture.GetService<IUserService>();
        _roleRepository = _authServiceFixture.GetService<IRoleRepository>();
    }

    private async Task<HttpClient> CreateAuthorizedClient(User user = null)
    {
        var client = _authServiceFixture.CreateClient();

        // Создаем тестового пользователя если не передан
        var testUser = user ?? await CreateTestUserAsync();

        // Используем AuthService для генерации токена
        var token = _authService.GetAccessToken(testUser);

        _outputHelper.WriteLine("--- AUTH TOKEN ---");
        _outputHelper.WriteLine(token);
        _outputHelper.WriteLine("--- AUTH TOKEN ---");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<User> CreateTestUserAsync(Guid? userId = null, string role = "User")
    {
        var userFromDb = await _userService.GetUserByEmailAsync("test@example.com");
        if (userFromDb is not null) return userFromDb;

        var roles = await _roleRepository.GetAllAsync();
        var userRole = roles.FirstOrDefault(r => r.RoleName == role);
        var user =  new User
        {
            Id = userId ?? Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            PasswordHash = "test-hash",
            UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = userRole.Id,
                        Role = userRole,
                        CreatedAt = DateTime.UtcNow
                    }
                }
        };
        await _userService.AddUser(user);
        return user;

    }
    // Токен с правильной JWT структурой но невалидной подписью
    private string CreateMalformedButStructuredToken()
    {
        // Создаем JWT с правильной структурой но невалидной подписью
        var header = Base64UrlEncoder.Encode("{\"alg\":\"HS256\",\"typ\":\"JWT\"}");
        var payload = Base64UrlEncoder.Encode("{\"sub\":\"123\",\"exp\":9999999999,\"name\":\"test\"}");

        // Создаем невалидную подпись (не Base64Url encoded)
        var invalidSignature = "this-is-not-valid-base64url-signature";

        return $"{header}.{payload}.{invalidSignature}";
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task Login_Should_Return_401_If_Invalid_Credentials()
    {
        // Arrange
        var client = _authServiceFixture.CreateClient();
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Читаем как простой текст
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine($"Response content: '{responseContent}'");

        // Проверяем текст сообщения
        responseContent.Should().Be("Invalid email.");
    }

    [Fact]
    [Trait("Category", "Validation")]
    public async Task Login_Should_Return_400_If_Empty_Fields()
    {
        // Arrange
        var client = _authServiceFixture.CreateClient();
        var request = new LoginRequest
        {
            Email = "",
            Password = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task Login_Should_Return_Tokens_If_Valid_Credentials()
    {
        // Arrange
        var client = _authServiceFixture.CreateClient();
        var request = new LoginRequest
        {
            Email = SeedModels.Admin.Email,
            Password = SeedModels.AdminPassword
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.AccessToken.Should().NotBeNullOrEmpty();
        content.Data.RefreshToken.Should().NotBeNullOrEmpty();
        content.Data.TokenType.Should().Be("Bearer");
        content.Data.ExpiresIn.Should().BeGreaterThan(0);
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task Logout_Should_Return_401_If_Not_Authenticated()
    {
        // Arrange
        var client = _authServiceFixture.CreateClient();
        var request = new LogoutRequest
        {
            RefreshToken = "some-refresh-token"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/logout", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Validation")]
    public async Task Logout_Should_Return_400_If_No_RefreshToken()
    {
        // Arrange
        var testUser = await CreateTestUserAsync();
        var client = await CreateAuthorizedClient(testUser);
        var request = new LogoutRequest
        {
            RefreshToken = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/logout", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task Logout_Should_Succeed_With_Valid_Token()
    {
        // Arrange
        var testUser = await CreateTestUserAsync();
        var client = await CreateAuthorizedClient(testUser);

        // Создаем и сохраняем refresh token через AuthService
        var refreshToken = _authService.GetRefreshToken(testUser);
        await _authService.AddRefreshTokenAsync(refreshToken, testUser.Id);

        var request = new LogoutRequest
        {
            RefreshToken = refreshToken,
            RevokeAllSessions = false
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/logout", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<LogoutResponse>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Message.Should().Contain("success");
    }

    [Fact]
    [Trait("Category", "Validation")]
    public async Task Refresh_Should_Return_400_If_Missing_Tokens()
    {
        // Arrange
        var client = _authServiceFixture.CreateClient();
        var request = new RefreshRequest
        {
            AccessToken = "",
            RefreshToken = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task Refresh_Should_Return_401_If_Invalid_AccessToken()
    {
        // Arrange
        var client = _authServiceFixture.CreateClient();

        // Создаем невалидный токен
        var invalidToken = CreateMalformedButStructuredToken();

        var request = new RefreshRequest
        {
            AccessToken = invalidToken, // Используем структурно правильный но невалидный токен
            RefreshToken = "some-refresh-token"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();
        content.Should().NotBeNull();
        content!.Success.Should().BeFalse();
        content.ErrorCode.Should().Be("INVALID_ACCESS_TOKEN");
    }

    [Fact]
    [Trait("Category", "Security")]
    public async Task Refresh_Should_Return_401_If_Invalid_RefreshToken()
    {
        // Arrange
        var testUser = await CreateTestUserAsync();

        // Генерируем валидный access token через AuthService
        var accessToken = _authService.GetAccessToken(testUser);

        var client = _authServiceFixture.CreateClient();
        var request = new RefreshRequest
        {
            AccessToken = accessToken,
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();
        content.Should().NotBeNull();
        content!.Success.Should().BeFalse();
        content.ErrorCode.Should().Be("INVALID_REFRESH_TOKEN");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Full_Auth_Flow_Should_Work()
    {
        // Arrange
        var client = _authServiceFixture.CreateClient();

        // Act 1: Login
        var loginRequest = new LoginRequest
        {
            Email = SeedModels.Admin.Email,
            Password = SeedModels.AdminPassword
        };

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        loginContent.Should().NotBeNull();
        var accessToken = loginContent!.Data!.AccessToken;
        var refreshToken = loginContent.Data.RefreshToken;

        // Act 2: Use authenticated endpoint
        var authClient = _authServiceFixture.CreateClient();
        authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Act 3: Refresh tokens
        var refreshRequest = new RefreshRequest
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshContent = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();
        refreshContent.Should().NotBeNull();
        refreshContent!.Success.Should().BeTrue();
        refreshContent.Data.Should().NotBeNull();
        refreshContent.Data!.AccessToken.Should().NotBeNullOrEmpty();
        refreshContent.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // Act 4: Logout
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = refreshContent.Data.RefreshToken
        };

        var logoutResponse = await authClient.PostAsJsonAsync("/api/auth/logout", logoutRequest);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [Trait("Category", "Logout")]
    public async Task Logout_Should_Handle_RevokeAllSessions(bool revokeAllSessions)
    {
        // Arrange
        var testUser = await CreateTestUserAsync();
        var client = await CreateAuthorizedClient(testUser);

        // Создаем и сохраняем refresh token
        var refreshToken = _authService.GetRefreshToken(testUser);
        await _authService.AddRefreshTokenAsync(refreshToken, testUser.Id);

        var request = new LogoutRequest
        {
            RefreshToken = refreshToken,
            RevokeAllSessions = revokeAllSessions
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/logout", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<LogoutResponse>();
        content.Should().NotBeNull();
        content!.SessionsRevoked.Should().Be(revokeAllSessions ? "all" : "current");
    }

    [Fact]
    [Trait("Category", "Authentication")]
    public async Task Should_Authenticate_With_AuthService_Token()
    {
        // Arrange
        var testUser = await CreateTestUserAsync();
        var client = await CreateAuthorizedClient(testUser);

        // Act - попробуем вызвать logout endpoint (требует аутентификации)
        var request = new LogoutRequest
        {
            RefreshToken = "any-token-for-auth-test"
        };

        var response = await client.PostAsJsonAsync("/api/auth/logout", request);

        // Assert - если получаем НЕ 401, значит аутентификация работает
        _outputHelper.WriteLine($"Auth Test Response: {response.StatusCode}");

        // Мы ожидаем не 401 (если 400 - это нормально, т.к. refresh token невалидный)
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            // Это нормально - значит аутентификация прошла, но данные запроса невалидны
            var errorContent = await response.Content.ReadAsStringAsync();
            _outputHelper.WriteLine($"Expected validation error: {errorContent}");
        }
    }
}