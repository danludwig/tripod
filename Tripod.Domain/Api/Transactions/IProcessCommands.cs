using System.Threading.Tasks;

namespace Tripod
{
    public interface IProcessCommands
    {
        Task Execute(IDefineCommand command);
    }
}
