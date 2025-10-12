using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель запроса для обновления токенов
    /// Используется для получения новой пары access/refresh token
    /// </summary>
    public class RefreshRequest
    {
        /// <summary>
        /// Истекший access token, который требуется обновить
        /// </summary>
        [Required(ErrorMessage = "Access token is required")]
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Действующий refresh token для верификации запроса
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор устройства (опционально)
        /// </summary>
        [JsonPropertyName("deviceId")]
        public string? DeviceId { get; set; }
    }

}
