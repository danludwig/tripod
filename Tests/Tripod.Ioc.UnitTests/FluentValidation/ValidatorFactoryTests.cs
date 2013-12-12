using FluentValidation;
using Should;
using Tripod.Ioc.Transactions;
using Xunit;

namespace Tripod.Ioc.FluentValidation
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
