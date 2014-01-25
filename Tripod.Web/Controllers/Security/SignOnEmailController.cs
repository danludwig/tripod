using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOnEmailController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOnEmailController(IProcessQueries queries, IProcessCommands commands)
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
            ViewBag.ActionUrl = Url.Action(MVC.SignOnEmail.Index());
            ViewBag.Purpose = EmailConfirmationPurpose.CreateRemoteUser;
            ViewBag.SendFromUrl = SendFromUrl(returnUrl);
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            return View(MVC.Security.Views.SignOnEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/email")]
        public virtual async Task<ActionResult> Index(SendConfirmationEmail command, string returnUrl, string loginProvider)
        {
            // todo: make sure we still have a remote login

            if (command == null || command.Purpose == EmailConfirmationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignOnEmail.Index());
                ViewBag.LoginProvider = loginProvider;
                return View(MVC.Security.Views.SignOnEmail, command);
            }

            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.AddConfirmEmailTicket(command.CreatedTicket);

            return RedirectToAction(await MVC.SignOnSecret.Index(command.CreatedTicket, returnUrl));
        }

        private string SendFromUrl(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.SignIn.Index(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }
    }
}
