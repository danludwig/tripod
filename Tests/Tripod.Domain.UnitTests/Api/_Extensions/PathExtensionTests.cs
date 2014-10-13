using System;
using Should;
using Xunit;

namespace Tripod
{
    public class PathExtensionTests
    {
        [Fact]
        public void GetFullPath_GetsFullPath_BasedOnAppDomainBaseDirectory()
        {
            const string relativePath = "App_Data/path/to/something.ext";
            var fullPath = AppDomain.CurrentDomain.GetFullPath(relativePath);
            var expected = string.Format(@"{0}\{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                relativePath.Replace("/", @"\"));
            fullPath.ShouldEqual(expected);
        }
    }
}
