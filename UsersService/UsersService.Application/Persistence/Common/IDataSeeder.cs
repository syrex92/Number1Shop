using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Persistence.Common
{
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}
