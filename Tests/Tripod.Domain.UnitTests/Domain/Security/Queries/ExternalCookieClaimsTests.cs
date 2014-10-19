using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class ExternalCookieClaimsTests
    {
        [Fact]
        public void Query_Ctor_NoArg_SetsAuthenticationType_ToDefaultExternalCookie()
        {
            var query = new ExternalCookieClaims();
            query.AuthenticationType.ShouldEqual(DefaultAuthenticationTypes.ExternalCookie);
        }

        [Fact]
        public void Query_Ctor_StringArg_SetsAuthenticationType_UsingArg()
        {
            string authenticationType = Guid.NewGuid().ToString();
            var query = new ExternalCookieClaims(authenticationType);
            query.AuthenticationType.ShouldEqual(authenticationType);
        }

        [Fact]
        public void Handler_ReturnsClaims_FromAuthenticator_GetRemoteMembershipClaims()
        {
            const string authenticationType = DefaultAuthenticationTypes.ExternalBearer;
            var data = new[]
            {
                new Claim(ClaimTypes.Email, string.Format("{0}@domain.tld", Guid.NewGuid())),
                new Claim(ClaimTypes.NameIdentifier, new Random().Next(3, int.MaxValue)
                    .ToString(CultureInfo.InvariantCulture)), 
                new Claim(ClaimTypes.Gender, string.Empty), 
            };
            var authenticator = new Mock<IAuthenticate>(MockBehavior.Strict);
            authenticator.Setup(x => x.GetRemoteMembershipClaims(authenticationType))
                .Returns(Task.FromResult(data as IEnumerable<Claim>));
            var handler = new HandleExternalCookieClaimsQuery(authenticator.Object);
            var query = new ExternalCookieClaims(authenticationType);

            Claim[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(3);
            result.ShouldEqual(data);
            authenticator.Verify(x => x.GetRemoteMembershipClaims(authenticationType), Times.Once);
        }
    }
}
