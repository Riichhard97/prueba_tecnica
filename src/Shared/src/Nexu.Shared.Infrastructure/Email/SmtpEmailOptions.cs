using System.ComponentModel.DataAnnotations;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class SmtpEmailOptions
    {
        [Required]
        public string Server { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string Password { get; set; }
        public int? Port { get; set; }
        public bool DefaultCredentials { get; set; }
        public bool EnableSsl { get; set; }
    }
}
