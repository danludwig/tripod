namespace Tripod.Domain.Security
{
    public enum EmailConfirmationPurpose
    {
        CreateLocalUser = 1, // must create both user and password for a new locl account
        CreateRemoteUser = 2, // confirm email not provided by remote membership account like twitter
        CreatePassword = 3, // add a local password to an existing account
        ResetPassword = 3, // reset password for a local membership acount
        AddEmail = 5, // add another email address to user account
    }
}