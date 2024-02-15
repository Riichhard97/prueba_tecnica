using System;
using System.ComponentModel.DataAnnotations;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class MandrillOptions
    {
        public const string DefaultEndpoint = "https://mandrillapp.com/api/1.0/messages/send";

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public Uri Url { get; set; } = new Uri(DefaultEndpoint, UriKind.Absolute);

        public string Domain { get; set; }
    }
}
