using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpEmailOptions _options;

        public SmtpEmailSender(IOptions<SmtpEmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task<MessageDeliveryResult> SendAsync(MailMessage message)
        {
            string NotSentRecipients = "";
            try
            {
                MailMessage msg = new MailMessage();
                var from = message.From != null ? message.From.ToString() : _options.From;
                if (!string.IsNullOrWhiteSpace(from)) msg.From = new MailAddress(from);
                foreach (MailAddress item in message.To)
                {
                    msg.To.Add(item);
                }
                foreach (MailAddress item in message.CC)
                {
                    msg.CC.Add(item);
                }
                foreach (MailAddress item in message.Bcc)
                {
                    msg.Bcc.Add(item);
                }
                msg.Subject = message.Subject;
                msg.Body = message.Body;
                msg.IsBodyHtml = message.IsBodyHtml;
                foreach (Attachment item in message.Attachments)
                    msg.Attachments.Add(item);

                using (SmtpClient smtp = new SmtpClient(_options.Server))
                {
                    smtp.Credentials = new NetworkCredential(_options.From, _options.Password);
                    smtp.UseDefaultCredentials = _options.DefaultCredentials;
                    smtp.EnableSsl = _options.EnableSsl;

                    if (_options.Port != null)
                        smtp.Port = (int)_options.Port;

                    await smtp.SendMailAsync(msg);
                };

                return MessageDeliveryResult.Success("success");
            }
            catch (SmtpFailedRecipientsException ex)
            {
                NotSentRecipients = "";
                foreach (Exception innerEx in ex.InnerExceptions)
                {
                    SmtpFailedRecipientException failedRecipEx = innerEx as SmtpFailedRecipientException;
                    if (failedRecipEx != null)
                    {
                        NotSentRecipients += (failedRecipEx.FailedRecipient.Replace("<", "").Replace(">", ""));
                    }
                }
                return MessageDeliveryResult.Error(NotSentRecipients);
            }
            catch (SmtpFailedRecipientException ex)
            {
                NotSentRecipients += ex.FailedRecipient.Replace("<", "").Replace(">", "");
                return MessageDeliveryResult.Error(NotSentRecipients);
            }
            catch (Exception ex)
            {
                return MessageDeliveryResult.Error(ex.Message);
            }
        }
    }
}
