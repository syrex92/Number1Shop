using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public double ExpiresIn { get; set; } // В секундах
        public double RefreshTokenExpiresIn { get; set; } // Точное время истечения
        public DateTime ExpiresAt { get; set; } // Точное время истечения
        public DateTime RefreshTokenExpiresAt { get; set; } // Точное время истечения
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
