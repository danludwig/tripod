using System.Reflection;
using FluentValidation;
using Should;
using SimpleInjector;
using SimpleInjector.Extensions;
using Tripod.Services.Transactions;
using Xunit;

namespace Tripod.Services.FluentValidation
{
    public class ValidationProcessorTests
    {
        [Fact]
        public void ValidateCommand_InvokesValidator_UsingContainerForResolution()
        {
            var container = new Container();
            container.RegisterSingle<IProcessValidation, ValidationProcessor>();
            container.RegisterManyForOpenGeneric(typeof(IValidator<>), Assembly.GetExecutingAssembly());
            container.Verify();
            var validation = container.GetInstance<IProcessValidation>();
            var result = validation.Validate(new FakeCommandWithValidator { InputValue = null });
            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldEqual(1);
        }

        [Fact]
        public void ValidateQuery_InvokesValidator_UsingContainerForResolution()
        {
            var container = new Container();
            container.RegisterSingle<IProcessValidation, ValidationProcessor>();
            container.RegisterManyForOpenGeneric(typeof(IValidator<>), Assembly.GetExecutingAssembly());
            container.Verify();
            var validation = container.GetInstance<IProcessValidation>();
            var result = validation.Validate(new FakeQueryWithValidator { InputValue = null });
            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldEqual(1);
        }
    }
}
