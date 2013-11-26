using System;

namespace Tripod
{
    /// <summary>
    /// A single unit of relational data that can be identified by a primary key.
    /// </summary>
    public abstract class Entity : IEquatable<Entity>
    {
        /// <summary>
        /// Determine whether this entity is equal to another entity.
        /// </summary>
        /// <param name="other">The entity to compare to this entity when determining equality.</param>
        /// <returns>True if both entities are not null or transient and share the same primary key value(s).
        /// Otherwise, false.</returns>
        public abstract bool Equals(Entity other);
    }
}