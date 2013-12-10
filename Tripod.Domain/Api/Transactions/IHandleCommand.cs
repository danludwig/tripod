namespace Tripod
{
    public interface IHandleCommand<in TCommand> where TCommand : IDefineCommand
    {
        void Handle(TCommand command);
    }
}