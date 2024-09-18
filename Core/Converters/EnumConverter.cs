using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicketingSystem.Core.Converters
{
    public class EnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var enumValue = reader.GetString();

            if (Enum.TryParse(enumValue, true, out T result))
            {   
                return result;
            }

            throw new JsonException($"Unable to convert \"{enumValue}\" to enum \"{typeof(T)}\"");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
