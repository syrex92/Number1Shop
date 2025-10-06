using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Models;

namespace UsersService.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            // Составной первичный ключ
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Связи уже настроены в User и Role конфигурациях
            // Дополнительная настройка не требуется

            builder.Property(ur => ur.CreatedAt)
                .IsRequired();
        }
    }
}
