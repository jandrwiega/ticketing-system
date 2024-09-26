using System.ComponentModel.DataAnnotations;
using TicketingSystem.Core.Attributes;
using TicketingSystem.Common.Models;
using TicketingSystem.Core.Converters;

namespace TicketingSystem.UnitTests
{
    public class ValidationTests
    {
        [Theory]
        [InlineData(10, "Test", false)]
        [InlineData(1, "Test", true)]
        public void ValidateOptionalMaxLength_ForGivenValues_ValidationSuccess(int maxLength, string input, bool expectError)
        {
            ValidateOptionalMaxLength<string> validator = new ValidateOptionalMaxLength<string>(maxLength);

            bool? result = validator.IsValid(new Optional<string>(input));

            Assert.Equal(!expectError, result);
        }

        [Theory]
        [InlineData(1, "Test", false)]
        [InlineData(10, "Test", true)]
        public void ValidateOptionalMinLength_ForGivenValues_ValidationSuccess(int minLength, string input, bool expectError)
        {
            ValidateOptionalMinLength<string> validator = new ValidateOptionalMinLength<string>(minLength);

            bool? result = validator.IsValid(new Optional<string>(input));

            Assert.Equal(!expectError, result);
        }

        [Theory]
        [InlineData(3, new string[] { "T1", "T2" }, false)]
        [InlineData(1, new string[] { "T1", "T2" }, true)]
        public void ValidateOptionalMaxLengthArray_ForGivenValues_ValidationSuccess(int maxLength, string[] input, bool expectError)
        {
            ValidateOptionalMaxLengthArray<string> validator = new ValidateOptionalMaxLengthArray<string>(maxLength);

            bool? result = validator.IsValid(new Optional<string[]>(input));

            Assert.Equal(!expectError, result);
        }

        [Theory]
        [InlineData(1, new string[] { "T1", "T2" }, false)]
        [InlineData(3, new string[] { "T1", "T2" }, true)]
        public void ValidateOptionalMinLengthArray_ForGivenValues_ValidationSuccess(int minLength, string[] input, bool expectError)
        {
            ValidateOptionalMinLengthArray<string> validator = new ValidateOptionalMinLengthArray<string>(minLength);

            bool? result = validator.IsValid(new Optional<string[]>(input));

            Assert.Equal(!expectError, result);
        }
    }
}