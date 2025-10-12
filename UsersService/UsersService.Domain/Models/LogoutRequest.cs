using System.Text.Json.Serialization;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель запроса для выхода из системы
    /// Содержит refresh token для отзыва и дополнительные параметры
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// Refresh token, который необходимо отозвать
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор устройства (опционально)
        /// </summary>
        [JsonPropertyName("deviceId")]
        public string? DeviceId { get; set; }

        /// <summary>
        /// Флаг отзыва всех активных сессий пользователя
        /// Если true - отзываются все refresh tokens пользователя
        /// </summary>
        [JsonPropertyName("revokeAllSessions")]
        public bool RevokeAllSessions { get; set; } = false;

        /// <summary>
        /// Причина выхода из системы
        /// Возможные значения: "user_initiated", "security_concern", "timeout"
        /// </summary>
        [JsonPropertyName("logoutReason")]
        public string? LogoutReason { get; set; } // "user_initiated", "security_concern", "timeout"
    }
}
