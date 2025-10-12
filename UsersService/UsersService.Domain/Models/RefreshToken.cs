using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Common;

namespace UsersService.Domain.Models
{
    public class RefreshToken : BaseEntity
    {
        // Хэшированная строка refresh-токена.
        public string TokenHash { get; set; }
        
        // Дата и время истечения срока действия токена.
        public DateTime Expires { get; set; }

        // Дата и время отзыва токена.
        public DateTime RevokedAt { get; set; }

        // Дата и время создания токена.
        public DateTime Created { get; set; }

        // Флаг, был ли токен отозван.
        public bool IsRevoked { get; set; }
        
        //Внешний ключ на Users
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
