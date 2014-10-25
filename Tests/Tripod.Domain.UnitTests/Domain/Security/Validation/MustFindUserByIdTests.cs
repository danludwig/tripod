using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustFindUserByIdTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustFindUserById(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenUser_IsNotFoundById()
        {
            var userId = FakeData.Id();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByIdCommand { UserId = userId };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Id == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(null as User));
            var validator = new FakeMustFindUserByIdValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.UserId);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_DoesNotExist_IntIdValue
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", userId.ToString(CultureInfo.InvariantCulture))
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.UserId, command.UserId);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsCalid_WhenUser_IsFoundById()
        {
            var userId = FakeData.Id();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByIdCommand { UserId = userId };
            var user = new ProxiedUser(userId);
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Id == userId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(user as User));
            var validator = new FakeMustFindUserByIdValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserId, command.UserId);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindUserByIdCommand
    {
        public int UserId { get; set; }
    }

    public class FakeMustFindUserByIdValidator : AbstractValidator<FakeMustFindUserByIdCommand>
    {
        public FakeMustFindUserByIdValidator(IProcessQueries queries)
        {
            RuleFor(x => x.UserId)
                .MustFindUserById(queries)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
