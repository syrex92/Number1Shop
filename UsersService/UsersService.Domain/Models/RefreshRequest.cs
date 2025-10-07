using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    public class RefreshRequest
    {
        [Required(ErrorMessage = "Access token is required")]
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh token is required")]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("deviceId")]
        public string? DeviceId { get; set; }
    }
}
