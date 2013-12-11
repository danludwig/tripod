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

            HasMany(x => x.Permissions)
                .WithMany(x => x.Users)
                .Map(x => x.ToTable("UserGivenPrivilege", SchemaName).MapLeftKey("UserId").MapRightKey("RoleId"))
            ;
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
}
