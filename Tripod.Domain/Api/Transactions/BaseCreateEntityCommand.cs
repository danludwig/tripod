namespace Tripod
{
    public abstract class BaseCreateEntityCommand<TEntity> : BaseEntityCommand where TEntity : Entity
    {
        public TEntity CreatedEntity { get; internal set; }
    }
}