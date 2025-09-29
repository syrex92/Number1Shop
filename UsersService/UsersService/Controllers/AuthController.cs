using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Services;
using UsersService.Domain.Models;

namespace UsersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password);

            if (user == null)
                return Unauthorized("Invalid username or password");

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Role = user.Role.RoleName,
                Username = user.UserName
            });
        }
    }
}
