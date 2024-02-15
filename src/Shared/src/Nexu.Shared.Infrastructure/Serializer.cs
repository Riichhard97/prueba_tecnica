using System;
using System.Globalization;
using System.Text.Json;
using Nexu.Shared.Infrastructure.Json;

namespace Nexu.Shared.Infrastructure
{
    public static class Serializer
    {
        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = false,
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new UtcDateTimeJsonConverter(),
                //NodaConverters.LocalDateConverter
            }
        };

        public static string Serialize<T>(T obj)
        {
            return Serialize(obj, null);
        }

        public static string Serialize<T>(T obj, JsonSerializerOptions options)
        {
            if (obj is null)
            {
                return null;
            }

            if (obj is IConvertible convertible)
            {
                return convertible.ToString(CultureInfo.InvariantCulture);
            }

            // Since System.Text.Json.JsonSerializer does not currently support polymorphic serialization,
            // we need to cast to object in case T is a subtype of the actual obj instance
            return JsonSerializer.Serialize(obj, typeof(object), options ?? SerializerOptions);
        }

        public static T Deserialize<T>(string value)
        {
            return Deserialize<T>(value, null);
        }

        public static T Deserialize<T>(string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return JsonSerializer.Deserialize<T>(value, options ?? SerializerOptions);
        }

        public static object Deserialize(string value, Type type, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            if (typeof(IConvertible).IsAssignableFrom(type))
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }

            return JsonSerializer.Deserialize(value, type, options ?? SerializerOptions);
        }
    }
}
