using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class SendgridEmailSender : IEmailSender
    {
        private readonly HttpClient _client;
        private readonly ILogger<SendgridEmailSender> _logger;
        private readonly SendgridOptions _options;

        private static readonly Action<ILogger, Exception> NoMessageIdHeader = LoggerMessage.Define(LogLevel.Warning,
            new EventId(1, nameof(LogNoMessageIdHeader)),
            "No 'X-Message-Id' received from Sendgrid");

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public SendgridEmailSender(HttpClient client, IOptions<SendgridOptions> options, ILogger<SendgridEmailSender> logger)
        {
            _client = client;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<MessageDeliveryResult> SendAsync(MailMessage message)
        {
            using var content = await CreateContent(message).ConfigureAwait(false);

            var request = new HttpRequestMessage(HttpMethod.Post, _options.Url)
            {
                Content = content,
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey)
                }
            };

            var response = await _client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.TryGetValues("X-Message-Id", out var values))
                {
                    return MessageDeliveryResult.Success(values.FirstOrDefault());
                }

                LogNoMessageIdHeader(_logger);

                return MessageDeliveryResult.Success(null);
            }

            // TODO: Extract error
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return MessageDeliveryResult.Error("Unknown delivery error: " + responseContent);
        }

        private async Task<StringContent> CreateContent(MailMessage message)
        {
            var payload = new Payload
            {
                From = new PayloadAddress(message.From ?? new MailAddress(_options.From))
            };
            payload.Personalizations.Add(new Personalization
            {
                Subject = message.Subject,
                To = PayloadAddress.From(message.To),
                CC = PayloadAddress.From(message.CC),
                Bcc = PayloadAddress.From(message.Bcc)
            });

            if (message.ReplyToList.Any())
            {
                payload.ReplyTo = new PayloadAddress(message.ReplyToList[0]);
            }

            if (message.IsBodyHtml)
            {
                payload.Content.Add(new PayloadContent(message.Body, "text/html"));
            }
            else
            {
                payload.Content.Add(new PayloadContent(message.Body, "text/plain"));
            }

            foreach (var view in message.AlternateViews)
            {
                using var reader = new StreamReader(view.ContentStream);
                var value = await reader.ReadToEndAsync().ConfigureAwait(false);
                payload.Content.Add(new PayloadContent(value, view.ContentType.MediaType));
            }

            if (message.Attachments.Any())
            {
                payload.Attachments = new List<PayloadAttachment>(message.Attachments.Count);
                foreach (var attachment in message.Attachments)
                {
                    payload.Attachments.Add(new PayloadAttachment
                    {
                        Content = await ExtractAttachmentContent(attachment).ConfigureAwait(false),
                        ContentId = attachment.ContentId,
                        Type = attachment.ContentType.MediaType,
                        Disposition = attachment.ContentDisposition.DispositionType,
                        Filename = attachment.ContentDisposition.FileName
                    });
                }
            }

            var json = JsonSerializer.Serialize(payload, SerializerOptions);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json /*"application/json"*/);
        }

        private static async Task<string> ExtractAttachmentContent(Attachment attachment)
        {
            using var memoryStream = new MemoryStream();
            await attachment.ContentStream.CopyToAsync(memoryStream).ConfigureAwait(false);
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        private static void LogNoMessageIdHeader(ILogger logger)
        {
            NoMessageIdHeader(logger, null);
        }

        private sealed class Payload
        {
            [JsonPropertyName("personalizations")]
            public List<Personalization> Personalizations { get; set; } = new List<Personalization>();

            [JsonPropertyName("from")]
            public PayloadAddress From { get; set; }

            [JsonPropertyName("reply_to")]
            public PayloadAddress ReplyTo { get; set; }

            [JsonPropertyName("content")]
            public List<PayloadContent> Content { get; set; } = new List<PayloadContent>();

            [JsonPropertyName("attachments")]
            public List<PayloadAttachment> Attachments { get; set; }

            //[JsonPropertyName("headers")]
            //public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        }

        private sealed class Personalization
        {
            [JsonPropertyName("subject")]
            public string Subject { get; set; }

            [JsonPropertyName("to")]
            public List<PayloadAddress> To { get; set; }

            [JsonPropertyName("cc")]
            public List<PayloadAddress> CC { get; set; }

            [JsonPropertyName("bcc")]
            public List<PayloadAddress> Bcc { get; set; }
        }

        private sealed class PayloadAddress
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            public PayloadAddress(MailAddress address)
            {
                Email = address.Address;
                if (!string.IsNullOrEmpty(address.DisplayName))
                {
                    Name = address.DisplayName;
                }
            }

            public static List<PayloadAddress> From(MailAddressCollection addresses)
            {
                if (addresses.Count > 0)
                {
                    return addresses.Select(x => new PayloadAddress(x)).ToList();
                }
                return null;
            }
        }

        private sealed class PayloadContent
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("value")]
            public string Value { get; set; }

            public PayloadContent(string value, string type)
            {
                Value = value;
                Type = type;
            }
        }

        private sealed class PayloadAttachment
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("content")]
            public string Content { get; set; }

            [JsonPropertyName("filename")]
            public string Filename { get; set; }

            [JsonPropertyName("disposition")]
            public string Disposition { get; set; }

            [JsonPropertyName("content_id")]
            public string ContentId { get; set; }
        }
    }
}
