using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nexu.Shared.Infrastructure.Email
{
    public class DummyEmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        public string Directory { get; }

        public DummyEmailSender(string directory, ILoggerFactory loggerFactory)
        {
            Directory = directory ?? throw new ArgumentNullException(nameof(directory));
            _logger = loggerFactory.CreateLogger<DummyEmailSender>();
        }

        public async Task<MessageDeliveryResult> SendAsync(MailMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!System.IO.Directory.Exists(Directory))
            {
                throw new DirectoryNotFoundException();
            }

            try
            {
                var timeStamp = string.Format("{0:yy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}", DateTime.Now);
                var temp = Path.GetFileName(Path.GetTempFileName());
                var fileName = string.Format("{0}-{1}.html", timeStamp, temp);
                using (var file = File.Create(Path.Combine(Directory, fileName)))
                {
                    using var streamWriter = new StreamWriter(file, Encoding.UTF8);
                    if (!message.IsBodyHtml)
                    {
                        await streamWriter.WriteAsync("<pre>").ConfigureAwait(false);
                    }
                    await streamWriter.WriteLineAsync($"From: {message.From}").ConfigureAwait(false);
                    await streamWriter.WriteLineAsync($"To: {message.To}").ConfigureAwait(false);
                    await streamWriter.WriteLineAsync(message.Body).ConfigureAwait(false);
                    if (!message.IsBodyHtml)
                    {
                        await streamWriter.WriteAsync("</pre>").ConfigureAwait(false);
                    }
                }

                _logger.LogInformation("Dumped email to {FileName}", fileName);

                return MessageDeliveryResult.Success(fileName);
            }
            catch (Exception ex)
            {
                return MessageDeliveryResult.Error(ex.Message);
            }
        }
    }
}
