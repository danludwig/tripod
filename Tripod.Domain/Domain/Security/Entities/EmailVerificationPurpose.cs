namespace Tripod.Domain.Security
{
    public enum EmailVerificationPurpose
    {
        Invalid = 0,
        CreateLocalUser = 1, // must create both user and password for a new locl account
        CreateRemoteUser = 2, // verify email not provided by remote membership account like twitter
        ForgotPassword = 3, // reset password for a local membership acount
        AddEmail = 4, // add another email address to user account
    }
}