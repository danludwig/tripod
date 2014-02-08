using FluentValidation;
using Should;
using Tripod.Services.Transactions;
using Xunit;

namespace Tripod.Services.FluentValidation
{
    public class ValidatorFactoryTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void CreateInstance_CanCreateInstanceOf_ValidateNothingValidator()
        {
            var factory = new ValidatorFactory(Container);
            var validator = factory.CreateInstance(typeof(IValidator<FakeCommandWithValidator>));
            validator.ShouldNotBeNull();
            validator.ShouldBeType<ValidateFakeCommand>();
        }
    }
}
