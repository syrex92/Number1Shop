using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public AuthResponse? Data { get; set; }
        public string? Error { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
