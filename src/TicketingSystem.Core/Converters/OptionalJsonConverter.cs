using System.Text.Json.Serialization;
using System.Text.Json;
using TicketingSystem.Core.Interfaces;

namespace TicketingSystem.Core.Converters
{
    [JsonConverter(typeof(OptionalJsonConverter))]
    public readonly struct Optional<T>(T? Value) : IOptional
    {
        public bool IsPresent { get; } = true;
        public T? Value { get; } = Value;

        public static Optional<T> NotPresent => default;

        bool IOptional.IsPresent => IsPresent;
        object? IOptional.Value => Value;

        public static explicit operator T?(Optional<T> Optional)
        {
            var ret = default(T);
            if (Optional.IsPresent)
            {
                ret = Optional.Value;
            }

            return ret;
        }

        public bool TryGetValue(out T? Value)
        {
            Value = this.Value;

            return IsPresent;
        }
    }

    public class OptionalJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            bool ret = false;

            if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>))
            {
                ret = true;
            }

            return ret;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type TypeOfT = typeToConvert.GetGenericArguments()[0];
            Type ConverterType = typeof(OptionalJsonConverter<>).MakeGenericType(TypeOfT);

            JsonConverter ret = Activator.CreateInstance(ConverterType) as JsonConverter
                ?? throw new NullReferenceException();

            return ret;
        }
    }

    public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
    {
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var RawValue = (T?)JsonSerializer.Deserialize(ref reader, typeof(T), options);

            var ret = new Optional<T>(RawValue);

            return ret;
        }

        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            if (value.IsPresent)
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
}
