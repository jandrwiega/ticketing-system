﻿using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Core.Attributes
{
    public class DependentValidation(string _dependentProperty, object _targetValue) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dtoProperties = validationContext.ObjectType.GetProperties();
            var propertyInfo = dtoProperties.SingleOrDefault(prop => prop.Name.Equals(_dependentProperty, StringComparison.CurrentCultureIgnoreCase));

            if (propertyInfo == null)
            {
                return new ValidationResult($"Unknown property: {_dependentProperty}");
            }

            var dependentValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (value is not null && dependentValue?.ToString()?.ToLower() != _targetValue.ToString()?.ToLower())
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
