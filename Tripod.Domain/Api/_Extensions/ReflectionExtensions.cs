using System;
using System.Linq.Expressions;
using System.Text;

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


        public static string PropertyName<T>(this T owner, Expression<Func<T, object>> expression, bool fullName = true) where T : class
        {
            if (owner == null) throw new ArgumentNullException("owner");

            var memberExpression = expression.Body as MemberExpression;
            var unaryExpression = expression.Body as UnaryExpression;

            if (memberExpression == null && unaryExpression != null)
                memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression != null)
            {
                var builder = new StringBuilder(memberExpression.Member.Name);
                if (fullName)
                {
                    memberExpression = memberExpression.Expression as MemberExpression;
                    while (memberExpression != null)
                    {
                        builder.Insert(0, '.');
                        builder.Insert(0, memberExpression.Member.Name);
                        memberExpression = memberExpression.Expression as MemberExpression;
                    }
                }
                return builder.ToString();
            }

            throw new NotSupportedException(string.Format(
                "Unable to determine the property name for the lambda '{0}' on '{1}'.",
                    expression, owner.GetType().Name));
        }
    }
}
