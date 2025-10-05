using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    public class LogoutRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("deviceId")]
        public string? DeviceId { get; set; }

        [JsonPropertyName("revokeAllSessions")]
        public bool RevokeAllSessions { get; set; } = false;

        [JsonPropertyName("logoutReason")]
        public string? LogoutReason { get; set; } // "user_initiated", "security_concern", "timeou
    }
}
