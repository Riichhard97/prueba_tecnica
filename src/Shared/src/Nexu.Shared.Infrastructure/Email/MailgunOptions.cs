using System;
using System.ComponentModel.DataAnnotations;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class MailgunOptions
    {
        public const string DefaultEndpoint = "https://api.mailgun.net/v3/";

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public Uri Url { get; set; } = new Uri(DefaultEndpoint, UriKind.Absolute);

        [Required]
        public string Domain { get; set; }

        public bool TestMode { get; set; }
    }
}
