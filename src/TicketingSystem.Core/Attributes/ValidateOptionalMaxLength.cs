using System.ComponentModel.DataAnnotations;
using TicketingSystem.Core.Converters;

namespace TicketingSystem.Core.Attributes
{
    public class ValidateOptionalMaxLength<T>(int length) : ValidationAttribute
    {
        private readonly int _length = length;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            Optional<T> optional = (Optional<T>)value; 

            if (optional.value == null)
            {
                return ValidationResult.Success;
            }

            if (optional.value.GetType().IsArray)
            {
                return new ValidationResult($"Use ValidateOptionalMaxLengthArray to validate array length");
            }

            if (optional.value.ToString()?.Length > _length)
            {
                return new ValidationResult($"{validationContext?.DisplayName} must contain no more that {_length} characters");
            }

            return ValidationResult.Success;
        }
    }
}