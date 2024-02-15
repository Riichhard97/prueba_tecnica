using System;
using System.ComponentModel.DataAnnotations;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class SendgridOptions
    {
        public const string DefaultEndpoint = "https://api.sendgrid.com/v3/mail/send";

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public Uri Url { get; set; } = new Uri(DefaultEndpoint, UriKind.Absolute);

        public bool SandboxMode { get; set; }
    }
}
