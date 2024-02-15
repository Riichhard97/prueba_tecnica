using System;
using Microsoft.AspNetCore.Authentication;
using Nexu.Shared.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AccountAuthenticationExtensions
    {
        public static AuthenticationBuilder AddAccount(this AuthenticationBuilder builder)
            => builder.AddAccount(AccountAuthenticationDefaults.DefaultAuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddAccount(this AuthenticationBuilder builder, Action<BearerAuthenticationOptions> configureOptions)
            => builder.AddAccount(AccountAuthenticationDefaults.DefaultAuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddAccount(this AuthenticationBuilder builder, string authenticationScheme, Action<BearerAuthenticationOptions> configureOptions)
            => builder.AddAccount(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddAccount(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BearerAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<BearerAuthenticationOptions, AccountAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
