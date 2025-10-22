using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель ответа с данными аутентификации
    /// Содержит access token, refresh token и информацию об их сроке действия
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT токен для доступа к защищенным ресурсам API
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Токен для обновления access token при его истечении
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Тип токена (обычно "Bearer")
        /// </summary>
        public string TokenType { get; set; } = string.Empty;

        /// <summary>
        /// Время жизни access token в секундах
        /// </summary>
        public double ExpiresIn { get; set; } // В секундах

        /// <summary>
        /// Время жизни refresh token в секундах
        /// </summary>
        public double RefreshTokenExpiresIn { get; set; } // В секундах

        /// <summary>
        /// Точная дата и время истечения срока действия access token
        /// </summary>
        public DateTime ExpiresAt { get; set; } // Точное время истечения

        /// <summary>
        /// Точная дата и время истечения срока действия refresh token
        /// </summary>
        public DateTime RefreshTokenExpiresAt { get; set; } // Точное время истечения

        /// <summary>
        /// Роль пользователя в системе
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; set; } = string.Empty;
    }
}
