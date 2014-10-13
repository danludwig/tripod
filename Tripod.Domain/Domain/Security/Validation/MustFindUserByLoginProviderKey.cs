using System;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class MustFindUserByLoginProviderKey<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _loginProvider;

        internal MustFindUserByLoginProviderKey(IProcessQueries queries, Func<T, string> loginProvider)
            : base(() => Resources.Validation_NoUserByLoginProviderKey)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (loginProvider == null) throw new ArgumentNullException("loginProvider");
            _queries = queries;
            _loginProvider = loginProvider;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var loginProvider = _loginProvider((T)context.Instance);
            var providerKey = (string)context.PropertyValue;
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey);
            var entity = _queries.Execute(new UserBy(userLoginInfo)).Result;

            // assert that user does exist
            return entity != null;
        }
    }

    [UsedImplicitly]
    public static class MustFindUserByLoginProviderKeyExtensions
    {
        public static IRuleBuilderOptions<T, string> MustFindUserByLoginProviderKey<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> loginProvider)
        {
            return ruleBuilder.SetValidator(new MustFindUserByLoginProviderKey<T>(queries, loginProvider));
        }
    }
}
