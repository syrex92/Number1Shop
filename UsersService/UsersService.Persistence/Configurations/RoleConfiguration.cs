using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersService.Domain.Models;

namespace UsersService.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Таблица
            builder.ToTable("Roles");

            // Ключ
            builder.HasKey(r => r.Id);

            // Свойства
            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(150);

            // Индексы
            builder.HasIndex(r => r.RoleName)
                .IsUnique();

            builder.HasMany(r => r.Users)
    .WithMany(u => u.Roles)
    .UsingEntity<UserRole>();

        }
    }
}
