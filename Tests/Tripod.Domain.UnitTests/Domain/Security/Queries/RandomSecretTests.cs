using System;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class RandomSecretTests
    {
        [Fact]
        public void Query_Ctor_MinMax_ThrowsArgumentOutOfRangeException_WhenMinLength_IsLessThanOne()
        {
            var minLength = FakeData.Int(int.MinValue, 0);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new RandomSecret(minLength, FakeData.Int(minLength, int.MaxValue)));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("minLength");
            exception.Message.ShouldStartWith(string.Format(
                Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Fact]
        public void Query_Ctor_MinMax_ThrowsArgumentOutOfRangeException_WhenMaxLength_IsLessThanMinLength()
        {
            var minLength = FakeData.Int(0, int.MaxValue);
            var maxLength = minLength - 1;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new RandomSecret(minLength, maxLength));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("maxLength");
            exception.Message.ShouldStartWith(string.Format(
                Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, "minLength"));
        }

        [Fact]
        public void Query_Ctor_MinMax_SetsMinAndMaxLengthProperties_WhenArgumentsAreInRange()
        {
            var minLength = FakeData.Int(1, int.MaxValue / 2);
            var maxLength = FakeData.Int(minLength, int.MaxValue);
            var query = new RandomSecret(minLength, maxLength);
            query.MinLength.ShouldEqual(minLength);
            query.MaxLength.ShouldEqual(maxLength);
        }

        [Fact]
        public void Query_Ctor_Exact_ThrowsArgumentOutOfRangeException_WhenExactLength_IsLessThanOne()
        {
            var exactLength = FakeData.Int(int.MinValue, 0);
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new RandomSecret(exactLength));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("exactLength");
            exception.Message.ShouldStartWith(string.Format(
                Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
        }

        [Fact]
        public void Query_Ctor_Exact_SetsBothMinAndMaxLengthProperties_WhenArgumentIsInRange()
        {
            var exactLength = FakeData.Id();
            var query = new RandomSecret(exactLength);
            query.MinLength.ShouldEqual(exactLength);
            query.MaxLength.ShouldEqual(exactLength);
        }

        [Fact]
        public void Handler_ReturnsValue_FromSecretCreator()
        {
            var minLength = FakeData.Int(1, 50);
            var maxLength = FakeData.Int(minLength, 100);
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
