using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Tripod.Services.Cryptography
{
    [UsedImplicitly]
    public class OwinDataProtectionTokenizers : IProvideTokenizers
    {
        public OwinDataProtectionTokenizers(IDataProtectionProvider dataProtectionProvider)
        {
            if (dataProtectionProvider == null) throw new ArgumentNullException("dataProtectionProvider");
            CookieEncryptionTokens = new DataProtectorTokenProvider(dataProtectionProvider.Create("ProtectCookie"));
        }

        public ITokenProvider CookieEncryptionTokens { get; private set; }
    }
}
