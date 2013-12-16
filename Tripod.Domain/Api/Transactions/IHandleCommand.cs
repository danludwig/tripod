using System.Threading.Tasks;

namespace Tripod
{
    public interface IHandleCommand<in TCommand> where TCommand : IDefineCommand
    {
        Task Handle(TCommand command);
    }
}