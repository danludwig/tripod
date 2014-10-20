using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class MustFindUserByNameOrEmailTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustFindUserByNameOrEmail(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void IsInvalid_WhenUserNameOrEmail_IsEmpty(string nameOrEmail)
        {
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByNameOrEmailCommand { NameOrEmail = nameOrEmail };
            Expression<Func<UserByNameOrVerifiedEmail, bool>> expectedQuery = x => x.NameOrEmail == nameOrEmail;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(new User()));
            var validator = new FakeMustFindUserByNameOrEmailValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.NameOrEmail);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_CouldNotFind
                .Replace("{PropertyName}", User.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", nameOrEmail)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Never);
            validator.ShouldHaveValidationErrorFor(x => x.NameOrEmail, command.NameOrEmail);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Never);
        }

        [Fact]
        public void IsInvalid_WhenUserNotFound_ByNameOrEmail()
        {
            var nameOrEmail = string.Format("{0}@domain.tld", Guid.NewGuid());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByNameOrEmailCommand { NameOrEmail = nameOrEmail };
            Expression<Func<UserByNameOrVerifiedEmail, bool>> expectedQuery = x => x.NameOrEmail == nameOrEmail;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as User));
            var validator = new FakeMustFindUserByNameOrEmailValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.NameOrEmail);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_CouldNotFind
                .Replace("{PropertyName}", User.Constraints.Label.ToLower())
                .Replace("{PropertyValue}", nameOrEmail)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.NameOrEmail, command.NameOrEmail);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsIValid_WhenUserFound_ByNameOrEmail()
        {
            var nameOrEmail = string.Format("{0}@domain.tld", Guid.NewGuid());
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindUserByNameOrEmailCommand { NameOrEmail = nameOrEmail };
            var entity = new ProxiedUser(new Random().Next(1, int.MaxValue)) { Name = nameOrEmail };
            Expression<Func<UserByNameOrVerifiedEmail, bool>> expectedQuery = x => x.NameOrEmail == nameOrEmail;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as User));
            var validator = new FakeMustFindUserByNameOrEmailValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.NameOrEmail, command.NameOrEmail);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindUserByNameOrEmailCommand
    {
        public string NameOrEmail { get; set; }
    }

    public class FakeMustFindUserByNameOrEmailValidator : AbstractValidator<FakeMustFindUserByNameOrEmailCommand>
    {
        public FakeMustFindUserByNameOrEmailValidator(IProcessQueries queries)
        {
            RuleFor(x => x.NameOrEmail)
                .MustFindUserByNameOrEmail(queries)
                .WithName(User.Constraints.Label)
            ;
        }
    }
}
