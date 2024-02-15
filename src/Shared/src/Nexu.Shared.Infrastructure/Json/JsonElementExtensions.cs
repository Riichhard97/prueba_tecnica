using System.Buffers;
using System.Text.Json;

namespace Nexu.Shared.Infrastructure.Json
{
    public static class JsonElementExtensions
    {
        private static readonly JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new UtcDateTimeJsonConverter(),
            }
        };

        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options ?? DefaultSerializerOptions);
        }
    }
}
