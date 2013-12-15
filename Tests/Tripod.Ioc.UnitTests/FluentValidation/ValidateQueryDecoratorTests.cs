using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Should;
using Tripod.Ioc.Transactions;
using Xunit;

namespace Tripod.Ioc.FluentValidation
{
    public class ValidateQueryDecoratorTests
    {
        [Fact]
        public void Handle_ThrowsValidationException_AndDoesNotInvokeDecoratedHandle_WhenValidationFails()
        {
            var query = new FakeQuery();
            var decorated = new Mock<IHandleQuery<FakeQuery, string>>(MockBehavior.Strict);
            var validator = new Mock<IValidator<FakeQuery>>(MockBehavior.Strict);
            Expression<Func<FakeQuery, bool>> expectedQuery = x => ReferenceEquals(x, query);
            var expectedResult = new ValidationResult(new[] { new ValidationFailure("Name", "Invalid."), });
            validator.Setup(x => x.Validate(It.Is(expectedQuery))).Returns(expectedResult);
            var decorator = new ValidateQueryDecorator<FakeQuery, string>(decorated.Object, validator.Object);

            var exception = Assert.Throws<ValidationException>(() => decorator.Handle(query));

            exception.ShouldNotBeNull();
            validator.Verify(x => x.Validate(It.Is(expectedQuery)), Times.Once);
            decorated.Verify(x => x.Handle(It.IsAny<FakeQuery>()), Times.Never);
        }

        [Fact]
        public void Handle_InvokesDecoratedHandle_WhenValidationPasses()
        {
            var query = new FakeQuery();
            var decorated = new Mock<IHandleQuery<FakeQuery, string>>(MockBehavior.Strict);
            var validator = new Mock<IValidator<FakeQuery>>(MockBehavior.Strict);
            Expression<Func<FakeQuery, bool>> expectedQuery = x => ReferenceEquals(x, query);
            var expectedResult = new ValidationResult();
            validator.Setup(x => x.Validate(It.Is(expectedQuery))).Returns(expectedResult);
            decorated.Setup(x => x.Handle(It.Is(expectedQuery))).Returns("faked");
            var decorator = new ValidateQueryDecorator<FakeQuery, string>(decorated.Object, validator.Object);

            var result = decorator.Handle(query);

            result.ShouldEqual("faked");
            validator.Verify(x => x.Validate(It.Is(expectedQuery)), Times.Once);
            decorated.Verify(x => x.Handle(It.Is(expectedQuery)), Times.Once);
        }
    }
}
