using System;
using System.Globalization;
using System.Linq;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;

namespace Tripod
{
    public class MaxLengthTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentOutOfRangeException_WhenMaxLength_IsLessThanOne()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new MaxLength(0));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("maxLength");
            exception.Message.ShouldStartWith(string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Fact]
        public void IsInvalid_WhenStringLength_IsGreaterThanMaxLength()
        {
            var command = new FakeStringLengthCommand { StringProperty = "asdf" };
            var validator = new FakeStringMaxLengthValidator(3);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> maxLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Count(maxLengthError).ShouldEqual(1);
            result.Errors.Single(maxLengthError).ErrorMessage.ShouldEqual(Resources.Validation_MaxLength
                .Replace("{PropertyName}", "String Property")
                .Replace("{MaxLength}", "3")
                .Replace("{TotalLength}", command.StringProperty.Length.ToString(CultureInfo.InvariantCulture))
            );
            validator.ShouldHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }

        [Fact]
        public void IsValid_WhenStringLength_IsEqualToMaxLength()
        {
            var command = new FakeStringLengthCommand { StringProperty = "xxxxx" };
            var validator = new FakeStringMaxLengthValidator(5);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            Func<ValidationFailure, bool> maxLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Any(maxLengthError).ShouldBeFalse();
            validator.ShouldNotHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }

        [Fact]
        public void IsValid_WhenStringLength_IsLessThanMaxLength()
        {
            var command = new FakeStringLengthCommand { StringProperty = FakeData.String() };
            var validator = new FakeStringMaxLengthValidator(command.StringProperty.Length + 1);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            Func<ValidationFailure, bool> maxLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Any(maxLengthError).ShouldBeFalse();
            validator.ShouldNotHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }

        [Fact]
        public void IsValid_WhenString_IsNull()
        {
            var command = new FakeStringLengthCommand { StringProperty = null };
            var validator = new FakeStringMaxLengthValidator(5);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            Func<ValidationFailure, bool> maxLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Any(maxLengthError).ShouldBeFalse();
            validator.ShouldNotHaveValidationErrorFor(x => x.StringProperty, command.StringProperty);
        }
    }
}
