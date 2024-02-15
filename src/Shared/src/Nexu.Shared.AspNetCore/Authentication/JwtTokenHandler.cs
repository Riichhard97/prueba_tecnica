using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Nexu.Shared.AspNetCore.Authentication
{
    public interface IJwtTokenHandler
    {
        string Protect(ClaimsPrincipal principal);
        public ClaimsPrincipal Validate(string token, bool validateLifetime);
    }

    public sealed class JwtTokenHandler : IJwtTokenHandler
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TimeSpan Expiration { get; }

        public JwtTokenHandler(SigningCredentials signingCredentials, TokenValidationParameters tokenValidationParameters, TimeSpan expiration)
        {
            if (expiration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expiration));
            }
            Expiration = expiration;

            _signingCredentials = signingCredentials;
            _tokenValidationParameters = tokenValidationParameters ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
        }

        public string Protect(ClaimsPrincipal principal)
        {
            if (_signingCredentials == null)
            {
                throw new AuthenticationException("Cannot generate a token if the public key is not provided.");
            }
            var handler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var result = handler.CreateEncodedJwt(_tokenValidationParameters.ValidIssuer, _tokenValidationParameters.ValidAudience,
                principal.Identities.First(), now, now.Add(Expiration), now, _signingCredentials);

            return result;
        }

        public ClaimsPrincipal Validate(string token, bool validateLifetime)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = _tokenValidationParameters.Clone();

            validationParameters.ValidateLifetime = validateLifetime;
            validationParameters.LifetimeValidator = validateLifetime ? validationParameters.LifetimeValidator : null;

            var result = handler.ValidateToken(token, validationParameters, out SecurityToken _);
            return result;
        }
    }
}
