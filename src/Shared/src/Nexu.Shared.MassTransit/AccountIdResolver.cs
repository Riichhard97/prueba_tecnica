using System;
using MassTransit;

namespace Nexu.Shared.MassTransit
{
    internal static class AccountIdResolver
    {
        /// <summary>
        /// Extracts the account Id from the specified <see cref="ConsumeContext{T}" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns>The account Id if found, otherwise <c>null</c></returns>
        public static Guid? GetAccountId<T>(ConsumeContext<T> context)
            where T : class
        {
            if (TryGetHeader(context, SendAccountPipeFilter.AccountHeaderKey, out var accountId))
            {
                return accountId;
            }

            if (context.Message is IAccountEvent accountEvent)
            {
                return accountEvent.AccountId;
            }

            return default;
        }

        private static bool TryGetHeader(ConsumeContext context, string key, out Guid value)
        {
            if (context.Headers.TryGetHeader(key, out var header))
            {
                if (header is Guid guid)
                {
                    value = guid;
                    return true;
                }

                if (header is string s)
                {
                    return Guid.TryParse(s, out value);
                }
            }

            value = default;
            return false;
        }
    }
}
