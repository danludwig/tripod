using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

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

            return View(MVC.Errors.Views.BadRequest);
        }

        [HttpPost, Route("sign-up/validate/{fieldName?}")]
        public virtual ActionResult Validate(SendConfirmationEmail command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState, fieldName);

            //ModelState[command.PropertyName(x => x.EmailAddress)].Errors.Clear();
            //result = new ValidatedFields(ModelState, fieldName);

            return new CamelCaseJsonResult(result);
        }

        private string SendFromUrl(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.SignIn.Index(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }
    }
}
