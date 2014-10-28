using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class DeleteRemoteMembershipTests : FluentValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Validator_LoginProvider_IsInvalid_WhenEmpty(string loginProvider)
        {
            var command = new DeleteRemoteMembership
            {
                LoginProvider = loginProvider,
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var validator = new ValidateDeleteRemoteMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.LoginProvider);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .notempty_error
                .Replace("{PropertyName}", RemoteMembership.Constraints.ProviderLabel)
            );
        }

        [Fact]
        public void Validator_Principal_IsInvalid_WhenNoLocalMembershipExists_AndNoOtherRemoteMembershipsExist()
        {
            var command = new DeleteRemoteMembership
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.IdString()),
                    new Claim(ClaimTypes.Name, FakeData.String()),
                }, "authenticationType"), null),
                LoginProvider = FakeData.String(),
                ProviderKey = FakeData.String(),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            User user = new ProxiedUser(command.Principal.Identity.GetUserId<int>());
            var remoteMembershipToDelete = new ProxiedRemoteMembership(command.LoginProvider, command.ProviderKey)
            {
                User = user,
                UserId = user.Id
            };
            user.RemoteMemberships.Add(remoteMembershipToDelete);
            Expression<Func<UserBy, bool>> expectedUserByQuery = y => y.Principal == command.Principal;
            queries.Setup(x => x.Execute(It.Is(expectedUserByQuery)))
                .Returns(Task.FromResult(user));
            queries.Setup(x => x.Execute(It.Is<RemoteMembershipsByUser>(y => y.UserId == user.Id)))
                .Returns(Task.FromResult(new RemoteMembership[] { remoteMembershipToDelete }.AsQueryable()));
            queries.Setup(x => x.Execute(It.Is<LocalMembershipByUser>(y => y.UserId == user.Id)))
                .Returns(Task.FromResult(null as LocalMembership));
            var validator = new ValidateDeleteRemoteMembershipCommand(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> targetError = x => x.PropertyName == command.PropertyName(y => y.Principal);
            result.Errors.Count(targetError).ShouldEqual(1);
            result.Errors.Single(targetError).ErrorMessage.ShouldEqual(Resources
                .Validation_RemoteMembershipByUser_IsOnlyLogin
                .Replace("{PropertyName}", User.Constraints.Label)
                .Replace("{PropertyValue}", command.Principal.Identity.Name)
                .Replace("{PasswordLabel}", LocalMembership.Constraints.Label.ToLower())
            );
        }

        [Fact]
        public void Handler_LoadsRemoteMembership_ByUserIdAndLoginInfo()
        {
            var command = new DeleteRemoteMembership
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.IdString()),
                    new Claim(ClaimTypes.Name, FakeData.String()),
                }, "authenticationType"), null),
                LoginProvider = FakeData.String(),
                ProviderKey = FakeData.String(),
            };
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            RemoteMembership remoteMembershipToDelete = new ProxiedRemoteMembership(
                command.LoginProvider, command.ProviderKey)
            {
                UserId = command.Principal.Identity.GetUserId<int>(),
            };
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()) { UserId = FakeData.Id() },
                remoteMembershipToDelete,
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String()) { UserId = FakeData.Id() },
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict)
                .SetupDataAsync(remoteMemberships.AsQueryable());
            entities.Setup(x => x.Get<RemoteMembership>()).Returns(dbSet.Object);
            entities.Setup(x => x.Delete(remoteMembershipToDelete));
            entities.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));
            var handler = new HandleDeleteRemoteMembershipCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<RemoteMembership>(), Times.Once);
        }

        [Fact]
        public void Handler_DoesNotDeleteOrSave_WhenRemoteMembershipNotFound_ByUserIdAndLoginInfo()
        {
            var command = new DeleteRemoteMembership
            {
                Principal = new GenericPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, FakeData.IdString()),
                    new Claim(ClaimTypes.Name, FakeData.String()),
                }, "authenticationType"), null),
                LoginProvider = FakeData.String(),
                ProviderKey = FakeData.String(),
            };
            var entities = new Mock<IWriteEntities>(MockBehavior.Strict);
            var remoteMemberships = new[]
            {
                new ProxiedRemoteMembership(command.LoginProvider, FakeData.String())
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(command.LoginProvider, command.ProviderKey)
                {
                    UserId = FakeData.Id()
                },
                new ProxiedRemoteMembership(FakeData.String(), FakeData.String())
                {
                    UserId = command.Principal.Identity.GetUserId<int>()
                },
            };
            var dbSet = new Mock<DbSet<RemoteMembership>>(MockBehavior.Strict)
                .SetupDataAsync(remoteMemberships.AsQueryable());
            entities.Setup(x => x.Get<RemoteMembership>()).Returns(dbSet.Object);
            var handler = new HandleDeleteRemoteMembershipCommand(entities.Object);

            handler.Handle(command).Wait();

            entities.Verify(x => x.Get<RemoteMembership>(), Times.Once);
            entities.Verify(x => x.Delete(It.IsAny<RemoteMembership>()), Times.Never);
            entities.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
