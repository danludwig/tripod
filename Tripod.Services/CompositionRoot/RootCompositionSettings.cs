using System.Reflection;

namespace Tripod.Services
{
    public class RootCompositionSettings
    {
        public bool IsGreenfield { get; set; }
        public Assembly[] FluentValidatorAssemblies { get; set; }
        public Assembly[] QueryHandlerAssemblies { get; set; }
        public Assembly[] CommandHandlerAssemblies { get; set; }
        public Assembly[] EventHandlerAssemblies { get; set; }
    }
}
