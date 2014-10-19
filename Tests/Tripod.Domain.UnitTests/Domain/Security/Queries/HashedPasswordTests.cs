using System;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class HashedPasswordTests
    {
        [Fact]
        public void Query_Ctor_SetsPasswordProperty()
        {
            var password = Guid.NewGuid().ToString();
            var query = new HashedPassword(password);
            query.Password.ShouldEqual(password);
        }

        [Fact]
        public void Handler_ThrowsArgumentNullException_WhenPasswordIsNull()
        {
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var handler = new HandleHashedPasswordQuery(userManager);
            var query = new HashedPassword(null);

            var exception = Assert.Throws<ArgumentNullException>(() => handler.Handle(query).Result);

            exception.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("asdfasdf")]
        [InlineData("password")]
        [InlineData("ad51e099-6e88-46fb-9aaa-ff2488724dbe")]
        [InlineData("")]
        [InlineData("\t\t\t")]
        public void Handler_ReturnsResult_FromUserManager_PasswordHasher(string password)
        {
            var userStore = new Mock<IUserStore<User, int>>(MockBehavior.Strict);
            var userManager = new UserManager<User, int>(userStore.Object);
            var handler = new HandleHashedPasswordQuery(userManager);
            var query = new HashedPassword(password);

            string result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
        }
    }
}
