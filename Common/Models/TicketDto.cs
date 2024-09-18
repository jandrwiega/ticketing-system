using System.ComponentModel.DataAnnotations;
using TicketingSystem.Common.Enums;
using System.Text.Json.Serialization;
using System.Text.Json;
using TicketingSystem.Common.Interfaces;

namespace TicketingSystem.Common.Models
{
    public class TicketCreateDto
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Ticket title can't be longer than 255 characters")]
        public required string Title { get; set; }

        [MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        public string? Description { get; set; }

        public Guid? Assignee { get; set; }

        public TicketStatusEnum? Status { get; set; }

        [Required]
        public required TicketTypeEnum Type { get; set; }

    }

    public class TicketUpdateDto
    {
        //[MaxLength(255, ErrorMessage = "Ticket title can't be longer than 255 characters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<string> Title { get; set; }

        //[MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<string> Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<Guid> Assignee { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<TicketStatusEnum> Status { get; set; }

        //[MinLength(1, ErrorMessage = "Minimum one child item required")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<Guid[]> RelatedElements { get; set; }
    }

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
            var ret = false;

            if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>))
            {
                ret = true;
            }

            return ret;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {

            var TypeOfT = typeToConvert.GetGenericArguments()[0];
            var ConverterType = typeof(OptionalJsonConverter<>).MakeGenericType(TypeOfT);

            var ret = Activator.CreateInstance(ConverterType) as JsonConverter
                ?? throw new NullReferenceException()
                ;

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
