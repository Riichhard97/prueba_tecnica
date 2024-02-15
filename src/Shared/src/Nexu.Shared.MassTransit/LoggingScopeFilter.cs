using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nexu.Shared.Infrastructure;
using GreenPipes;
using MassTransit;
using MassTransit.Context;
using MassTransit.Logging;

namespace Nexu.Shared.MassTransit
{
    /// <summary>
    /// Creates a new logging scope with
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LoggingScopeFilter<T> : IFilter<ConsumeContext<T>>
        where T : class
    {
        //private readonly ILogger<LoggingScopeFilter> _logger;

        //public LoggingScopeFilter(ILogger<LoggingScopeFilter> logger)
        //{
        //    _logger = logger;
        //}

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("logging");
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            EnabledScope? scope = default;
            try
            {
                var accountId = AccountIdResolver.GetAccountId(context);
                scope = LogContext.BeginScope();
                scope?.Add(nameof(context.MessageId), context.MessageId);
                scope?.Add(nameof(context.RequestId), context.RequestId);
                scope?.Add(nameof(context.CorrelationId), context.CorrelationId);
                scope?.Add(nameof(context.ConversationId), context.ConversationId);
                if (accountId.HasValue)
                {
                    scope?.Add("AccountId", accountId);
                }
                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception e)
                // The exception filter runs in the same context in which the original exception occurred
                when (LogError(e))
            {
                // Never caught, because `LogError()` returns false
            }
            finally
            {
                scope?.Dispose();
            }
        }

        private static bool LogError(Exception ex)
        {
            LogContext.Error?.Log(ex, "An unexpected exception occured");
            //_logger.LogError(ex, "An unexpected exception occured");
            return false;
        }
    }

    public sealed class LoggingScopeFilterSpecification<T> : IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new LoggingScopeFilter<T>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
