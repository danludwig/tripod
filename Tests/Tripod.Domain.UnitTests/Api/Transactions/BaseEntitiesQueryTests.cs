using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Should;
using Xunit;

namespace Tripod
{
    public class BaseEntitiesQueryTests
    {
        [Fact]
        public void EagerLoad_GetSet()
        {
            var instance = new FakeEntitiesQuery
            {
                EagerLoad = new Expression<Func<FakeEntityWithNavigationProperty, object>>[]
                {
                    x => x.NavigationProperty,
                }
            };
            instance.ShouldNotBeNull();
            instance.EagerLoad.Count().ShouldEqual(1);
        }

        [Fact]
        public void OrderBy_GetSet()
        {
            var instance = new FakeEntitiesQuery
            {
                OrderBy = new Dictionary<Expression<Func<FakeEntityWithNavigationProperty, object>>, OrderByDirection>
                {
                    { x => x.NavigationProperty, OrderByDirection.Ascending }
                }
            };
            instance.ShouldNotBeNull();
            instance.OrderBy.Count().ShouldEqual(1);
        }
    }
}
