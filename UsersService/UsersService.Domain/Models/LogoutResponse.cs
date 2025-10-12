using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель ответа на запрос выхода из системы
    /// Подтверждает успешный выход и предоставляет информацию об отозванных сессиях
    /// </summary>
    public class LogoutResponse
    {
        /// <summary>
        /// Флаг успешного выполнения операции
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение о результате операции
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Время выполнения выхода из системы
        /// </summary>
        public DateTime LogoutTime { get; set; }

        /// <summary>
        /// Информация об отозванных сессиях
        /// Возможные значения: "current" (текущая сессия), "all" (все сессии)
        /// </summary>
        public string SessionsRevoked { get; set; } = string.Empty; // "current", "all"
    }
}
