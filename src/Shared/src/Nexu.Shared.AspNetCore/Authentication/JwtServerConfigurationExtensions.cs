using System;
using Microsoft.IdentityModel.Tokens;
using Nexu.Shared.Infrastructure.Security;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public static class JwtServerConfigurationExtensions
    {
        /// <summary>
        /// Returns the <see cref="SigningCredentials" /> from the <see cref="JwtServerConfiguration.PrivateKey" />
        /// </summary>
        /// <param name="rsaConfiguration"></param>
        /// <summary>
        /// Returns the asymmetric JWT validation key.
        /// </summary>
        /// <param name="rsaConfiguration"></param>
        /// <exception cref="JwtConfigurationException">
        /// <see cref="JwtServerConfiguration.PrivateKey"/> key is empty.
        /// </exception>
        public static SigningCredentials GetSigningCredentials(this RsaConfiguration rsaConfiguration)
        {
            if (rsaConfiguration is null)
            {
                throw new ArgumentNullException(nameof(rsaConfiguration));
            }

            if (string.IsNullOrEmpty(rsaConfiguration.PrivateKey))
            {
                throw new JwtConfigurationException("Private RSA key is empty.");
            }

            var privateRsaParameters = rsaConfiguration.GetPrivateKey();
            return new SigningCredentials(new RsaSecurityKey(privateRsaParameters), SecurityAlgorithms.RsaSha256);
        }
    }
}
