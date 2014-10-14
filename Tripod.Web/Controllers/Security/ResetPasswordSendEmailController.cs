using System.Threading.Tasks;
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
            ViewBag.VerifyUrlFormat = Url.AbsoluteActionFormat(
                MVC.ResetPassword.Index("{0}", "{1}", returnUrl).Result);
            ViewBag.SendFromUrl = Url.AbsoluteAction(
                MVC.ResetPasswordSendEmail.Index(returnUrl));
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

            command.VerifyUrlFormat = Url.AbsoluteActionFormat(
                MVC.ResetPassword.Index("{0}", "{1}", returnUrl).Result);
            command.SendFromUrl = Url.AbsoluteAction(
                MVC.ResetPasswordSendEmail.Index(returnUrl));
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.ResetPasswordVerifySecret.Index(command.CreatedTicket, returnUrl));
        }
    }
}