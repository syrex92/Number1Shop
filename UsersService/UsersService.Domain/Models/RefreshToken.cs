using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Common;

namespace UsersService.Domain.Models
{
    /// <summary>
    /// Модель refresh token для управления сессиями пользователей
    /// Хранит хэшированные refresh tokens с информацией об их сроке действия и статусе
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Хэшированное значение refresh token
        /// Хранится в безопасном виде для верификации при обновлении access token
        /// </summary>
        public string TokenHash { get; set; }

        /// <summary>
        /// Дата и время истечения срока действия токена
        /// После этой даты токен становится невалидным и не может быть использован
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Дата и время отзыва токена
        /// Заполняется при явном выходе пользователя или отзыве токена
        /// </summary>
        public DateTime RevokedAt { get; set; }

        /// <summary>
        /// Дата и время создания токена
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Флаг, указывающий был ли токен отозван
        /// True - токен отозван и не может быть использован
        /// False - токен активен и может использоваться для обновления access token
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Внешний ключ для связи с пользователем
        /// Идентификатор пользователя, которому принадлежит токен
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Навигационное свойство для пользователя
        /// Пользователь, связанный с данным refresh token
        /// </summary>
        public User User { get; set; }
    }

}
