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
            Queryable = queryable;
            Entities = entities;
        }

        internal IQueryable<TEntity> Queryable { get; set; }
        internal IQueryEntities Entities { get; private set; }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Queryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get { return Queryable.Expression; } }
        public Type ElementType { get { return Queryable.ElementType; } }
        public IQueryProvider Provider { get { return Queryable.Provider; } }
    }
}