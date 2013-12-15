using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;
using SecurityClaim = System.Security.Claims.Claim;

namespace Tripod.Ioc.Security
{
    public class SecurityStore : IQueryableUserStore<User, int>, IUserLoginStore<User, int>,
        IUserRoleStore<User, int>, IUserPasswordStore<User, int>, IUserClaimStore<User, int>,
        IUserEmailStore<User, int>, IUserConfirmationStore<User, int>, IUserSecurityStampStore<User, int>
    {
        #region Construction & Properties

        private readonly ICommandEntities _entities;

        public SecurityStore(ICommandEntities entities)
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
                .Include(u => u.Permissions)
                .Include(u => u.EmailAddresses)
                .Include(u => u.LocalMembership)
                .Include(u => u.Claims)
                .Include(u => u.Logins)
                .FirstOrDefaultAsync(filter);
        }

        private static Task SaveChanges()
        {
            //if (AutoSaveChanges)
            //{
            //    await _entities.SaveChangesAsync().ConfigureAwait(false);
            //}
            return Task.FromResult(0);
        }

        #endregion
        #region IQueryableUserStore

        IQueryable<User> IQueryableUserStore<User, int>.Users
        {
            get
            {
                return _entities.Query<User>();
            }
        }

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

        public async Task CreateAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");
            _entities.Create(user);
            await SaveChanges().ConfigureAwait(false);
        }

        public async Task DeleteAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");
            _entities.Delete(user);
            await SaveChanges().ConfigureAwait(false);
        }

        public async Task UpdateAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");
            _entities.Update(user);
            await SaveChanges().ConfigureAwait(false);
        }

        #endregion
        #region IUserLoginStore

        public async Task<User> FindAsync(UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (login == null) throw new ArgumentNullException("login");
            return await GetUserAggregateAsync(u => u.Logins.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey));
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

        public async Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");

            var userLogin = await _entities.GetAsync<RemoteMembership>(login.LoginProvider, login.ProviderKey);
            if (userLogin != null)
                _entities.Delete(userLogin);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            var result = user.Logins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey))
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
                throw new ArgumentException("Value cannot be null or empty.", "roleName");

            var permission = await _entities.Get<Permission>().SingleOrDefaultAsync(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (permission == null) throw new InvalidOperationException(string.Format("Role {0} does not exist", roleName));

            user.Permissions.Add(permission);
            permission.Users.Add(user);
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Value cannot be null or empty.", "roleName");

            var permission = user.Permissions.SingleOrDefault(x => x.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (permission != null)
            {
                user.Permissions.Remove(permission);
                permission.Users.Remove(user);
            }
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
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Value cannot be null or empty.", "roleName");

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

        public Task<IList<SecurityClaim>> GetClaimsAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            var result = user.Claims
                .Select(x => new SecurityClaim(x.ClaimType, x.ClaimValue)).ToArray() as IList<SecurityClaim>;
            return Task.FromResult(result);
        }

        public Task AddClaimAsync(User user, SecurityClaim claim)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            var entity = new Claim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            };
            user.Claims.Add(entity);
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(User user, SecurityClaim claim)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            foreach (var entity in user.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToArray())
                _entities.Delete(entity);
            return Task.FromResult(0);
        }

        #endregion
        #region IUserEmailStore

        public Task SetEmailAsync(User user, string email)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");

            if (!user.EmailAddresses.Any(x => x.Value.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                var entity = new EmailAddress
                {
                    Value = email,
                    IsDefault = !user.EmailAddresses.Any(x => x.IsDefault),
                };
                user.EmailAddresses.Add(entity);
            }
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");

            var entity = user.EmailAddresses.FirstOrDefault(x => x.IsDefault && x.IsConfirmed)
                ?? user.EmailAddresses.FirstOrDefault(x => x.IsDefault)
                ?? user.EmailAddresses.FirstOrDefault(x => x.IsConfirmed)
                ?? user.EmailAddresses.FirstOrDefault();
            return Task.FromResult(entity != null ? entity.Value : null);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.IsDefault && x.IsConfirmed && x.Value.Equals(email, StringComparison.OrdinalIgnoreCase)))
                ?? GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.IsDefault && x.Value.Equals(email, StringComparison.OrdinalIgnoreCase)))
                ?? GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.IsConfirmed && x.Value.Equals(email, StringComparison.OrdinalIgnoreCase)))
                ?? GetUserAggregateAsync(u => u.EmailAddresses.Any(x => x.Value.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        #endregion
        #region IUserConfirmationStore

        public Task<bool> IsConfirmedAsync(User user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.LocalMembership != null && user.LocalMembership.IsConfirmed);
        }

        public Task SetConfirmedAsync(User user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");

            if (user.LocalMembership == null && confirmed)
                user.LocalMembership = new LocalMembership();
            if (user.LocalMembership != null)
                user.LocalMembership.IsConfirmed = confirmed;
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
    }
}
