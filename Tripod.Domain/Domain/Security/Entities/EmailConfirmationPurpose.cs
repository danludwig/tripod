namespace Tripod.Domain.Security
{
    public enum EmailConfirmationPurpose
    {
        CreatePassword = 1, // sign up with a local membership account
        ResetPassword = 2, // reset password for a local membership acount
        CreateUser = 3, // confirm email not provided by remote membership account
        AddEmail = 4, // add another email address to user account
    }
}