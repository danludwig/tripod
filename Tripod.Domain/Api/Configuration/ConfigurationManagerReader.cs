using System.Collections.Specialized;
using System.Configuration;

namespace Tripod
{
    [UsedImplicitly]
    public class ConfigurationManagerReader : IReadConfiguration
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        public ConfigurationSection GetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName) as ConfigurationSection;
        }
    }
}