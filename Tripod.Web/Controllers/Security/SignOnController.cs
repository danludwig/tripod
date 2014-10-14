using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOnController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly AppConfiguration _appConfiguration;

        [UsedImplicitly]
        public SignOnController(
              IProcessQueries queries
            , IProcessCommands commands
            , AppConfiguration appConfiguration
        )
        {
            _queries = queries;
            _commands = commands;
            _appConfiguration = appConfiguration;
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on")]
        public virtual ActionResult Post(string provider, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(provider)) return View(MVC.Errors.Views.BadRequest);

            // Request a redirect to the external login provider
            string redirectUri = Url.Action(MVC.SignOn.SignOnCallback(returnUrl));
            return new ChallengeResult(_appConfiguration, provider, redirectUri);
        }

        [HttpGet, Route("sign-on")]
        public virtual async Task<ActionResult> SignOnCallback(string returnUrl)
        {
            // make sure we have external login info
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null)
                return RedirectToAction(MVC.SignIn.Index());

            var signOn = new SignOn
            {
                Principal = User,
            };
            await _commands.Execute(signOn);
            if (signOn.SignedOn != null)
            {
                Response.ClientCookie(signOn.SignedOn.Id, _queries);
                return this.RedirectToLocal(returnUrl, await MVC.UserName.Index());
            }

            // if user doesn't have an email claim, we need them to verify an email address
            var emailClaim = await _queries.Execute(new ExternalCookieClaim(ClaimTypes.Email));
            if (emailClaim == null)
                return RedirectToAction(await MVC.SignOnSendEmail.Index(returnUrl));

            // if user does have an email claim, create a verification against it
            var createEmailVerification = new CreateEmailVerification
            {
                Purpose = EmailVerificationPurpose.CreateRemoteUser,
                EmailAddress = emailClaim.Value,
            };
            await _commands.Execute(createEmailVerification);

            return RedirectToAction(await MVC.SignOnCreateUser.Index(
                createEmailVerification.CreatedEntity.Token,
                createEmailVerification.CreatedEntity.Ticket,
                returnUrl));
        }
    }
}
