using System;

namespace Tripod
{
    public static class ReflectionExtensions
    {
        public static bool IsGenericallyAssignableFrom(this Type openGeneric, Type closedGeneric)
        {
            var interfaceTypes = closedGeneric.GetInterfaces();

            foreach (var interfaceType in interfaceTypes)
                if (interfaceType.IsGenericType)
                    if (interfaceType.GetGenericTypeDefinition() == openGeneric) return true;

            var baseType = closedGeneric.BaseType;
            if (baseType == null) return false;

            return baseType.IsGenericType &&
                (baseType.GetGenericTypeDefinition() == openGeneric ||
                openGeneric.IsGenericallyAssignableFrom(baseType));
        }
    }
}
