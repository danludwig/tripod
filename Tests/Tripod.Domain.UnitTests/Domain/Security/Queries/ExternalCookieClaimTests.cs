using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class ExternalCookieClaimTests
    {
        [Fact]
        public void Query_Ctor_SingleArg_SetsClaimType()
        {
            var claimType = FakeData.String();
            var query = new ExternalCookieClaim(claimType);
            query.ClaimType.ShouldEqual(claimType);
        }

        [Fact]
        public void Query_Ctor_SingleArg_SetsAuthenticationType_ToDefaultExternalCookie()
        {
            var claimType = FakeData.String();
            var query = new ExternalCookieClaim(claimType);
            query.AuthenticationType.ShouldEqual(DefaultAuthenticationTypes.ExternalCookie);
        }

        [Fact]
        public void Query_Ctor_SecondArg_SetsAuthenticationType()
        {
            var claimType = FakeData.String();
            var authenticationType = FakeData.String();
            var query = new ExternalCookieClaim(claimType, authenticationType);
            query.AuthenticationType.ShouldEqual(authenticationType);
        }

        [Fact]
        public void Handler_ReturnsNullClaim_WhenDoesNotExist()
        {
            const string authenticationType = DefaultAuthenticationTypes.ApplicationCookie;
            var data = new[]
            {
                new Claim(ClaimTypes.Email, FakeData.Email()),
                new Claim(ClaimTypes.NameIdentifier, FakeData.Id()
                    .ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Gender, string.Empty),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<ExternalCookieClaims, bool>> expectedQuery = x => x.AuthenticationType == authenticationType;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(data as IEnumerable<Claim>));
            var handler = new HandleExternalCookieClaimQuery(queries.Object);
            var query = new ExternalCookieClaim(ClaimTypes.GivenName, authenticationType);

            Claim result = handler.Handle(query).Result;

            result.ShouldBeNull();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNonNullClaim_WhenExists()
        {
            const string authenticationType = DefaultAuthenticationTypes.ApplicationCookie;
            const string claimType = ClaimTypes.GivenName;
            string claimValue = FakeData.String();
            var data = new[]
            {
                new Claim(ClaimTypes.Email, FakeData.Email()),
                new Claim(ClaimTypes.NameIdentifier, FakeData.Id()
                    .ToString(CultureInfo.InvariantCulture)),
                new Claim(claimType, claimValue),
            };
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            Expression<Func<ExternalCookieClaims, bool>> expectedQuery = x => x.AuthenticationType == authenticationType;
            queries.Setup(x => x.Execute(It.Is(expectedQuery)))
                .Returns(Task.FromResult(data as IEnumerable<Claim>));
            var handler = new HandleExternalCookieClaimQuery(queries.Object);
            var query = new ExternalCookieClaim(ClaimTypes.GivenName, authenticationType);

            Claim result = handler.Handle(query).Result;

            Assert.NotNull(result);
            result.Type.ShouldEqual(claimType);
            result.Value.ShouldEqual(claimValue);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
        }
    }
}
