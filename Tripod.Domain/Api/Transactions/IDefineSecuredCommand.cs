using System.Security.Principal;

namespace Tripod
{
    public interface IDefineSecuredCommand : IDefineCommand
    {
        IPrincipal Principal { get; set; }
    }
}