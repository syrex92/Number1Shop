using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    public class LogoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime LogoutTime { get; set; }
        public string SessionsRevoked { get; set; } = string.Empty; // "current", "all"
    }
}
