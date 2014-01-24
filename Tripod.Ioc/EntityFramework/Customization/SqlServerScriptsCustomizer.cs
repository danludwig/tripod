using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Tripod.Ioc.EntityFramework
{
    [UsedImplicitly]
    public class SqlServerScriptsCustomizer : ICustomizeDb
    {
        public void Customize(DbContext db)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sqlScriptNames = assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith("Tripod.Ioc.EntityFramework.Customization.SqlServer.") && x.EndsWith(".sql"));
            foreach (var sqlScriptName in sqlScriptNames)
            {
                var sqlScriptText = assembly.GetManifestResourceText(sqlScriptName);
                var sqlCommands = sqlScriptText.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                foreach (var sqlCommand in sqlCommands)
                    db.Database.ExecuteSqlCommand(sqlCommand);
            }
        }
    }
}
