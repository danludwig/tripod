using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Moq;
using Should;
using Tripod.Domain.Security;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Ioc.Security
{
    public class OwinAuthenticatorTests
    {
        [Theory, InlineData(true), InlineData(false)]
        public void SignOn_ThrowsIfNoOwin(bool useBigFatPhony)
        {
            var authenticationManager = useBigFatPhony ? new BigFatPhonyAuthenticationManager() : null;
            var instance = new OwinAuthenticator(authenticationManager, null);
            var exception = Assert.Throws<InvalidOperationException>(() => instance.SignOn(null).GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("no owin");
        }

        [Fact]
        public void SignOn_InvokesSignOut_OnIAuthenticationManager()
        {
            var user = new User { Name = Guid.NewGuid().ToString() };
            var authenticationManager = new Mock<IAuthenticationManager>(MockBehavior.Strict);
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var instance = new OwinAuthenticator(authenticationManager.Object, userManager);
            authenticationManager.Setup(x => x.SignOut(DefaultAuthenticationTypes.ExternalCookie));
            authenticationManager.Setup(x => x.SignIn(It.IsAny<AuthenticationProperties>(), It.IsAny<ClaimsIdentity>()));

            instance.SignOn(user).Wait();

            authenticationManager.Verify(x => x.SignOut(DefaultAuthenticationTypes.ExternalCookie), Times.Once);
        }

        [Theory, InlineData(true), InlineData(false)]
        public void SignOn_InvokesSignIn_OnIAuthenticationManager_PassingCookiePersistenceData(bool isPersistent)
        {
            var user = new User { Name = Guid.NewGuid().ToString() };
            var authenticationManager = new Mock<IAuthenticationManager>(MockBehavior.Strict);
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var instance = new OwinAuthenticator(authenticationManager.Object, userManager);
            authenticationManager.Setup(x => x.SignOut(It.IsAny<string>()));
            Expression<Func<ClaimsIdentity, bool>> expectedIdentity = x => x.Name.Equals(user.Name);
            Expression<Func<AuthenticationProperties, bool>> expectedProperties = x => x.IsPersistent == isPersistent;
            authenticationManager.Setup(x => x.SignIn(It.Is(expectedProperties), It.Is(expectedIdentity)));

            instance.SignOn(user, isPersistent).Wait();

            authenticationManager.Verify(x => x.SignIn(It.Is(expectedProperties), It.Is(expectedIdentity)), Times.Once);
        }

        [Theory, InlineData(true), InlineData(false)]
        public void SignOff_ThrowsIfNoOwin(bool useBigFatPhony)
        {
            var authenticationManager = useBigFatPhony ? new BigFatPhonyAuthenticationManager() : null;
            var instance = new OwinAuthenticator(authenticationManager, null);
            var exception = Assert.Throws<InvalidOperationException>(() => instance.SignOff().GetAwaiter().GetResult());
            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("no owin");
        }

        [Fact]
        public void SignOff_InvokesSignOut_OnIAuthenticationManager()
        {
            var authenticationManager = new Mock<IAuthenticationManager>(MockBehavior.Strict);
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var instance = new OwinAuthenticator(authenticationManager.Object, userManager);
            authenticationManager.Setup(x => x.SignOut());

            instance.SignOff().Wait();

            authenticationManager.Verify(x => x.SignOut(), Times.Once);
        }
    }
}
