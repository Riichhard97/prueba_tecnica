using System;
using System.Threading.Tasks;
using Nexu.Shared.Infrastructure;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Nexu.Shared.MassTransit
{
    /// <summary>
    /// Detects current account information from the message context and sets it on the current account accessor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsumeAccountMessageFilter<T> : IFilter<ConsumeContext<T>>
        where T : class
    {
        public void Probe(ProbeContext context)
        {
            // Method intentionally left empty.
        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var accountId = AccountIdResolver.GetAccountId(context);
            if (accountId.HasValue)
            {
                var serviceProvider = context.GetPayload<IServiceProvider>();

                var currentAccountSetter = serviceProvider.GetRequiredService<ICurrentProjectSetter>();

                currentAccountSetter.TrySet(accountId.Value);
            }
#if DEBUG
            else
            {
                throw new InvalidOperationException("No account found.");
            }
#endif

            return next.Send(context);
        }
    }
}
