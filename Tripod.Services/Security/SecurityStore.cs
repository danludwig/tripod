using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;

namespace Tripod.Services.Security
{
    public class SecurityStore : IQueryableUserStore<User, int>, IUserLoginStore<User, int>,
        IUserRoleStore<User, int>, IUserPasswordStore<User, int>, IUserClaimStore<User, int>, IUserSecurityStampStore<User, int>
        //IUserEmailStore<User, int>, IUserConfirmationStore<User, int>
    {
        #region Construction & Properties

        private readonly IWriteEntities _entities;

        public SecurityStore(IWriteEntities entities)
        {
            _entities = entities;
        }

        #endregion
        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }

        #endregion
        #region Helpers

        private Task<User> GetUserAggregateAsync(Expression<Func<User, bool>> filter)
        {
            return _entities.Get<User>()
                .EagerLoad(u => u.Permissions)
                .EagerLoad(u => u.EmailAddresses)
                .EagerLoad(u => u.LocalMembership)
                .EagerLoad(u => u.Claims)
                .EagerLoad(u => u.RemoteMemberships)
                .FirstOrDefaultAsync(filter);
        }

        //private static Task SaveChanges()
        //{
        //    //if (AutoSaveChanges)
        //    //{
        //    //    await _entities.SaveChangesAsync().ConfigureAwait(false);
        //    //}
        //    return Task.FromResult(0);
        //}

        #endregion
        #region IUserStore

        public Task<User> FindByIdAsync(int userId)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.Id.Equals(userId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public Task CreateAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");
            _entities.Create(user);
            //await SaveChanges().ConfigureAwait(false);
            return Task.FromResult(0);
        }

        public Task UpdateAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");
            _entities.Update(user);
            //await SaveChanges().ConfigureAwait(false);
            return Task.FromResult(0);
        }

        public Task DeleteAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");
            _entities.Delete(user);
            //await SaveChanges().ConfigureAwait(false);
            return Task.FromResult(0);
        }

        #endregion
        #region IQueryableUserStore

        IQueryable<User> IQueryableUserStore<User, int>.Users
        {
            get
            {
                ThrowIfDisposed();
                return _entities.Query<User>();
            }
        }

        #endregion
        #region IUserLoginStore

        public async Task<User> FindAsync(UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (login == null) throw new ArgumentNullException("login");
            return await GetUserAggregateAsync(u => u.RemoteMemberships.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey));
        }

        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");
            var instance = new RemoteMembership
            {
                UserId = user.Id,
                Id =
                {
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey
                },
            };
            _entities.Create(instance);
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");

            //var userLogin = await _entities.GetAsync<RemoteMembership>(login.LoginProvider, login.ProviderKey);
            var userLogin = user.RemoteMemberships.FirstOrDefault(x =>
                x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            if (userLogin != null)
                user.RemoteMemberships.Remove(userLogin);
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            var result = user.RemoteMemberships.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey))
                .ToArray() as IList<UserLoginInfo>;
            return Task.FromResult(result);
        }

        #endregion
        #region IUserRoleStore

        public async Task AddToRoleAsync(User user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException(Resources.Exception_Argument_CannotBeNullOrEmpty, "roleName");

            var permission = await _entities.Get<Permission>().SingleOrDefaultAsync(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (permission == null) throw new InvalidOperationException(string.Format(Resources.Exception_InvalidOperation_DoesNotExist,
                Permission.Constraints.Label, roleName));

            user.Permissions.Add(permission);
            permission.Users.Add(user);
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException(Resources.Exception_Argument_CannotBeNullOrEmpty, "roleName");

            var permission = user.Permissions.SingleOrDefault(x => x.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (permission == null) return Task.FromResult(0);

            user.Permissions.Remove(permission);
            permission.Users.Remove(user);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Permissions.Select(x => x.Name).ToArray() as IList<string>);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException(Resources.Exception_Argument_CannotBeNullOrEmpty, "roleName");

            return Task.FromResult(user.Permissions.Any(x => x.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)));
        }

        #endregion
        #region IUserPasswordStore

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");

            if (user.LocalMembership == null)
                user.LocalMembership = new LocalMembership();
            user.LocalMembership.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");

            return Task.FromResult(user.LocalMembership != null ? user.LocalMembership.PasswordHash : null);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.LocalMembership != null && user.LocalMembership.PasswordHash != null);
        }

        #endregion
        #region IUserClaimStore

        public Task<IList<Claim>> GetClaimsAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            var result = user.Claims
                .Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToArray() as IList<Claim>;
            return Task.FromResult(result);
        }

        public Task AddClaimAsync(User user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            var entity = new UserClaim
            {
                User = user,
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            };
            user.Claims.Add(entity);
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(User user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            var userClaims = user.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToArray();
            foreach (var entity in userClaims)
                user.Claims.Remove(entity);
            return Task.FromResult(0);
        }

        #endregion
        #region IUserSecurityStampStore

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.SecurityStamp);
        }

        #endregion
        //#region IUserEmailStore

        //public Task SetEmailAsync(User user, string email)
        //{
        //    ThrowIfDisposed();
        //    if (user == null) throw new ArgumentNullException("user");

        //    if (user.EmailAddresses.Any(x => x.AuthenticationType.Equals(email, StringComparison.OrdinalIgnoreCase)))
        //        return Task.FromResult(0);

        //    var entity = new EmailAddress
        //    {
        //        AuthenticationType = email,
        //        IsPrimary = !user.EmailAddresses.Any(x => x.IsPrimary),
        //    };
        //    user.EmailAddresses.Add(entity);
        //    return Task.FromResult(0);
        //}

        //public Task<string> GetEmailAsync(User user)
        //{
        //    ThrowIfDisposed();
        //    if (user == null) throw new ArgumentNullException("user");

        //    var entity = user.EmailAddresses.FirstOrDefault(x => x.IsPrimary && x.IsConfirmed)
        //        ?? user.EmailAddresses.FirstOrDefault(x => x.IsPrimary)
        //        ?? user.EmailAddresses.FirstOrDefault(x => x.IsConfirmed)
        //        ?? user.EmailAddresses.FirstOrDefault();
        //    return Task.FromResult(entity != null ? entity.AuthenticationType : null);
        //}

        //public Task<User> FindByEmailAsync(string email)
        //{
        //    ThrowIfDisposed();
        //    return GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.IsPrimary && x.IsConfirmed && x.AuthenticationType.Equals(email, StringComparison.OrdinalIgnoreCase)))
        //        ?? GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.IsPrimary && x.AuthenticationType.Equals(email, StringComparison.OrdinalIgnoreCase)))
        //        ?? GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.IsConfirmed && x.AuthenticationType.Equals(email, StringComparison.OrdinalIgnoreCase)))
        //        ?? GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.AuthenticationType.Equals(email, StringComparison.OrdinalIgnoreCase)));
        //}

        //#endregion
        //#region IUserConfirmationStore

        //public Task<bool> IsConfirmedAsync(User user)
        //{
        //    ThrowIfDisposed();
        //    if (user == null) throw new ArgumentNullException("user");
        //    return Task.FromResult(user.LocalMembership != null && user.LocalMembership.IsConfirmed);
        //}

        //public Task SetConfirmedAsync(User user, bool confirmed)
        //{
        //    ThrowIfDisposed();
        //    if (user == null) throw new ArgumentNullException("user");

        //    if (user.LocalMembership == null && confirmed)
        //        user.LocalMembership = new LocalMembership();
        //    if (user.LocalMembership != null)
        //        user.LocalMembership.IsConfirmed = confirmed;
        //    return Task.FromResult(0);
        //}

        //#endregion
    }
}
