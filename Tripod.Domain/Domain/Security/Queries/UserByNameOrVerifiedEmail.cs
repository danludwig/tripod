using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class UserByNameOrVerifiedEmail : BaseEntityQuery<User>, IDefineQuery<Task<User>>
    {
        public UserByNameOrVerifiedEmail(string nameOrEmail)
        {
            NameOrEmail = nameOrEmail;
        }

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

            user = email != null ? email.User : null;
            return Task.FromResult(user);
        }
    }
}
