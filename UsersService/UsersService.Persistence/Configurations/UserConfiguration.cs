using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersService.Domain.Models;

namespace UsersService.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Таблица
            builder.ToTable("Users");

            // Ключ
            builder.HasKey(u => u.Id);

            // Свойства
            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.RoleId)
                .IsRequired();

            // Индексы
            builder.HasIndex(u => u.UserName)
                .IsUnique();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasIndex(u => u.RoleId);

            // Связь один-ко-многим с Role
            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
