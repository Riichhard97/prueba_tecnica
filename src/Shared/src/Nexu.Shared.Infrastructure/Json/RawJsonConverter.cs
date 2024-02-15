using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nexu.Shared.Infrastructure.Json
{
    /// <summary>
    /// Serializes a string property value as raw JSON. Used for dynamic properties that are serialized as string on the database.
    /// </summary>
    /// <example>
    /// [JsonConverter(typeof(RawJsonConverter))]
    /// public string Data { get; set; }
    /// </example>
    public sealed class RawJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (!string.IsNullOrEmpty(value))
            {
                using var document = JsonDocument.Parse(value);
                document.RootElement.WriteTo(writer);
            }
        }
    }
}
