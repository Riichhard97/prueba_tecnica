using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nexu.Shared.AspNetCore;
using Nexu.Shared.AspNetCore.Authentication;
using Nexu.Shared.Infrastructure.Security;

namespace Nexu.Shared.AspNetCore
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, JwtClientConfiguration jwtConfiguration,
            RsaConfigurationBase jwtRsaConfiguration, RsaConfigurationBase accountRsaConfiguration)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (jwtConfiguration is null)
            {
                throw new ArgumentNullException(nameof(jwtConfiguration));
            }

            if (string.IsNullOrEmpty(jwtConfiguration.Audience))
            {
                throw new AppConfigurationException($"JWT client configuration {nameof(jwtConfiguration.Audience)} is empty.");
            }

            if (string.IsNullOrEmpty(jwtRsaConfiguration?.PublicKey) && string.IsNullOrEmpty(jwtConfiguration.Authority))
            {
                throw new AppConfigurationException(
                    $"Either {nameof(jwtRsaConfiguration.PublicKey)} or {nameof(jwtConfiguration.Authority)} are required for client JWT token validation.");
            }

            if (string.IsNullOrEmpty(accountRsaConfiguration.PublicKey))
            {
                throw new AppConfigurationException("Missing account RSA public key");
            }

            return builder
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, "JWT Bearer", options =>
                {
                    if (!string.IsNullOrEmpty(jwtRsaConfiguration?.PublicKey))
                    {
                        options.TokenValidationParameters = jwtConfiguration.CreateTokenValidationParameters(jwtRsaConfiguration);
                    }
                    else if (!string.IsNullOrEmpty(jwtConfiguration.Authority))
                    {
                        options.Authority = jwtConfiguration.Authority;
                    }

                    options.SaveToken = true;

                    options.Audience = jwtConfiguration.Audience;

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddAccount(AccountAuthenticationDefaults.DefaultAuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = accountRsaConfiguration.GetValidationKey(),
                        ClockSkew = TimeSpan.Zero, // remove delay of token when expire
                        AuthenticationType = AccountAuthenticationDefaults.DefaultAuthenticationScheme
                    };
                })
                .AddImpersonation(ImpersonateAuthenticationDefaults.DefaultAuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = accountRsaConfiguration.GetValidationKey(),
                        ClockSkew = TimeSpan.Zero, // remove delay of token when expire
                        AuthenticationType = ImpersonateAuthenticationDefaults.DefaultAuthenticationScheme
                    };
                });
        }
    }
}
