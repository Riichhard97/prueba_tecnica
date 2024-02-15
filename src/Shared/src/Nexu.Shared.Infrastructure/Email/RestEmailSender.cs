using System;
using System.Net.Http;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nexu.Shared.Infrastructure.Email
{
    public abstract class RestEmailSender : IEmailSender
    {
        protected HttpClient Client { get; }

        protected ILogger Logger { get; }

        protected abstract Uri Url { get; }

        protected RestEmailSender(HttpClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;
        }

        public async Task<MessageDeliveryResult> SendAsync(MailMessage message)
        {
            using var content = await CreateContent(message).ConfigureAwait(false);

            var request = await CreateRequest().ConfigureAwait(false);
            request.Content = content;

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            using var document = JsonDocument.Parse(json);
            return HandleResponse(response, document.RootElement);
        }

        protected virtual Task<HttpRequestMessage> CreateRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Url);
            return Task.FromResult(request);
        }

        protected abstract Task<HttpContent> CreateContent(MailMessage message);

        protected abstract MessageDeliveryResult HandleResponse(HttpResponseMessage response, JsonElement element);
    }
}
