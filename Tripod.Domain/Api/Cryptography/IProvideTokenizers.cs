using Microsoft.AspNet.Identity;

namespace Tripod
{
    public interface IProvideTokenizers
    {
        ITokenProvider CookieEncryptionTokens { get; }
    }
}