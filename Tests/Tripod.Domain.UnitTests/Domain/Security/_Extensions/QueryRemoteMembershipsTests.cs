using System.Linq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class QueryRemoteMembershipsTests
    {
        #region ByUserId

        [Fact]
        public void ByUserId_Queryable_FiltersByUserId()
        {
            var userId = FakeData.Id();
            var data = new[]
            {
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
            };

            var results = data.AsQueryable().ByUserId(userId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        [Fact]
        public void ByUserId_Enumerable_FiltersByUserId()
        {
            var userId = FakeData.Id();
            var data = new[]
            {
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = userId, },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = FakeData.Id(canNotBe: userId), },
                new RemoteMembership { UserId = userId, },
            };

            var results = data.AsEnumerable().ByUserId(userId).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        #endregion
        #region ByUserName

        [Fact]
        public void ByUserName_Queryable_FiltersByUserName()
        {
            var userName = FakeData.String();
            var user = new User { Name = userName, };
            var data = new[]
            {
                new RemoteMembership { User = new User{ Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = new User{ Name = FakeData.String(), }, },
                new RemoteMembership { User = new User{ Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
            };

            var results = data.AsQueryable().ByUserName(userName).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        [Fact]
        public void ByUserName_Enumerable_FiltersByUserName()
        {
            var userName = FakeData.String();
            var user = new User { Name = userName, };
            var data = new[]
            {
                new RemoteMembership { User = new User{ Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = user, },
                new RemoteMembership { User = new User{ Name = FakeData.String(), }, },
                new RemoteMembership { User = new User{ Name = FakeData.String(), }, },
                new RemoteMembership { User = user, },
            };

            var results = data.AsEnumerable().ByUserName(userName).ToArray();
            results.ShouldNotBeNull();
            results.Length.ShouldEqual(3);
        }

        #endregion
    }
}
