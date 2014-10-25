using System;

namespace Tripod
{
    public static class FakeData
    {
        public static string Email()
        {
            return string.Format("{0}@domain.tld", Guid.NewGuid());
        }
    }
}
