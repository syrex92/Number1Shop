using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель запроса для аутентификации пользователя
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email пользователя
        /// </summary>
        /// <example>user@example.com</example>
        public string Email { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        /// <example>password123</example>
        public string Password { get; set; }
    }
}
