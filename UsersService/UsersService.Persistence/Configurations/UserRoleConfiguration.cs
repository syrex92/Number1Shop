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
            // Настройка имени таблицы (опционально)
            builder.ToTable("UserRoles");

            // Составной первичный ключ
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Настройка связи с User
            builder.HasOne(ur => ur.User)
                .WithMany()  // Без указания навигационного свойства
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи с Role
            builder.HasOne(ur => ur.Role)
                .WithMany()  // Без указания навигационного свойства
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
