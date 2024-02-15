using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Nexu.Shared.AspNetCore.Filters;
using Nexu.Shared.Infrastructure;

namespace Nexu.Shared.AspNetCore
{
    public sealed class NoCurrentAccountExceptionHandler : ICustomExceptionHandler
    {
        private static readonly Action<ILogger, Exception> NoCurrentAccount = LoggerMessage.Define(LogLevel.Warning,
            new EventId(1, nameof(LogNoCurrentAccount)),
            "No account provided in current session.");

        private readonly ILogger<NoCurrentAccountExceptionHandler> _logger;

        public NoCurrentAccountExceptionHandler(ILogger<NoCurrentAccountExceptionHandler> logger)
        {
            _logger = logger;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is NoCurrentProjectException)
            {
                LogNoCurrentAccount(_logger);
                context.Result = new ForbidResult();
            }

            return Task.CompletedTask;
        }

        private static void LogNoCurrentAccount(ILogger logger)
        {
            NoCurrentAccount(logger, null);
        }
    }
}
