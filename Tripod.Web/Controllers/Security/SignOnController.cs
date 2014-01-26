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

        [UsedImplicitly]
        public SignOnController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on")]
        public virtual ActionResult Index(string provider, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(provider)) return View(MVC.Errors.Views.BadRequest);

            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action(MVC.SignOn.Index(returnUrl)));
        }

        [HttpGet, Route("sign-on")]
        public virtual async Task<ActionResult> Index(string returnUrl)
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
                return this.RedirectToLocal(returnUrl);

            // if user doesn't have an email claim, we need them to confirm an email address
            var emailClaim = await _queries.Execute(new ExternalCookieClaim(ClaimTypes.Email));
            if (emailClaim == null)
                return RedirectToAction(await MVC.SignOnEmail.Index(returnUrl));

            // if user does have an email claim, create a confirmation against it
            var createEmailConfirmation = new CreateEmailConfirmation
            {
                Purpose = EmailConfirmationPurpose.CreateRemoteUser,
                EmailAddress = emailClaim.Value,
            };
            await _commands.Execute(createEmailConfirmation);
            return RedirectToAction(await MVC.SignOnUser.Index(createEmailConfirmation.CreatedEntity.Token, returnUrl));
        }
	}
}