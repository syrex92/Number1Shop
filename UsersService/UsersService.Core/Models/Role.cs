using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Common;

namespace UsersService.Domain.Models
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
