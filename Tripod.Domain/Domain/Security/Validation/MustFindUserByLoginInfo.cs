using System;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class MustFindUserByLoginInfo : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserByLoginInfo(IProcessQueries queries)
            : base(() => Resources.Validation_NoUserByLoginInfo)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userLoginInfo = (UserLoginInfo)context.PropertyValue;
            var entity = _queries.Execute(new UserBy(userLoginInfo)).Result;

            // assert that user does exist
            return entity != null;
        }
    }

    public static class MustFindUserByLoginInfoExtensions
    {
        public static IRuleBuilderOptions<T, UserLoginInfo> MustFindUserByLoginInfo<T>
            (this IRuleBuilder<T, UserLoginInfo> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserByLoginInfo(queries));
        }
    }
}
