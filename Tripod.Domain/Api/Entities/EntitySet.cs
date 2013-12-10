using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tripod
{
    public class EntitySet<TEntity> : IQueryable<TEntity> where TEntity : Entity
    {
        public EntitySet(IQueryable<TEntity> queryable, IQueryEntities entities)
        {
            if (queryable == null) throw new ArgumentNullException("queryable");
            if (entities == null) throw new ArgumentNullException("entities");
            _queryable = queryable;
            Entities = entities;
        }

        private readonly IQueryable<TEntity> _queryable;
        internal IQueryEntities Entities { get; private set; }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get { return _queryable.Expression; } }
        public Type ElementType { get { return _queryable.ElementType; } }
        public IQueryProvider Provider { get { return _queryable.Provider; } }
    }
}