using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Nexu.Shared.Infrastructure.Security;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public static class JwtConfigurationExtensions
    {
        public static TokenValidationParameters CreateTokenValidationParameters(this JwtConfigurationBase jwtConfiguration, RsaConfigurationBase rsaConfiguration)
        {
            if (jwtConfiguration is null)
            {
                throw new ArgumentNullException(nameof(jwtConfiguration));
            }

            return new TokenValidationParameters
            {
                ValidateIssuer = jwtConfiguration.Issuer != null,
                ValidateAudience = jwtConfiguration.Audience != null,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfiguration.Issuer,
                ValidAudience = jwtConfiguration.Audience,
                IssuerSigningKey = GetValidationKey(rsaConfiguration),
                ClockSkew = TimeSpan.Zero, // remove delay of token when expire
                AuthenticationType = IdentityConstants.ApplicationScheme
            };
        }

        /// <summary>
        /// Returns the asymmetric JWT validation key.
        /// </summary>
        /// <param name="rsaConfiguration"></param>
        /// <exception cref="JwtConfigurationException">
        /// <see cref="RsaConfigurationBase.PublicKey"/> key is empty.
        /// or
        /// <see cref="RsaConfigurationBase.PublicKey"/> is an invalid RSA key.
        /// </exception>
        public static RsaSecurityKey GetValidationKey(this RsaConfigurationBase rsaConfiguration)
        {
            if (rsaConfiguration is null)
            {
                throw new ArgumentNullException(nameof(rsaConfiguration));
            }

            if (string.IsNullOrEmpty(rsaConfiguration.PublicKey))
            {
                throw new JwtConfigurationException("Public RSA key is empty.");
            }

            var publicRsaParameters = RsaCommon.FromPemPublicKey(rsaConfiguration.PublicKey);
            if (publicRsaParameters == null)
            {
                throw new JwtConfigurationException("Invalid public RSA key.");
            }

            return new RsaSecurityKey(publicRsaParameters);
        }
    }
}
