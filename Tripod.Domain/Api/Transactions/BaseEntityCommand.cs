namespace Tripod
{
    public abstract class BaseEntityCommand
    {
        protected BaseEntityCommand()
        {
            Commit = true;
        }

        internal bool Commit { get; set; }
    }
}
