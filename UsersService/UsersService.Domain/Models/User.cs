using UsersService.Domain.Common;

namespace UsersService.Domain.Models
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
