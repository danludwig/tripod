using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Moq;
using Should;
using SimpleInjector;
using Tripod.Domain.Security;
using Tripod.Ioc.EntityFramework;
using Xunit;

namespace Tripod.Ioc.Security
{
    public class CompositionRootTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void RegistersSecurityStore_Transiently()
        {
            var registration = Container.GetRegistration(typeof(SecurityStore));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<SecurityStore>();
                instance.ShouldNotBeNull();
            }
        }

        [Fact]
        public void RegistersIUserStore_Transiently_UsingSecurityStore()
        {
            var registration = Container.GetRegistration(typeof(IUserStore<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IUserStore<User, int>>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<SecurityStore>();
            }
        }

        [Fact]
        public void RegistersIUserLoginStore_Transiently_UsingSecurityStore()
        {
            var registration = Container.GetRegistration(typeof(IUserLoginStore<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IUserLoginStore<User, int>>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<SecurityStore>();
            }
        }

        [Fact]
        public void RegistersIUserRoleStore_Transiently_UsingSecurityStore()
        {
            var registration = Container.GetRegistration(typeof(IUserRoleStore<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IUserRoleStore<User, int>>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<SecurityStore>();
            }
        }

        [Fact]
        public void RegistersIUserPasswordStore_Transiently_UsingSecurityStore()
        {
            var registration = Container.GetRegistration(typeof(IUserPasswordStore<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IUserPasswordStore<User, int>>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<SecurityStore>();
            }
        }

        [Fact]
        public void RegistersIUserClaimStore_Transiently_UsingSecurityStore()
        {
            var registration = Container.GetRegistration(typeof(IUserClaimStore<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IUserClaimStore<User, int>>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<SecurityStore>();
            }
        }

        [Fact]
        public void RegistersIUserSecurityStampStore_Transiently_UsingSecurityStore()
        {
            var registration = Container.GetRegistration(typeof(IUserSecurityStampStore<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IUserSecurityStampStore<User, int>>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<SecurityStore>();
            }
        }

        [Fact]
        public void RegistersIQueryableUserStore_Transiently_UsingSecurityStore()
        {
            var registration = Container.GetRegistration(typeof(IQueryableUserStore<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IQueryableUserStore<User, int>>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<SecurityStore>();
            }
        }

        [Fact]
        public void RegistersUserManager_Transiently()
        {
            var registration = Container.GetRegistration(typeof(UserManager<User, int>));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<UserManager<User, int>>();
                instance.ShouldNotBeNull();
            }
        }

        [Fact]
        public void RegistersUserManager_UsingOwinTokenProviders_WhenCurrentHttpContext_HasOwinEnvironment()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://localhost", null), new HttpResponse(null));
            var owinEnvironment = new Dictionary<string, object>();
            var userStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User, int>(userStore.Object);
            var userTokenProvider = new Mock<ITokenProvider>(MockBehavior.Strict);
            userManager.UserConfirmationTokens = userTokenProvider.Object;
            var passwordTokenProvider = new Mock<ITokenProvider>(MockBehavior.Strict);
            userManager.PasswordResetTokens = passwordTokenProvider.Object;
            owinEnvironment["AspNet.Identity.Owin:" + userManager.GetType().AssemblyQualifiedName] = userManager;
            HttpContext.Current.Items.Add("owin.Environment", owinEnvironment);
            var container = new Container();
            container.RegisterEntityFramework();
            container.RegisterSecurity();
            container.Verify();

            var instance = container.GetInstance<UserManager<User, int>>();
            instance.ShouldNotBeNull();
            instance.UserConfirmationTokens.ShouldNotBeNull();
            instance.UserConfirmationTokens.ShouldEqual(userTokenProvider.Object);
            instance.PasswordResetTokens.ShouldNotBeNull();
            instance.PasswordResetTokens.ShouldEqual(passwordTokenProvider.Object);
        }

        [Fact]
        public void RegistersIAuthenticationManager_UsingBigFatPhony_WhenCurrentHttpContext_IsNull()
        {
            //var registration = Container.GetRegistration(typeof (IAuthenticationManager));
            //registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            //var instance = Container.GetInstance<IAuthenticationManager>();
            //instance.ShouldNotBeNull();
            //instance.ShouldBeType<BigFatPhonyAuthenticationManager>();
            HttpContext.Current = null;
            var container = new Container();
            container.RegisterEntityFramework();
            container.RegisterSecurity();
            container.Verify();

            var registration = container.GetRegistration(typeof(IAuthenticationManager));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            var instance = container.GetInstance<IAuthenticationManager>();
            instance.ShouldNotBeNull();
            instance.ShouldBeType<BigFatPhonyAuthenticationManager>();
        }

        [Fact]
        public void RegistersIAuthenticationManager_UsingBigFatPhony_WhenCurrentHttpContext_HasNoOwinEnvironment()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://localhost", null), new HttpResponse(null));
            var container = new Container();
            container.RegisterEntityFramework();
            container.RegisterSecurity();
            container.Verify();

            var registration = container.GetRegistration(typeof(IAuthenticationManager));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            var instance = container.GetInstance<IAuthenticationManager>();
            instance.ShouldNotBeNull();
            instance.ShouldBeType<BigFatPhonyAuthenticationManager>();
        }

        [Fact]
        public void RegistersIAuthenticationManager_UsingOwin_WhenCurrentHttpContext_HasOwinEnvironment()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://localhost", null), new HttpResponse(null));
            var owinEnvironment = new Dictionary<string, object>();
            var userStore = new Mock<IUserStore<User, int>>();
            var userManager = new UserManager<User, int>(userStore.Object);
            owinEnvironment["AspNet.Identity.Owin:" + userManager.GetType().AssemblyQualifiedName] = userManager;
            HttpContext.Current.Items.Add("owin.Environment", owinEnvironment);
            var container = new Container();
            container.RegisterEntityFramework();
            container.RegisterSecurity();
            container.Verify();

            var registration = container.GetRegistration(typeof(IAuthenticationManager));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            var instance = container.GetInstance<IAuthenticationManager>();
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void RegistersIAuthenticate_UsingOwinAuthenticator()
        {
            var registration = Container.GetRegistration(typeof(IAuthenticate));
            registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            using (Container.BeginLifetimeScope())
            {
                var instance = Container.GetInstance<IAuthenticate>();
                instance.ShouldNotBeNull();
                instance.ShouldBeType<OwinAuthenticator>();
            }
        }
    }
}
