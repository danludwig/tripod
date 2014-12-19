namespace Tripod
{
    public abstract class BaseEntityCommand : IDefineEvent
    {
        protected BaseEntityCommand()
        {
            Commit = true;
        }

        internal bool Commit { get; set; }
    }
}
