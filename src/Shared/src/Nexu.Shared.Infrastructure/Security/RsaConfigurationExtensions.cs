using System;
using System.Security.Cryptography;

namespace Nexu.Shared.Infrastructure.Security
{
    public static class RsaConfigurationExtensions
    {
        public static RSA GetPublicKey(this RsaConfigurationBase configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration.PublicKey != null)
            {
                return RsaCommon.FromPemPublicKey(configuration.PublicKey);
            }

            return null;
        }

        public static RSA GetPrivateKey(this RsaConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration.PrivateKey != null)
            {
                return RsaCommon.FromPemPrivateKey(configuration.PrivateKey, configuration.Password);
            }

            return null;
        }
    }
}
