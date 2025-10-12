using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Common
{
    /// <summary>
    /// Абстрактный базовый класс для всех сущностей доменной модели
    /// Содержит общие свойства, которые наследуются всеми моделями
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Уникальный идентификатор сущности
        /// Генерируется автоматически при создании новой записи
        /// </summary>
        public Guid Id { get; set; }
    }
}
