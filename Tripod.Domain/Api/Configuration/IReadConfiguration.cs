using System.Collections.Specialized;
using System.Configuration;

namespace Tripod
{
    public interface IReadConfiguration
    {
        NameValueCollection AppSettings { get; }
        ConfigurationSection GetSection(string sectionName);
    }
}
