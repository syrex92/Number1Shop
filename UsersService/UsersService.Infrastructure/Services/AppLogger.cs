using UsersService.Application.Services;
using Serilog;

namespace UsersService.Infrastructure.Services
{
    public class AppLogger<T> : IAppLogger<T>
    {
        private readonly ILogger _logger;

        public AppLogger() => _logger = Log.ForContext<T>();

        public void LogDebug(string message, params object[] args)
            => _logger.Debug(message, args);

        public void LogInfo(string message, params object[] args)
            => _logger.Information(message, args);

        public void LogWarning(string message, params object[] args)
            => _logger.Warning(message, args);

        public void LogError(string message, params object[] args)
            => _logger.Error(message, args);

        public void LogFatal(string message, params object[] args)
            => _logger.Fatal(message, args);
    }
}
