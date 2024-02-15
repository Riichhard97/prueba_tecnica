using System;
using Microsoft.AspNetCore.Authentication;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public static class ImpersonateAuthenticationExtensions
    {
        public static AuthenticationBuilder AddImpersonation(this AuthenticationBuilder builder)
            => builder.AddImpersonation(ImpersonateAuthenticationDefaults.DefaultAuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddImpersonation(this AuthenticationBuilder builder, Action<BearerAuthenticationOptions> configureOptions)
            => builder.AddImpersonation(ImpersonateAuthenticationDefaults.DefaultAuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddImpersonation(this AuthenticationBuilder builder, string authenticationScheme, Action<BearerAuthenticationOptions> configureOptions)
            => builder.AddImpersonation(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddImpersonation(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BearerAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<BearerAuthenticationOptions, ImpersonateAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
