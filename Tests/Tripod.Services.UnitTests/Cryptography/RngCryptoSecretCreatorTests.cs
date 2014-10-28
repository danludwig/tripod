using System;
using Should;
using Xunit;

namespace Tripod.Services.Cryptography
{
    public class RngCryptoSecretCreatorTests
    {
        [Fact]
        public void CreateSecret_MinMaxLength_ThrowsArgumentOutOfRangeException_WhenMinLengthIsLessThan1()
        {
            var minLength = FakeData.Int(-1000, 0);
            var maxLength = FakeData.Int(1, 502);
            ICreateSecrets secretCreator = new RngCryptoSecretCreator();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => secretCreator.CreateSecret(minLength, maxLength));

            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("minLength");
            exception.Message.ShouldStartWith(string.Format(Resources
                .Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Fact]
        public void CreateSecret_ExactLength_ThrowsArgumentOutOfRangeException_WhenLengthIsLessThan1()
        {
            var exactLength = FakeData.Int(-1000, 0);
            ICreateSecrets secretCreator = new RngCryptoSecretCreator();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => secretCreator.CreateSecret(exactLength));

            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("minLength");
            exception.Message.ShouldStartWith(string.Format(Resources
                .Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Fact]
        public void CreateSecret_MinMaxLength_ThrowsArgumentOutOfRangeException_WhenMaxLengthIsLessThanMinLength()
        {
            var minLength = FakeData.Int(103, 500);
            var maxLength = FakeData.Int(1, 102);
            ICreateSecrets secretCreator = new RngCryptoSecretCreator();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => secretCreator.CreateSecret(minLength, maxLength));

            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("maxLength");
            exception.Message.ShouldStartWith(string.Format(Resources
                .Exception_ArgumentOutOfRange_CannotBeLessThan, "minLength"));
        }

        [Fact]
        public void CreateSecret_MinMaxLength_CreatesRandomSecret()
        {
            var minLength = FakeData.Int(2, 101);
            var maxLength = FakeData.Int(minLength + 1, 502);
            ICreateSecrets secretCreator = new RngCryptoSecretCreator();

            var secret = secretCreator.CreateSecret(minLength, maxLength);

            secret.ShouldNotBeNull();
            secret.Length.ShouldBeGreaterThanOrEqualTo(minLength);
            secret.Length.ShouldBeLessThanOrEqualTo(maxLength);
        }

        [Fact]
        public void CreateSecret_ExactLength_CreatesRandomSecret()
        {
            var exactLength = FakeData.Int(2, 502);
            ICreateSecrets secretCreator = new RngCryptoSecretCreator();

            var secret = secretCreator.CreateSecret(exactLength);

            secret.ShouldNotBeNull();
            secret.Length.ShouldEqual(exactLength);
        }
    }
}
