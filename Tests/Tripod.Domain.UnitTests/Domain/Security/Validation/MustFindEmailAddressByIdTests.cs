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
    public class MustFindEmailAddressByIdTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new MustFindEmailAddressById(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenEmailAddress_IsNotFoundById()
        {
            var emailAddressId = new Random().Next(1, int.MaxValue);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindEmailAddressByIdCommand { EmailAddressId = emailAddressId };
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Id == emailAddressId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as EmailAddress));
            var validator = new FakeMustFindEmailAddressByIdValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.EmailAddressId);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_DoesNotExist_IntIdValue
                .Replace("{PropertyName}", EmailAddress.Constraints.Label)
                .Replace("{PropertyValue}", emailAddressId.ToString(CultureInfo.InvariantCulture))
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.EmailAddressId, command.EmailAddressId);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenEmailAddress_IsFoundById()
        {
            var emailAddressId = new Random().Next(1, int.MaxValue);
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeMustFindEmailAddressByIdCommand { EmailAddressId = emailAddressId };
            var entity = new ProxiedEmailAddress(emailAddressId);
            Expression<Func<EmailAddressBy, bool>> expectedQuery = x => x.Id == emailAddressId;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity as EmailAddress));
            var validator = new FakeMustFindEmailAddressByIdValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.EmailAddressId, command.EmailAddressId);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }

    public class FakeMustFindEmailAddressByIdCommand
    {
        public int EmailAddressId { get; set; }
    }

    public class FakeMustFindEmailAddressByIdValidator : AbstractValidator<FakeMustFindEmailAddressByIdCommand>
    {
        public FakeMustFindEmailAddressByIdValidator(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddressId)
                .MustFindEmailAddressById(queries)
                .WithName(EmailAddress.Constraints.Label)
            ;
        }
    }
}
