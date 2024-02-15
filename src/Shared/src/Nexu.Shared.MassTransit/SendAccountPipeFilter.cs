using System;
using System.Threading.Tasks;
using Nexu.Shared.Infrastructure;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nexu.Shared.MassTransit
{
    public sealed class SendAccountPipeFilter : IFilter<SendContext>, IFilter<PublishContext>
    {
        internal const string AccountHeaderKey = "App_AccountId";

        private readonly IServiceProvider _serviceProvider;

        public SendAccountPipeFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            // Method intentionally left empty.
        }

        public Task Send(SendContext context, IPipe<SendContext> next)
        {
            SetHeader(context);
            return next.Send(context);
        }

        public Task Send(PublishContext context, IPipe<PublishContext> next)
        {
            SetHeader(context);
            return next.Send(context);
        }

        private void SetHeader(SendContext context)
        {
            if (!context.Headers.TryGetHeader(AccountHeaderKey, out _))
            {
                var accountId = GetAccountId(context);
                if (accountId.HasValue)
                {
                    context.Headers.Set(AccountHeaderKey, accountId.ToString());
                }
#if DEBUG
                else
                {
                    throw new InvalidOperationException("No account found.");
                }
#endif
            }
        }

        private Guid? GetAccountId(SendContext context)
        {
            var serviceProvider = GetServiceProvider(context);
            if (serviceProvider != null)
            {
                var currentAccountAccessor = serviceProvider.GetRequiredService<ICurrentProjectAccessor>();
                return currentAccountAccessor.TryGet();
            }

            // Throw?
            return default;
        }

        private IServiceProvider GetServiceProvider(SendContext context)
        {
            if (context.TryGetPayload(out IServiceProvider serviceProvider))
            {
                return serviceProvider;
            }

            if (context.TryGetPayload(out ConsumeContext consumeContext) && consumeContext.TryGetPayload(out serviceProvider))
            {
                return serviceProvider;
            }

            // Dirty hack: If the service provider is not found in the payloads, fall back to the current HttpContext (if any)
            // This situation should only happen the first time a message is sent or published.
            // for other situations there should exist a consume context, which will get a scoped service provider
            // thanks to DependencyInjectionExtensions.UseServiceScope
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            if (httpContextAccessor?.HttpContext != null)
            {
                return httpContextAccessor.HttpContext.RequestServices;
            }

            return null;
        }
    }
}
