using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Models;

namespace UsersService.Domain
{
    /// <summary>
    /// Первичные данны для инициализации БД
    /// </summary>
    public static class SeedModels
    {
        public static Guid UserId { get; } = Guid.Parse("181BCD21-0EEB-4C9B-A495-F581901A7B1A");

        public static string AdminPassword = "123456";
        public static List<Role> Roles { get; } = [new Role { RoleName = "User" }, new Role { RoleName = "Admin" }];
        public static User Admin { get; } = new User { Id = UserId, UserName = "admin", Email = "admin@example.com", };
    }
}
