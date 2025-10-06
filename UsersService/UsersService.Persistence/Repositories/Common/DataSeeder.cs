using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UsersService.Application.Persistence;
using UsersService.Application.Persistence.Common;
using UsersService.Domain.Models;

namespace UsersService.Persistence.Repositories.Common
{
    public class DataSeeder : IDataSeeder
    {
        public static string AdminPassword = "1234";

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(IUserRepository userRepository, IRoleRepository roleRepository, IRepository<UserRole> userRoleRepository, ILogger<DataSeeder> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedRolesAsync();
                await SeedAdminUserAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleRepository.AnyAsync())
            {
                var roles = new List<Role>
            {
                new Role { RoleName = "User" },
                new Role { RoleName = "Admin" }
            };
                await _roleRepository.AddAllAsync(roles);

                _logger.LogInformation("Seeded roles: User, Admin");
            }
        }

        private async Task SeedAdminUserAsync()
        {
            if (!await _userRepository.AnyAsync())
            {
                var roles = await _roleRepository.GetAllAsync();
                var adminRole = roles.FirstOrDefault(r => r.RoleName == "Admin");

                if (adminRole == null)
                {
                    throw new InvalidOperationException("Admin role not found during seeding");
                }

                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    UserRoles = new List<UserRole>
                    {
                        new UserRole
                        {
                            RoleId = adminRole.Id,
                            Role = adminRole,
                            CreatedAt = DateTime.UtcNow
                        }
                    }
                };
                adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, AdminPassword);

                await _userRepository.AddAsync(adminUser);

                _logger.LogInformation("Seeded admin user");
            }
        }
    }
}
