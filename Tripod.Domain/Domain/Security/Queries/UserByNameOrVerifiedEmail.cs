using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Find a User by either Name or verified EmailAddress using a single input.
    /// </summary>
    public class UserByNameOrVerifiedEmail : BaseEntityQuery<User>, IDefineQuery<Task<User>>
    {
        /// <summary>
        /// Find a User by either Name or verified EmailAddress using a single input.
        /// </summary>
        /// <param name="nameOrEmail">Text that can be used to find a User either by Name
        ///     or a verified EmailAddress.</param>
        public UserByNameOrVerifiedEmail(string nameOrEmail) { NameOrEmail = nameOrEmail; }

        public string NameOrEmail { get; private set; }
    }

    [UsedImplicitly]
    public class HandleUserByNameOrVerifiedEmailQuery : IHandleQuery<UserByNameOrVerifiedEmail, Task<User>>
    {
        private readonly IProcessQueries _queries;

        public HandleUserByNameOrVerifiedEmailQuery(IProcessQueries queries)
        {
            _queries = queries;
        }

        public Task<User> Handle(UserByNameOrVerifiedEmail query)
        {
            var user = _queries.Execute(new UserBy(query.NameOrEmail)).Result;
            if (user != null) return Task.FromResult(user);

            var email = _queries.Execute(new EmailAddressBy(query.NameOrEmail)
            {
                IsVerified = true,
                EagerLoad = new Expression<Func<EmailAddress, object>>[]
                {
                    x => x.User,
                },
            }).Result;

            user = email != null && email.IsVerified ? email.User : null;
            return Task.FromResult(user);
        }
    }
}
