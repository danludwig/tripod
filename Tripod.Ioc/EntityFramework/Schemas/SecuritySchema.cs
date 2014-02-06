using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tripod.Domain.Security;

namespace Tripod.Ioc.EntityFramework
{
    [UsedImplicitly]
    public class UserDb : EntityTypeConfiguration<User>
    {
        public const string SchemaName = "Security";

        public UserDb()
        {
            ToTable("User", SchemaName);

            HasKey(x => x.Id);

            Property(x => x.Name).IsRequired().HasMaxLength(User.Constraints.NameMaxLength);
            Property(x => x.SecurityStamp).HasMaxLength(User.Constraints.SecurityStampMaxLength);

            HasMany(x => x.Permissions).WithMany(x => x.Users)
                .Map(x => x.ToTable("UserGivenPermission", SchemaName).MapLeftKey("UserId").MapRightKey("PermissionId"));
        }
    }

    [UsedImplicitly]
    public class PermissionDb : EntityTypeConfiguration<Permission>
    {
        public PermissionDb()
        {
            ToTable("Permission", UserDb.SchemaName);

            HasKey(x => x.Id);

            Property(x => x.Name).IsRequired().HasMaxLength(Permission.Constraints.NameMaxLength);

            HasMany(x => x.Users).WithMany(x => x.Permissions)
                .Map(x => x.ToTable("UserGivenPermission", UserDb.SchemaName).MapLeftKey("PermissionId").MapRightKey("UserId"));
        }
    }

    [UsedImplicitly]
    public class EmailAddressDb : EntityTypeConfiguration<EmailAddress>
    {
        public EmailAddressDb()
        {
            ToTable(typeof(EmailAddress).Name, UserDb.SchemaName);

            HasKey(x => x.Id);
            Property(x => x.Value).IsRequired().HasMaxLength(EmailAddress.Constraints.ValueMaxLength);
            Property(x => x.HashedValue).IsRequired().HasMaxLength(EmailAddress.Constraints.HashedValueMaxLength);

            HasOptional(x => x.User).WithMany(x => x.EmailAddresses).HasForeignKey(x => x.UserId).WillCascadeOnDelete();
        }
    }

    [UsedImplicitly]
    public class EmailVerificationDb : EntityTypeConfiguration<EmailVerification>
    {
        public EmailVerificationDb()
        {
            ToTable(typeof(EmailVerification).Name, UserDb.SchemaName);

            HasKey(x => x.Id);

            Property(x => x.Secret).HasMaxLength(EmailVerification.Constraints.SecretMaxLength);
            Property(x => x.Ticket).HasMaxLength(EmailVerification.Constraints.TicketMaxLength);

            HasRequired(x => x.EmailAddress).WithMany().HasForeignKey(x => x.EmailAddressId).WillCascadeOnDelete();
            HasOptional(x => x.Message).WithOptionalDependent()
                .Map(x => x.MapKey("EmailMessageId")).WillCascadeOnDelete(false);
        }
    }

    [UsedImplicitly]
    public class EmailMessageDb : EntityTypeConfiguration<EmailMessage>
    {
        public EmailMessageDb()
        {
            ToTable(typeof(EmailMessage).Name, UserDb.SchemaName);

            HasKey(x => x.Id);

            Property(x => x.From).HasMaxLength(EmailMessage.Constraints.FromMaxLength);
            Property(x => x.Subject).HasMaxLength(EmailMessage.Constraints.SubjectMaxLength);
            Property(x => x.LastSendError).HasMaxLength(EmailMessage.Constraints.LastSendErrorMaxLength);

            HasRequired(x => x.EmailAddress).WithMany().HasForeignKey(x => x.EmailAddressId).WillCascadeOnDelete();
        }
    }

    [UsedImplicitly]
    public class LocalMembershipDb : EntityTypeConfiguration<LocalMembership>
    {
        public LocalMembershipDb()
        {
            ToTable(typeof(LocalMembership).Name, UserDb.SchemaName);

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("UserId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(x => x.User).WithOptional(x => x.LocalMembership).WillCascadeOnDelete();
        }
    }

    [UsedImplicitly]
    public class RemoteMembershipDb : EntityTypeConfiguration<RemoteMembership>
    {
        public RemoteMembershipDb()
        {
            ToTable(typeof(RemoteMembership).Name, UserDb.SchemaName);

            Ignore(x => x.Id);
            HasKey(x => new { x.LoginProvider, x.ProviderKey });
            Property(x => x.LoginProvider).HasColumnName("LoginProvider").HasMaxLength(RemoteMembership.Constraints.ProviderMaxLength);
            Property(x => x.ProviderKey).HasColumnName("ProviderKey").HasMaxLength(RemoteMembership.Constraints.ProviderUserIdMaxLength);

            HasRequired(x => x.User).WithMany(x => x.RemoteMemberships).HasForeignKey(x => x.UserId).WillCascadeOnDelete();
        }
    }

    [UsedImplicitly]
    public class UserClaimDb : EntityTypeConfiguration<UserClaim>
    {
        public UserClaimDb()
        {
            ToTable(typeof(UserClaim).Name, UserDb.SchemaName);

            HasKey(x => x.Id);

            HasRequired(x => x.User).WithMany(x => x.Claims).HasForeignKey(x => x.UserId).WillCascadeOnDelete();
        }
    }
}
