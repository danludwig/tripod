using System;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class EmailMessageTests
    {
        [Fact]
        public void Ctor_InitializesNothing()
        {
            var entity = new EmailMessage();
            entity.Id.ShouldEqual(0);
            entity.EmailAddressId.ShouldEqual(0);
            entity.EmailAddress.ShouldBeNull();
            entity.From.ShouldBeNull();
            entity.Subject.ShouldBeNull();
            entity.Body.ShouldBeNull();
            entity.IsBodyHtml.ShouldBeFalse();
            entity.SendOnUtc.ShouldEqual(DateTime.MinValue);
            entity.SentOnUtc.ShouldBeNull();
            entity.CancelledOnUtc.ShouldBeNull();
            entity.LastSendError.ShouldBeNull();
        }
    }
}