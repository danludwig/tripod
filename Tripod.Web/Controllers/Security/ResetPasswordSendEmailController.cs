using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class ResetPasswordSendEmailController : Controller
    {
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public ResetPasswordSendEmailController(IProcessCommands commands)
        {
            _commands = commands;
        }

        [HttpGet, Route("sign-in/password")]
        public virtual ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Purpose = EmailVerificationPurpose.ForgotPassword;
            ViewBag.VerifyUrlFormat = VerifyUrlFormat(returnUrl);
            ViewBag.SendFromUrl = Url.AbsoluteAction(Request.Url, MVC.ResetPasswordSendEmail.Index(returnUrl));
            return View(MVC.Security.Views.ResetPassword.SendEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password")]
        public virtual async Task<ActionResult> Post(SendVerificationEmail command, string returnUrl)
        {
            if (command == null || command.Purpose == EmailVerificationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(MVC.Security.Views.ResetPassword.SendEmail, command);
            }

            command.VerifyUrlFormat = VerifyUrlFormat(returnUrl);
            command.SendFromUrl = Url.AbsoluteAction(Request.Url, MVC.ResetPasswordSendEmail.Index(returnUrl));
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.ResetPasswordVerifySecret.Index(command.CreatedTicket, returnUrl));
        }

        private string VerifyUrlFormat(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var encodedUrlFormat = Url.Action(MVC.ResetPassword.Index("{0}", "{1}", "{2}"));
            var decodedUrlFormat = HttpUtility.UrlDecode(encodedUrlFormat);
            Debug.Assert(decodedUrlFormat != null);
            var formattedUrl = string.Format(decodedUrlFormat, "{0}", "{1}", returnUrl);
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, formattedUrl);
        }
    }
}