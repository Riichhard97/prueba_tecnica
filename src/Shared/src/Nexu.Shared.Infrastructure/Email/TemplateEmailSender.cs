using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure.Email
{
    public class TemplateEmailSender : ITemplateEmailSender
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateEngine _templateEngine;

        public TemplateEmailSender(IEmailSender emailSender, ITemplateEngine templateEngine)
        {
            _emailSender = emailSender;
            _templateEngine = templateEngine;
        }

        public async Task SendAsync(string templateName, object model, IEnumerable<MailMessage> mailMessages)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (mailMessages == null)
            {
                throw new ArgumentNullException(nameof(mailMessages));
            }

            var body = await _templateEngine.Render(templateName, model).ConfigureAwait(false);

            foreach (var mailMessage in mailMessages)
            {
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                await _emailSender.SendAsync(mailMessage).ConfigureAwait(true);
            }
        }
    }
}
