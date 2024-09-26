using System.ComponentModel.DataAnnotations;
using TicketingSystem.Core.Converters;

namespace TicketingSystem.Core.Attributes
{
    public class ValidateOptionalMinLength<T>(int length) : ValidationAttribute
    {
        private readonly int _length = length;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            Optional<T> optional = (Optional<T>)value;

            if (optional.Value == null)
            {
                return ValidationResult.Success;
            }

            if (optional.Value.GetType().IsArray)
            {
                return new ValidationResult($"Use ValidateOptionalMinLengthArray to validate array length");
            }

            if (optional.Value.ToString()?.Length < _length)
            {
                return new ValidationResult($"{validationContext?.DisplayName} requires at least {_length} characters");
            }

            return ValidationResult.Success;
        }
    }
}