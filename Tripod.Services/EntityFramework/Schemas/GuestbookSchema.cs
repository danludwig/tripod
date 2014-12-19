using System.Data.Entity.ModelConfiguration;
using Tripod.Domain.Guestbook;

namespace Tripod.Services.EntityFramework
{
    [UsedImplicitly]
    public class GuestbookDb : EntityTypeConfiguration<GuestbookEntry>
    {
        private const string SchemaName = "Guestbook";

        public GuestbookDb()
        {
            ToTable("Entry", SchemaName);

            HasKey(x => x.Id);

            Property(x => x.Text).IsRequired().HasMaxLength(GuestbookEntry.Constraints.TextMaxLength);
        }
    }
}
