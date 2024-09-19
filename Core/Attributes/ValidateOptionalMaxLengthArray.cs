using System.ComponentModel.DataAnnotations;
using TicketingSystem.Core.Converters;

namespace TicketingSystem.Core.Attributes
{
    public class ValidateOptionalMaxLengthArray<T>(int length) : ValidationAttribute
    {
        private readonly int _length = length;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Optional<T[]> optional = (Optional<T[]>)value;

            if (optional.Value == null)
            {
                return ValidationResult.Success;
            }

            if (!optional.Value.GetType().IsArray)
            {
                return new ValidationResult("This attribute allow validation only array types");
            }

            if (optional.Value.Length > _length)
            {
                return new ValidationResult($"{validationContext.DisplayName} must contain no more that {_length} items");
            }

            return ValidationResult.Success;
        }
    }
}