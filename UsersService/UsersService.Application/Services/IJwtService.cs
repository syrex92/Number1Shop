using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Domain.Models;

namespace UsersService.Application.Services
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
