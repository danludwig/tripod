using System;
using System.IO;

namespace Tripod
{
    public static class PathExtensions
    {
        public static string GetFullPath(this AppDomain appDomain, string relativePath)
        {
            var fullPath = appDomain.BaseDirectory;
            if (relativePath.Contains("/"))
                relativePath = relativePath.Replace("/", @"\");
            if (!relativePath.StartsWith(fullPath))
                fullPath = Path.Combine(fullPath, relativePath);
            return fullPath;
        }
    }
}
