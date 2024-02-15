using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure
{
    /// <summary>
    /// Represents an exception produced by a remote (micro)service over HTTP.
    /// </summary>
    [Serializable]
    public class HttpServiceResponseException : Exception
    {
        public HttpServiceResponseException()
        {
        }

        public HttpServiceResponseException(string message) : base(message)
        {
        }

        public HttpServiceResponseException(string message, Exception inner) : base(message, inner)
        {
        }

        protected HttpServiceResponseException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }

        public static async Task<HttpServiceResponseException> From(HttpResponseMessage response, string message)
        {
            var builder = new StringBuilder(message);
            builder.AppendLine();
            builder.Append(response.RequestMessage.Method.Method)
                .Append(" ")
                .AppendLine(response.RequestMessage.RequestUri.ToString());

            foreach (var header in response.Headers)
            {
                builder.Append(header.Key)
                    .Append(": ");
                var i = 0;
                foreach (var value in header.Value)
                {
                    if (i > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(value);
                    i++;
                }
                builder.AppendLine();
            }

            builder.AppendLine();

            if (response.Content != null)
            {
                builder.AppendLine(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }

            return new HttpServiceResponseException(builder.ToString());
        }
    }
}
