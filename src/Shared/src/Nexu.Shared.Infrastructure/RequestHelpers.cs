using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace Nexu.Shared.Infrastructure
{
    public static class RequestHelpers
    {
        public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> values)
        {
            var builder = new StringBuilder();
            foreach (var kvp in values)
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }

                builder.Append(UrlEncoder.Default.Encode(kvp.Key))
                    .Append('=');
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    builder.Append(UrlEncoder.Default.Encode(kvp.Value));
                }
            }

            if (builder.Length == 0)
            {
                return string.Empty;
            }
            return "?" + builder;
        }
    }
}
