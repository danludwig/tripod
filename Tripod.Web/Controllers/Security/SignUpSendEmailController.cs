using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignUpSendEmailController : Controller
    {
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignUpSendEmailController(IProcessCommands commands)
        {
            _commands = commands;
        }

        [HttpGet, Route("sign-up")]
        public virtual ViewResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignUpSendEmail.Post());
            ViewBag.Purpose = EmailVerificationPurpose.CreateLocalUser;
            ViewBag.VerifyUrlFormat = Url.AbsoluteActionFormat(MVC.SignUpCreateUser.Index("{0}", "{1}", returnUrl).Result);
            ViewBag.SendFromUrl = Url.AbsoluteAction(MVC.SignUpSendEmail.Index(returnUrl));
            return View(MVC.Security.Views.SignUp.SendEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/validate/{fieldName?}", Order = 1)]
        public virtual ActionResult Validate(SendVerificationEmail command, string fieldName = null)
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

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual async Task<ActionResult> Post(SendVerificationEmail command, string returnUrl)
        {
            if (command == null || command.Purpose == EmailVerificationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignUpSendEmail.Post());
                return View(MVC.Security.Views.SignUp.SendEmail, command);
            }

            command.VerifyUrlFormat = Url.AbsoluteActionFormat(
                await MVC.SignUpCreateUser.Index("{0}", "{1}", returnUrl));
            command.SendFromUrl = Url.AbsoluteAction(
                MVC.SignUpSendEmail.Index(returnUrl));
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.SignUpVerifySecret.Index(command.CreatedTicket, returnUrl));
        }
    }
}