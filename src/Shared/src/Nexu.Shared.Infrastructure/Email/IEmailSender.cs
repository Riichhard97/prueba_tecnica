using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure.Email
{
    public interface IEmailSender
    {
        Task<MessageDeliveryResult> SendAsync(MailMessage message);
    }

    public static class EmailSenderExtensions
    {
        public static Task<MessageDeliveryResult> SendAsync(this IEmailSender emailSender, string subject, string email, string body, bool isBodyHtml = true)
        {
            if (emailSender is null)
            {
                throw new ArgumentNullException(nameof(emailSender));
            }

            if (email is null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (subject is null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var mailMessage = CreateMailMessage(subject, email, body, isBodyHtml);

            return emailSender.SendAsync(mailMessage);
        }

        internal static MailMessage CreateMailMessage(string subject, string to, string body = null, bool isBodyHtml = true)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(to);
            mailMessage.Subject = subject;
            if (!string.IsNullOrEmpty(body))
            {
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;
            }
            return mailMessage;
        }
    }
}
