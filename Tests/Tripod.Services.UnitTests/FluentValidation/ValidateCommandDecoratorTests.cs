using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Should;
using Tripod.Services.Transactions;
using Xunit;

namespace Tripod.Services.FluentValidation
{
    public class ValidateCommandDecoratorTests
    {
        [Fact]
        public void Handle_ThrowsValidationException_AndDoesNotInvokeDecoratedHandle_WhenValidationFails()
        {
            var command = new FakeCommandWithValidator();
            var decorated = new Mock<IHandleCommand<FakeCommandWithValidator>>(MockBehavior.Strict);
            var validator = new Mock<IValidator<FakeCommandWithValidator>>(MockBehavior.Strict);
            Expression<Func<FakeCommandWithValidator, bool>> expectedCommand = x => ReferenceEquals(x, command);
            var expectedResult = new ValidationResult(new[] { new ValidationFailure("Name", "Invalid.", command.ReturnValue), });
            validator.Setup(x => x.Validate(It.Is(expectedCommand))).Returns(expectedResult);
            var decorator = new ValidateCommandDecorator<FakeCommandWithValidator>(decorated.Object, validator.Object);

            var exception = Assert.Throws<ValidationException>(() => decorator.Handle(command));

            exception.ShouldNotBeNull();
            validator.Verify(x => x.Validate(It.Is(expectedCommand)), Times.Once);
            decorated.Verify(x => x.Handle(It.IsAny<FakeCommandWithValidator>()), Times.Never);
        }

        [Fact]
        public void Handle_InvokesDecoratedHandle_WhenValidationPasses()
        {
            var command = new FakeCommandWithValidator();
            var decorated = new Mock<IHandleCommand<FakeCommandWithValidator>>(MockBehavior.Strict);
            var validator = new Mock<IValidator<FakeCommandWithValidator>>(MockBehavior.Strict);
            Expression<Func<FakeCommandWithValidator, bool>> expectedCommand = x => ReferenceEquals(x, command);
            var expectedResult = new ValidationResult();
            validator.Setup(x => x.Validate(It.Is(expectedCommand))).Returns(expectedResult);
            decorated.Setup(x => x.Handle(It.Is(expectedCommand))).Returns(Task.FromResult(0));
            var decorator = new ValidateCommandDecorator<FakeCommandWithValidator>(decorated.Object, validator.Object);

            decorator.Handle(command).Wait();

            validator.Verify(x => x.Validate(It.Is(expectedCommand)), Times.Once);
            decorated.Verify(x => x.Handle(It.Is(expectedCommand)), Times.Once);
        }
    }
}
