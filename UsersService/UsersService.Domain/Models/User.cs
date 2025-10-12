using UsersService.Domain.Common;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель пользователя системы
    /// Содержит основную информацию о пользователе и его учетные данные
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Уникальное имя пользователя для входа в систему
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email пользователя (используется для входа и уведомлений)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Хэш пароля пользователя (хранится в зашифрованном виде)
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Коллекция ролей, назначенных пользователю
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
