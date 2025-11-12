using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель ответа на запрос аутентификации
    /// Содержит флаг успеха операции, данные аутентификации или информацию об ошибке
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Флаг успешного выполнения операции
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Данные аутентификации (токены и информация о пользователе)
        /// </summary>
        public AuthResponse? Data { get; set; }

        /// <summary>
        /// Описание ошибки (заполняется при Success = false)
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Сообщение о результате операции
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
