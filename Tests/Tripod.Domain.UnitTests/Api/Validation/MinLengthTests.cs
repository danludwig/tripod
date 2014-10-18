using System;
using System.Globalization;
using System.Linq;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod
{
    public class MinLengthTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentOutOfRangeException_WhenMinLength_IsLessThanOne()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new MinLength(0));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("minLength");
            exception.Message.ShouldStartWith(string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Theory]
        [InlineData("12345678", 9)]
        [InlineData("asdf", 5)]
        [InlineData("x", 2)]
        public void IsInvalid_WhenStringLength_IsLessThanMinLength(string value, int minLength)
        {
            var command = new FakeStringLengthCommand { StringProperty = value };
            var validator = new FakeStringMinLengthValidator(minLength);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> minLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Count(minLengthError).ShouldEqual(1);
            result.Errors.Single(minLengthError).ErrorMessage.ShouldEqual(Resources.Validation_MinLength
                .Replace("{PropertyName}", "String Property")
                .Replace("{MinLength}", minLength.ToString(CultureInfo.InvariantCulture))
                .Replace("{TotalLength}", command.StringProperty.Length.ToString(CultureInfo.InvariantCulture))
                .Replace("{Characters}", value.Length == 1
                    ? Resources.Validation_CharacterLower
                    : Resources.Validation_CharactersLower)
            );
            validator.ShouldHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }

        [Fact]
        public void IsValid_WhenStringLength_IsEqualToMinLength()
        {
            var command = new FakeStringLengthCommand { StringProperty = "xxxxx" };
            var validator = new FakeStringMinLengthValidator(5);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            Func<ValidationFailure, bool> minLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Any(minLengthError).ShouldBeFalse();
            validator.ShouldNotHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }

        [Fact]
        public void IsValid_WhenStringLength_IsGreaterThanMinLength()
        {
            var command = new FakeStringLengthCommand { StringProperty = Guid.NewGuid().ToString() };
            var validator = new FakeStringMinLengthValidator(5);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            Func<ValidationFailure, bool> minLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Any(minLengthError).ShouldBeFalse();
            validator.ShouldNotHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }

        [Fact]
        public void IsValid_WhenString_IsNull()
        {
            var command = new FakeStringLengthCommand { StringProperty = null };
            var validator = new FakeStringMinLengthValidator(new Random().Next(1, int.MaxValue - 3));

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            Func<ValidationFailure, bool> minLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Any(minLengthError).ShouldBeFalse();
            validator.ShouldNotHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }
    }
}
