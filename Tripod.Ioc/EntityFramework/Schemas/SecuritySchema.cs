using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tripod.Domain.Security;

namespace Tripod.Ioc.EntityFramework
{
    public class UserDb : EntityTypeConfiguration<User>
    {
        public const string SchemaName = "Security";

        public UserDb()
        {
            ToTable("User", SchemaName);

            HasKey(x => x.Id);

            Property(x => x.Name).IsRequired().HasMaxLength(User.Constraints.NameMaxLength);

            HasMany(x => x.Permissions).WithMany(x => x.Users)
                .Map(x => x.ToTable("UserGivenPermission", SchemaName).MapLeftKey("UserId").MapRightKey("RoleId"));

            HasMany(x => x.Logins).WithRequired(x => x.Owner).HasForeignKey(x => x.UserId).WillCascadeOnDelete();

            HasMany(x => x.Claims).WithRequired(x => x.Owner).HasForeignKey(x => x.UserId).WillCascadeOnDelete();
        }
    }

    public class PermissionDb : EntityTypeConfiguration<Permission>
    {
        public PermissionDb()
        {
            ToTable("Permission", UserDb.SchemaName);

            HasKey(x => x.Id);

            Property(x => x.Name).IsRequired().HasMaxLength(Permission.Constraints.NameMaxLength);
        }
    }

    public class EmailAddressDb : EntityTypeConfiguration<EmailAddress>
    {
        public EmailAddressDb()
        {
            ToTable(typeof(EmailAddress).Name, UserDb.SchemaName);

            HasKey(x => x.Id);
            Property(x => x.Value).IsRequired().HasMaxLength(EmailAddress.Constraints.ValueMaxLength);

            HasOptional(x => x.Owner).WithMany(x => x.EmailAddresses).HasForeignKey(x => x.OwnerId).WillCascadeOnDelete();
        }
    }

    public class LocalMembershipDb : EntityTypeConfiguration<LocalMembership>
    {
        public LocalMembershipDb()
        {
            ToTable(typeof(LocalMembership).Name, UserDb.SchemaName);

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("UserId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(x => x.Owner).WithOptional(x => x.LocalMembership).WillCascadeOnDelete();
        }
    }

    public class RemoteMembershipDb : EntityTypeConfiguration<RemoteMembership>
    {
        public RemoteMembershipDb()
        {
            ToTable(typeof(RemoteMembership).Name, UserDb.SchemaName);

            Ignore(x => x.Id);
            HasKey(x => new { x.LoginProvider, x.ProviderKey });
            Property(x => x.LoginProvider).HasColumnName("LoginProvider").HasMaxLength(RemoteMembership.Constraints.ProviderMaxLength);
            Property(x => x.ProviderKey).HasColumnName("ProviderKey").HasMaxLength(RemoteMembership.Constraints.ProviderUserIdMaxLength);

            HasRequired(x => x.Owner).WithMany(x => x.Logins).HasForeignKey(x => x.UserId).WillCascadeOnDelete();
        }
    }

    public class ClaimDb : EntityTypeConfiguration<Claim>
    {
        public ClaimDb()
        {
            ToTable(typeof(Claim).Name, UserDb.SchemaName);

            HasKey(x => x.Id);

            HasRequired(x => x.Owner).WithMany(x => x.Claims).HasForeignKey(x => x.UserId).WillCascadeOnDelete();
        }
    }
}
