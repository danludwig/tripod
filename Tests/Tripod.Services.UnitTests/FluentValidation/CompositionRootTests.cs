using FluentValidation;
using Should;
using SimpleInjector;
using Tripod.Services.Transactions;
using Xunit;

namespace Tripod.Services.FluentValidation
{
    public class CompositionRootTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void Sets_ValidatorOptions_CascadeMode_To_StopOnFirstFailure()
        {
            ValidatorOptions.CascadeMode.ShouldEqual(CascadeMode.StopOnFirstFailure);
        }

        [Fact]
        public void Sets_ResourceProviderType_To_ValidationResources()
        {
            ValidatorOptions.ResourceProviderType.ShouldEqual(typeof(Resources));
        }

        [Fact]
        public void RegistersIProcessValidation_UsingValidationProcessor_AsSingleton()
        {
            var instance = Container.GetInstance<IProcessValidation>();
            var registration = Container.GetRegistration(typeof(IProcessValidation));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<ValidationProcessor>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Singleton);
        }

        [Fact]
        public void RegistersIValidator_Transiently_UsingOpenGenerics_WhenValidatorExists()
        {
            var instance = Container.GetInstance<IValidator<FakeCommandWithValidator>>();
            var registration = Container.GetRegistration(typeof(IValidator<FakeCommandWithValidator>));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<ValidateFakeCommand>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
        }

        [Fact]
        public void RegistersIValidator_AsSingleton_UsingValidateNothingDecorator_WhenValidatorDoesNotExist()
        {
            var instance = Container.GetInstance<IValidator<FakeCommandWithoutValidator>>();
            var registration = Container.GetRegistration(typeof(IValidator<FakeCommandWithoutValidator>));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<ValidateNothingDecorator<FakeCommandWithoutValidator>>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Singleton);
        }
    }
}
