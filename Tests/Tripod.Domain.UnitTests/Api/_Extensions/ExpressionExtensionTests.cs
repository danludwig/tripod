using System;
using System.Linq;
using System.Linq.Expressions;
using Should;
using Xunit;

namespace Tripod
{
    public class ExpressionExtensionTests
    {
        [Fact]
        public void Not_NegatesExpression()
        {
            var data = new[]
            {
                new FakeEntityWithIntId(0),
                new FakeEntityWithIntId(1),
                new FakeEntityWithIntId(2),
                new FakeEntityWithIntId(3),
                new FakeEntityWithIntId(4),
            };
            Expression<Func<FakeEntityWithIntId, bool>> idIsEven = x => x.Id % 2 == 0;
            var evens = data.AsQueryable().Where(idIsEven);

            var odds = data.AsQueryable().Where(idIsEven.Not());

            evens.Count().ShouldEqual(3);
            evens.Select(x => x.Id).All(x => new[] { 0, 2, 4 }.Contains(x)).ShouldBeTrue();
            odds.Count().ShouldEqual(2);
            odds.Select(x => x.Id).All(x => new[] { 1, 3 }.Contains(x)).ShouldBeTrue();
        }
    }
}
