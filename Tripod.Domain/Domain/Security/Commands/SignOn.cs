using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's remote membership.
    /// </summary>
    public class SignOn : IDefineCommand
    {
        public IPrincipal Principal { get; set; }
        public bool IsPersistent { get; [UsedImplicitly] set; }
        public User SignedOn { get; internal set; }
    }

    [UsedImplicitly]
    public class HandleSignOnCommand : IHandleCommand<SignOn>
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly IWriteEntities _entities;
        private readonly IAuthenticate _authenticator;

        public HandleSignOnCommand(IProcessQueries queries, IProcessCommands commands, IWriteEntities entities, IAuthenticate authenticator)
        {
            _queries = queries;
            _commands = commands;
            _entities = entities;
            _authenticator = authenticator;
        }

        public async Task Handle(SignOn command)
        {
            if (command.Principal.Identity.IsAuthenticated)
            {
                command.SignedOn = await _queries.Execute(new UserBy(command.Principal));
                return;
            }

            // try to find user by external login credentials
            var externalLogin = await _queries.Execute(new PrincipalRemoteMembershipTicket(command.Principal));
            if (externalLogin == null)
                return;

            var user = await _queries.Execute(new UserBy(externalLogin.Login));
            if (user != null)
            {
                await _authenticator.SignOn(user, command.IsPersistent);
                command.SignedOn = user;
                return;
            }

            // if they don't exist, check with email claim
            var emailClaim = await _queries.Execute(new ExternalCookieClaim(ClaimTypes.Email));
            if (emailClaim != null)
            {
                var emailAddress = await _queries.Execute(new EmailAddressBy(emailClaim)
                {
                    IsVerified =  true,
                    EagerLoad = new Expression<Func<EmailAddress, object>>[]
                    {
                        x => x.Owner,
                    },
                });
                if (emailAddress != null)
                {
                    // need to add the external login credentials
                    user = emailAddress.Owner;
                    _entities.Update(user); // make sure it is attached to the context
                    await _commands.Execute(new CreateRemoteMembership
                    {
                        Principal = command.Principal,
                        User = user,
                    });

                    await _authenticator.SignOn(user, command.IsPersistent);
                    command.SignedOn = user;
                }
            }
        }
    }
}
