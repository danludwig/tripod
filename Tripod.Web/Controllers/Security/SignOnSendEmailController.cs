using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOnSendEmailController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOnSendEmailController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-on/email")]
        public virtual async Task<ActionResult> Index(string returnUrl)
        {
            // make sure we still have a remote login
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null)
                return RedirectToAction(MVC.SignIn.Index());

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignOnSendEmail.Post());
            ViewBag.Purpose = EmailVerificationPurpose.CreateRemoteUser;
            ViewBag.SendFromUrl = SendFromUrl(returnUrl);
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            return View(MVC.Security.Views.SignOn.SendEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/email")]
        public virtual async Task<ActionResult> Post(SendVerificationEmail command, string returnUrl, string loginProvider)
        {
            // todo: make sure we still have a remote login

            if (command == null || command.Purpose == EmailVerificationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignOnSendEmail.Post());
                ViewBag.LoginProvider = loginProvider;
                return View(MVC.Security.Views.SignOn.SendEmail, command);
            }

            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.SignOnVerifySecret.Index(command.CreatedTicket, returnUrl));
        }

        private string SendFromUrl(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.SignIn.Index(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }
    }
}