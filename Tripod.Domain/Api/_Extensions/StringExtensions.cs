using System.Collections.Generic;
using System.Text;

namespace Tripod
{
    public static class StringExtensions
    {
        public static string Format(this IEnumerable<KeyValuePair<string, string>> replacements, string template)
        {
            if (string.IsNullOrWhiteSpace(template))
                return template;

            var content = new StringBuilder(template);
            if (replacements != null)
            {
                foreach (var replacement in replacements)
                {
                    content.Replace(replacement.Key, replacement.Value);
                }
            }

            return content.ToString();
        }
    }
}
