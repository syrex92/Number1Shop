using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Common;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Промежуточная модель для связи пользователей и ролей (many-to-many)
    /// Содержит информацию о назначении ролей пользователям
    /// </summary>
    public class UserRole : BaseEntity
    {
        /// <summary>
        /// Дата и время назначения роли пользователю
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Пользователь, которому назначена роль
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Роль, назначенная пользователю
        /// </summary>
        public Role Role { get; set; }
    }
}
