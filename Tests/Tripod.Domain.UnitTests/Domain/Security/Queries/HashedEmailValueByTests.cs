using System;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class HashedEmailValueByTests
    {
        [Fact]
        public void Query_Ctor_SetsEmailAddressProperty_UsingStringArg()
        {
            var emailAddress = FakeData.Email();
            var query = new HashedEmailValueBy(emailAddress);
            query.EmailAddress.ShouldEqual(emailAddress);
        }

        [Theory]
        [InlineData("adee8f8a-0557-47f7-ad77-a2070607639f@domain.tld", "3b1e397b61f746e2b247b50d2919182e")]
        [InlineData("ebb2b86f-ce04-439e-916a-e9722dfb61e8@domain.tld", "f444f1a0b2c5597a80d7bed06c7d6207")]
        [InlineData("008cf114-39dd-4d0e-93d5-4035d3b4f8a9@domain.tld", "f3aeb2c4824c408ac22040b5ad121582")]
        [InlineData(null, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData("", "d41d8cd98f00b204e9800998ecf8427e")]
        public void Handler_HashesEmail_GravatarStyle(string emailAddress, string expectedHash)
        {
            var handler = new HandleHashedEmailValueByQuery();
            var query = new HashedEmailValueBy(emailAddress);

            string result = handler.Handle(query).Result;

            result.ShouldEqual(expectedHash);
        }
    }
}
