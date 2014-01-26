using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class UserByNameOrConfirmedEmail : BaseEntityQuery<User>, IDefineQuery<Task<User>>
    {
        public UserByNameOrConfirmedEmail(string nameOrEmail)
        {
            NameOrEmail = nameOrEmail;
        }

        public string NameOrEmail { get; private set; }
    }

    public class HandleUserByNameOrConfirmedEmailQuery : IHandleQuery<UserByNameOrConfirmedEmail, Task<User>>
    {
        private readonly IProcessQueries _queries;

        public HandleUserByNameOrConfirmedEmailQuery(IProcessQueries queries)
        {
            _queries = queries;
        }

        public Task<User> Handle(UserByNameOrConfirmedEmail query)
        {
            var user = _queries.Execute(new UserBy(query.NameOrEmail)).Result;
            if (user != null) return Task.FromResult(user);

            var email = _queries.Execute(new EmailAddressBy(query.NameOrEmail)
            {
                IsConfirmed = true,
                EagerLoad = new Expression<Func<EmailAddress, object>>[]
                {
                    x => x.Owner,
                },
            }).Result;

            user = email != null ? email.Owner : null;
            return Task.FromResult(user);
        }
    }
}
