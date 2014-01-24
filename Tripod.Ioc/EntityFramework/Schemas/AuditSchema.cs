using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tripod.Domain.Audit;
using Tripod.Domain.Security;

namespace Tripod.Ioc.EntityFramework
{
    [UsedImplicitly]
    public class ExceptopmAuditDb : EntityTypeConfiguration<ExceptionAudit>
    {
        private const string SchemaName = "Audit";

        public ExceptopmAuditDb()
        {
            ToTable(typeof(ExceptionAudit).Name, SchemaName);

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("ErrorId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(x => x.Application).IsRequired().HasMaxLength(ExceptionAudit.Constraints.ApplicationMaxLength);
            Property(x => x.Host).IsRequired().HasMaxLength(ExceptionAudit.Constraints.HostMaxLength);
            Property(x => x.Type).IsRequired().HasMaxLength(ExceptionAudit.Constraints.TypeMaxLength);
            Property(x => x.Source).IsRequired().HasMaxLength(ExceptionAudit.Constraints.SourceMaxLength);
            Property(x => x.Message).IsRequired().HasMaxLength(ExceptionAudit.Constraints.MessageMaxLength);
            Property(x => x.User).IsRequired().HasMaxLength(User.Constraints.NameMaxLength);
            Property(x => x.OnUtc).HasColumnName("TimeUtc");
            Property(x => x.Sequence).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Xml).IsRequired().HasColumnName("AllXml").HasColumnType("ntext");
        }
    }
}
