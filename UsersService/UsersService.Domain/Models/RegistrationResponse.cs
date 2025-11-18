using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Models
{
    public class RegistrationResponse : BaseResponse
    {
        public AuthResponse? Data { get; set; }
    }
}
