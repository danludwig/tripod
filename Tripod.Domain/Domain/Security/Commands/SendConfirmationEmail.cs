using FluentValidation;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Authenticate user's local membership.
    /// </summary>
    public class SendConfirmationEmail : IDefineCommand
    {
        public string EmailAddress { get; set; }
        public bool IsExpectingEmail { get; set; }
    }

    public class ValidateSendConfirmationEmailCommand : AbstractValidator<SendConfirmationEmail>
    {
        public ValidateSendConfirmationEmailCommand(IProcessQueries queries)
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .EmailAddress()
                    .WithName(EmailAddress.Constraints.Label);

            RuleFor(x => x.IsExpectingEmail)
                .Equal(true)
                    .WithMessage(Resources.Validation_SendConfirmationEmail_IsExpectingEmail)
                        .WithName(EmailAddress.Constraints.Label.ToLower());
        }
    }

    //public class HandleSendConfirmationEmailCommand : IHandleCommand<SendConfirmationEmail>
    //{
    //    private readonly UserManager<User, int> _userManager;
    //    private readonly IAuthenticate _authenticator;

    //    public HandleSendConfirmationEmailCommand(UserManager<User, int> userManager, IAuthenticate authenticator)
    //    {
    //        _userManager = userManager;
    //        _authenticator = authenticator;
    //    }

    //    public async Task Handle(SendConfirmationEmail command)
    //    {
    //        var user = await _userManager.FindAsync(command.UserName, command.Password);
    //        await _authenticator.SignOn(user, command.IsPersistent);
    //    }
    //}
}
