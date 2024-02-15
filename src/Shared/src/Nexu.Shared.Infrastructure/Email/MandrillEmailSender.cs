using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nexu.Shared.Infrastructure.Json;

namespace Nexu.Shared.Infrastructure.Email
{
    public sealed class MandrillEmailSender : IEmailSender
    {
        private readonly HttpClient _client;
        private readonly MandrillOptions _options;

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public MandrillEmailSender(HttpClient client, IOptions<MandrillOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<MessageDeliveryResult> SendAsync(MailMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            using var content = await CreateContent(message).ConfigureAwait(false);

            var response = await _client.PostAsync(_options.Url, content).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                using var document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return ExtractResult(document.RootElement);
                }

                var error = document.RootElement.ToObject<MandrillError>();

                return MessageDeliveryResult.Error(error.ToString());
            }

            return MessageDeliveryResult.Error($"Status code {response.StatusCode}. Response: {json}");
        }

        private static async Task<string> ExtractAttachmentContent(Attachment attachment)
        {
            using var memoryStream = new MemoryStream();
            await attachment.ContentStream.CopyToAsync(memoryStream).ConfigureAwait(false);
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        private static MessageDeliveryResult ExtractResult(JsonElement element)
        {
            foreach (var item in element.EnumerateArray())
            {
                var result = item.ToObject<MandrillResponse>();
                switch (result.Status)
                {
                    case "sent":
                    case "queued":
                    case "scheduled":
                        return MessageDeliveryResult.Success(result.Id);

                    case "rejected":
                        return MessageDeliveryResult.Error($"Reject reason: {result.RejectReason}");

                    case "invalid":
                        return MessageDeliveryResult.Error($"Invalid: {item}");

                    default:
                        throw new ArgumentException($"Unknown Mandrill response status '{result.Status}'");
                }
            }

            throw new InvalidOperationException("Empty Mandrill response");
        }

        private async Task<HttpContent> CreateContent(MailMessage message)
        {
            var payload = new Payload()
            {
                Key = _options.ApiKey,
                Message = new PayloadMessage(message, message.From ?? new MailAddress(_options.From))
                {
                    SigningDomain = _options.Domain
                }
            };

            if (message.ReplyToList.Any())
            {
                if (payload.Message.Headers is null)
                {
                    payload.Message.Headers = new Dictionary<string, string>();
                }
                payload.Message.Headers.Add("Reply-To", message.ReplyToList[0].ToString());
            }

            if (message.Attachments.Count > 0)
            {
                payload.Message.Attachments = new List<MessageAttachment>(message.Attachments.Count);
                foreach (var attachment in message.Attachments)
                {
                    payload.Message.Attachments.Add(new MessageAttachment(attachment.ContentType.MediaType, attachment.Name,
                        await ExtractAttachmentContent(attachment).ConfigureAwait(false)));
                }
            }

            var json = JsonSerializer.Serialize(payload, SerializerOptions);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            //var stream = new MemoryStream();
            //await JsonSerializer.SerializeAsync(stream, payload).ConfigureAwait(false);
            //stream.Position = 0;
            //return new StreamContent(stream);
        }

        private sealed class MandrillError
        {
            [JsonPropertyName("code")]
            public int Code { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            public override string ToString()
            {
                return $"Code {Code}; Message {Name}: {Message}";
            }
        }

        private sealed class MandrillResponse
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("_id")]
            public string Id { get; set; }

            [JsonPropertyName("reject_reason")]
            public string RejectReason { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }
        }

        private sealed class MessageAttachment
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            public MessageAttachment(string type, string name, string content)
            {
                Type = type;
                Name = name;
                Content = content;
            }
        }

        private sealed class MessageTo
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            public MessageTo(MailAddress address, string type)
            {
                Email = address.Address;
                if (!string.IsNullOrEmpty(address.DisplayName))
                {
                    Name = address.DisplayName;
                }
                Type = type;
            }
        }

        private sealed class Payload
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("async")]
            public bool Async { get; set; }

            [JsonPropertyName("message")]
            public PayloadMessage Message { get; set; }

            /*
            [JsonPropertyName("send_at")]
            public DateTime? SendAt { get; set; }
            */
        }

        private sealed class PayloadMessage
        {
            [JsonPropertyName("attachments")]
            public List<MessageAttachment> Attachments { get; set; }

            [JsonPropertyName("auto_html")]
            public bool AutoHtml { get; set; }

            [JsonPropertyName("auto_text")]
            public bool AutoText { get; set; }

            [JsonPropertyName("from_email")]
            public string FromEmail { get; set; }

            [JsonPropertyName("from_name")]
            public string FromName { get; set; }

            [JsonPropertyName("headers")]
            public Dictionary<string, string> Headers { get; set; }

            [JsonPropertyName("html")]
            public string Html { get; set; }

            [JsonPropertyName("important")]
            public bool Important { get; set; }

            [JsonPropertyName("signing_domain")]
            public string SigningDomain { get; set; }

            [JsonPropertyName("subject")]
            public string Subject { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("to")]
            public List<MessageTo> To { get; set; }

            /*
            [JsonPropertyName("images")]
            public List<MessageAttachment> Images { get; set; }
            */

            [JsonPropertyName("track_clicks")]
            public bool TrackClicks { get; set; }

            [JsonPropertyName("track_opens")]
            public bool TrackOpens { get; set; }

            public PayloadMessage(MailMessage message, MailAddress from)
            {
                Subject = message.Subject;
                FromEmail = from.Address;
                if (!string.IsNullOrEmpty(from.DisplayName))
                {
                    FromName = from.DisplayName;
                }

                if (message.IsBodyHtml)
                {
                    Html = message.Body;

                    if (message.AlternateViews.Any())
                    {
                        // TODO: Add text view
                    }
                    else
                    {
                        AutoText = true;
                    }
                }
                else
                {
                    Text = message.Body;
                    AutoHtml = true;
                }

                Important = message.Priority == MailPriority.High;

                To = new List<MessageTo>();
                To.AddRange(message.To.Select(x => new MessageTo(x, "to")));
                To.AddRange(message.CC.Select(x => new MessageTo(x, "cc")));
                To.AddRange(message.Bcc.Select(x => new MessageTo(x, "bcc")));

                if (message.Headers.Count > 0)
                {
                    Headers = new Dictionary<string, string>(ExtractHeaders(message.Headers));
                }
            }

            private static IEnumerable<KeyValuePair<string, string>> ExtractHeaders(NameValueCollection headers)
            {
                foreach (var key in headers.AllKeys)
                {
                    yield return KeyValuePair.Create(key, headers[key]);
                }
            }
        }
    }
}
