using System;
using System.Diagnostics;
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
    public class MustNotFindUserByNameTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustNotFindUserByName<object>(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenUserName_AlreadyExists()
        {
            var userName = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindUserByNameCommand { UserName = userName };
            var entity = new User { Name = command.UserName };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeMustNotFindUserByNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_AlreadyExists
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{PropertyValue}", command.UserName)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command.UserName);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenUserByName_AlreadyExists_AndHasUserId_DifferentFromProvided()
        {
            var userId = FakeData.Id();
            var otherUserId = FakeData.Id(userId);
            var userName = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindUserByNameCommand
            {
                UserName = userName,
                UserId = otherUserId,
            };
            var entity = new ProxiedUser(userId)
            {
                Name = command.UserName,
            };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as User));
            var validator = new FakeMustNotFindUserByNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_AlreadyExists
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{PropertyValue}", command.UserName)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenUserByName_DoesNotExist()
        {
            var userName = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindUserByNameCommand { UserName = userName };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as User));
            var validator = new FakeMustNotFindUserByNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command.UserName);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenUserByName_AlreadyExists_ButHasUserId_SameAsProvided()
        {
            var userId = FakeData.Id();
            var userName = FakeData.String();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustNotFindUserByNameCommand
            {
                UserName = userName,
                UserId = userId,
            };
            var entity = new ProxiedUser(userId)
            {
                Name = command.UserName,
            };
            Expression<Func<UserBy, bool>> expectedQuery = x => x.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as User));
            var validator = new FakeMustNotFindUserByNameValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustNotFindUserByNameCommand
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
    }

    public class FakeMustNotFindUserByNameValidator : AbstractValidator<FakeMustNotFindUserByNameCommand>
    {
        public FakeMustNotFindUserByNameValidator(IProcessQueries queries)
        {
            When(x => !x.UserId.HasValue, () =>
                RuleFor(x => x.UserName)
                    .MustNotFindUserByName(queries)
                    .WithName(User.Constraints.NameLabel)
            );

            When(x => x.UserId.HasValue, () =>
                RuleFor(x => x.UserName)
                    .MustNotFindUserByName(queries, x =>
                    {
                        Debug.Assert(x.UserId.HasValue);
                        return x.UserId.Value;
                    })
                .WithName(User.Constraints.NameLabel));
        }
    }
}
