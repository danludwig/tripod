using System;
using System.Globalization;
using System.Linq;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Should;
using Xunit;

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
            exception.Message.ShouldStartWith(Resources.Exception_ArgumentOutOfRange_CannotBeLessThanOne);
        }

        [Fact]
        public void IsInvalid_WhenStringLength_IsLessThanMinLength()
        {
            var command = new FakeStringLengthCommand { StringProperty = "asdf" };
            var validator = new FakeStringMinLengthValidator(5);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> minLengthError = x => x.PropertyName == command.PropertyName(y => y.StringProperty);
            result.Errors.Count(minLengthError).ShouldEqual(1);
            result.Errors.Single(minLengthError).ErrorMessage.ShouldEqual(Resources.Validation_MinLength
                .Replace("{PropertyName}", "String Property")
                .Replace("{MinLength}", "5")
                .Replace("{TotalLength}", command.StringProperty.Length.ToString(CultureInfo.InvariantCulture))
                .Replace("{Characters}", Resources.Validation_CharactersLower)
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
