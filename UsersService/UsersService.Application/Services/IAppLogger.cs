using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Services
{
    public interface IAppLogger<T>
    {
        void LogInfo(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogDebug(string message, params object[] args); 
        void LogFatal(string message, params object[] args); 
    }
}
