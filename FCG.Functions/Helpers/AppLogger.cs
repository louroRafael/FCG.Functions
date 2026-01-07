using FCG.Functions.Interfaces;
using Microsoft.Extensions.Logging;

namespace FCG.Functions.Helpers
{
    public class AppLogger<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;
        private readonly ICorrelationIdGenerator _correlationId;

        public AppLogger(ILogger<T> logger, ICorrelationIdGenerator correlationId)
        {
            _logger = logger;
            _correlationId = correlationId;
        }

        public virtual void LogInformation(string message, params object[] args)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = _correlationId.Get()
            }))
            {
                _logger.LogInformation(message, args);
            }
        }

        public virtual void LogWarning(string message, params object[] args)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = _correlationId.Get()
            }))
            {
                _logger.LogWarning(message, args);
            }
        }

        public virtual void LogError(Exception ex, string message, params object[] args)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = _correlationId.Get()
            }))
            {
                _logger.LogError(ex, message, args);
            }
        }

    }
}
