using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Nexu.Shared.AspNetCore.Authentication
{
    /// <summary>
    /// Provides information needed to control Bearer Authentication handler behavior
    /// </summary>
    public class BearerAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets the ordered list of <see cref="ISecurityTokenValidator"/> used to validate access tokens.
        /// </summary>
        public IList<ISecurityTokenValidator> SecurityTokenValidators { get; } = new List<ISecurityTokenValidator> { new JwtSecurityTokenHandler() };

        /// <summary>
        /// Gets or sets the parameters used to validate identity tokens.
        /// </summary>
        /// <remarks>Contains the types and definitions required for validating a token.</remarks>
        /// <exception cref="ArgumentNullException">if 'value' is null.</exception>
        public TokenValidationParameters TokenValidationParameters { get; set; } = new TokenValidationParameters();

        /// <summary>
        /// The object provided by the application to process events raised by the bearer authentication handler.
        /// The application may implement the interface fully, or it may create an instance of <see cref="BearerAuthenticationEvents" />
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new BearerAuthenticationEvents Events
        {
            get { return (BearerAuthenticationEvents)base.Events; }
            set { base.Events = value; }
        }
    }
}
