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
        }
    }
}
