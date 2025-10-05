using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    public class UserRole
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
