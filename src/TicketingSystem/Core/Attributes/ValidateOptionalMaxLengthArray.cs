using System.ComponentModel.DataAnnotations;
using TicketingSystem.Core.Converters;

namespace TicketingSystem.Core.Attributes
{
    public class ValidateOptionalMaxLengthArray<T>(int length) : ValidationAttribute
    {
        private readonly int _length = length;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            Optional<T[]> optional = (Optional<T[]>)value;

            if (optional.value == null)
            {
                return ValidationResult.Success;
            }

            if (!optional.value.GetType().IsArray)
            {
                return new ValidationResult("This attribute allow validation only array types");
            }

            if (optional.value.Length > _length)
            {
                return new ValidationResult($"{validationContext?.DisplayName} must contain no more that {_length} items");
            }

            return ValidationResult.Success;
        }
    }
}