using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tripod
{
    public abstract class BaseEntityQuery<TEntity> where TEntity : Entity
    {
        public IEnumerable<Expression<Func<TEntity, object>>> EagerLoad { get; set; }
    }
}
