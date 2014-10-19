using System;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class RandomSecretTests
    {
        [Fact]
        public void Ctor_MinMax_ThrowsArgumentOutOfRangeException_WhenMinLength_IsLessThanOne()
        {
            var minLength = new Random().Next(int.MinValue, 0);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new RandomSecret(minLength, new Random().Next(int.MaxValue)));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("minLength");
            exception.Message.ShouldStartWith(string.Format(
                Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Fact]
        public void Ctor_MinMax_ThrowsArgumentOutOfRangeException_WhenMaxLength_IsLessThanMinLength()
        {
            var minLength = new Random().Next(0, int.MaxValue);
            var maxLength = minLength - 1;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new RandomSecret(minLength, maxLength));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("maxLength");
            exception.Message.ShouldStartWith(string.Format(
                Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, "minLength"));
        }

        [Fact]
        public void Ctor_MinMax_SetsMinAndMaxLengthProperties_WhenArgumentsAreInRange()
        {
            var minLength = new Random().Next(1, int.MaxValue / 2);
            var maxLength = minLength + new Random().Next(0, 2);
            var query = new RandomSecret(minLength, maxLength);
            query.MinLength.ShouldEqual(minLength);
            query.MaxLength.ShouldEqual(maxLength);
        }

        [Fact]
        public void Ctor_Exact_ThrowsArgumentOutOfRangeException_WhenExactLength_IsLessThanOne()
        {
            var exactLength = new Random().Next(int.MinValue, 0);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new RandomSecret(exactLength));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("exactLength");
            exception.Message.ShouldStartWith(string.Format(
                Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Fact]
        public void Ctor_Exact_SetsBothMinAndMaxLengthProperties_WhenArgumentIsInRange()
        {
            var exactLength = new Random().Next(1, int.MaxValue);
            var query = new RandomSecret(exactLength);
            query.MinLength.ShouldEqual(exactLength);
            query.MaxLength.ShouldEqual(exactLength);
        }

        [Fact]
        public void Handle_ReturnsValue_FromSecretCreator()
        {
            var minLength = new Random().Next(1, 50);
            var maxLength = new Random().Next(minLength, 100);
            var secret = Guid.NewGuid().ToString();
            while (secret.Length < minLength)
                secret += Guid.NewGuid().ToString();
            while (secret.Length > maxLength)
                secret = secret.Substring(0, secret.Length - 2);
            var secretCreator = new Mock<ICreateSecrets>(MockBehavior.Strict);
            secretCreator.Setup(x => x.CreateSecret(minLength, maxLength))
                .Returns(secret);
            var handler = new HandleRandomSecretQuery(secretCreator.Object);
            var query = new RandomSecret(minLength, maxLength);

            string result = handler.Handle(query);

            result.ShouldEqual(secret);
            secretCreator.Verify(x => x.CreateSecret(minLength, maxLength), Times.Once);
        }
    }
}
