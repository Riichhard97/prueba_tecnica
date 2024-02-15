using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class MailgunEmailSender : IEmailSender
    {
        private static readonly Action<ILogger, string, Exception> ErrorSendingEmail = LoggerMessage.Define<string>(LogLevel.Error,
            new EventId(1, nameof(LogErrorSendingEmail)),
            "Error sending email. {Error}");

        private readonly HttpClient _client;
        private readonly ILogger<MailgunEmailSender> _logger;
        private readonly MailgunOptions _options;

        public MailgunEmailSender(HttpClient client, IOptions<MailgunOptions> options, ILogger<MailgunEmailSender> logger)
        {
            _client = client;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<MessageDeliveryResult> SendAsync(MailMessage message)
        {
            using var content = await CreateContent(message).ConfigureAwait(false);

            var authentication = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_options.ApiKey}")));
            var url = new Uri(_options.Url, $"{_options.Domain}/messages");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content,
                Headers =
                    {
                        Authorization = authentication
                    }
            };

            var response = await _client.SendAsync(request).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            using var document = JsonDocument.Parse(json);

            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        break;

                    case HttpStatusCode.Unauthorized:
                        return MessageDeliveryResult.Error("Invalid email credentials.");

                    case HttpStatusCode.PaymentRequired:
                        break;

                    case HttpStatusCode.NotFound:
                        break;

                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                        break;
                }

                var error = document.RootElement.GetProperty("message");
                Debug.Assert(error.ValueKind == JsonValueKind.String);
                var errorMessage = error.GetString();
                var logMessage = $"StatusCode: {response.StatusCode}\n" +
                    $"Response: {errorMessage}\n" +
                    $"To={message.To}\n" +
                    $"Subject={message.Subject}\n" +
                    $"Sender={message.Sender}\n" +
                    $"Body={message.Body}";
                LogErrorSendingEmail(_logger, logMessage);
                return MessageDeliveryResult.Error(errorMessage);
            }

            var id = document.RootElement.GetProperty("id");
            Debug.Assert(id.ValueKind == JsonValueKind.String);
            return MessageDeliveryResult.Success(id.GetString());
        }

        private static void LogErrorSendingEmail(ILogger logger, string arg)
        {
            ErrorSendingEmail(logger, arg, null);
        }

        private static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(string key, MailAddressCollection addresses)
        {
            return addresses.Select(x => KeyValuePair.Create(key, x.ToString()));
        }

        private async Task<HttpContent> CreateContent(MailMessage message)
        {
            var parameters = new List<KeyValuePair<string, string>>();
            var from = message.From != null ? message.From.ToString() : _options.From;
            parameters.Add(KeyValuePair.Create("from", from));

            parameters.AddRange(ToKeyValuePairs("to", message.To));
            parameters.AddRange(ToKeyValuePairs("cc", message.CC));
            parameters.AddRange(ToKeyValuePairs("bcc", message.Bcc));
            parameters.Add(KeyValuePair.Create("subject", message.Subject));
            parameters.Add(KeyValuePair.Create(message.IsBodyHtml ? "html" : "text", message.Body));

            foreach (var item in message.ReplyToList)
            {
                parameters.Add(KeyValuePair.Create("h:Reply-To", item.ToString()));
            }

            //foreach (var alternateView in message.AlternateViews)
            //{
            //}
            if (_options.TestMode)
            {
                parameters.Add(KeyValuePair.Create("o:testmode", "yes"));
            }

            var content = new MultipartFormDataContent();

            foreach (var kvp in parameters)
            {
                content.Add(new StringContent(kvp.Value), kvp.Key);
            }

            foreach (var attachment in message.Attachments)
            {
                var memoryStream = new MemoryStream();
                await attachment.ContentStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                memoryStream.Position = 0;
                var attachmentContent = new StreamContent(memoryStream);
                content.Add(attachmentContent, "attachment", attachment.Name);
            }

            return content;
        }
    }
}
