using System.Security.Principal;

namespace Tripod
{
    public interface IDefineSecuredCommand : IDefineCommand
    {
        IPrincipal Principal { [UsedImplicitly] get; set; }
    }
}