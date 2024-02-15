using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure.Email
{
    public interface ITemplateEmailSender
    {
        Task SendAsync(string templateName, object model, IEnumerable<MailMessage> mailMessages);
    }

    public static class TemplateEmailSenderExtensions
    {
        public static async Task SendAsync(this ITemplateEmailSender templateEmailSender, string templateName, object model, string subject,
            IEnumerable<string> toAddresses)
        {
            if (templateEmailSender is null)
            {
                throw new ArgumentNullException(nameof(templateEmailSender));
            }

            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException("Template name must not be null or empty", nameof(templateName));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("Subject must not be null or empty", nameof(subject));
            }

            if (toAddresses is null)
            {
                throw new ArgumentNullException(nameof(toAddresses));
            }

            var mailMessages = toAddresses.Select(x => EmailSenderExtensions.CreateMailMessage(subject, x))
                .ToList();

            await templateEmailSender.SendAsync(templateName, model, mailMessages).ConfigureAwait(false);
        }

        public static async Task SendAsync(this ITemplateEmailSender templateEmailSender, string templateName,
            object model, MailMessage message)
        {
            if (templateEmailSender is null)
            {
                throw new ArgumentNullException(nameof(templateEmailSender));
            }

            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException("Template name must not be null or empty", nameof(templateName));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await templateEmailSender.SendAsync(templateName, model, new[] { message }).ConfigureAwait(false);
        }
    }
}
