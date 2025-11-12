using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Common;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель роли пользователя в системе
    /// Определяет уровень доступа и права пользователя
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// Название роли (например: "Admin", "User", "Moderator")
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Коллекция связей пользователей с данной ролью
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
