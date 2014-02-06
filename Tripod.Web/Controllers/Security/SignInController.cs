using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignInController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignInController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-in")]
        public virtual ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Security.Views.SignIn);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in")]
        public virtual async Task<ActionResult> Index(SignIn command, string returnUrl)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);
            if (!ModelState.IsValid) return View(MVC.Security.Views.SignIn, command);
            await _commands.Execute(command);
            Response.ClientCookie(command.SignedIn.Id, _queries);
            return this.RedirectToLocal(returnUrl, await MVC.UserSettings.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/validate/{fieldName?}")]
        public virtual ActionResult Validate(SignIn command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null || command.PropertyName(x => x.Password).Equals(fieldName, StringComparison.OrdinalIgnoreCase))
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState, fieldName);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, fieldName);

            return new CamelCaseJsonResult(result);
        }

        [HttpGet, Route("sign-in/password")]
        public virtual ActionResult SendVerificationEmail(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Purpose = EmailVerificationPurpose.ForgotPassword;
            ViewBag.VerifyUrlFormat = VerifyUrlFormat(returnUrl);
            ViewBag.SendFromUrl = SendFromUrl(returnUrl);
            return View(MVC.Security.Views.SignInSendVerificationEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password")]
        public virtual async Task<ActionResult> SendVerificationEmail(SendVerificationEmail command, string returnUrl)
        {
            if (command == null || command.Purpose == EmailVerificationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignUp.SendVerificationEmail());
                return View(MVC.Security.Views.SignInSendVerificationEmail, command);
            }

            command.VerifyUrlFormat = VerifyUrlFormat(returnUrl);
            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.SignUp.VerifyEmailSecret(command.CreatedTicket, returnUrl));
        }

        private string VerifyUrlFormat(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var encodedUrlFormat = Url.Action(MVC.SignUp.CreateLocalMembership("{0}", "{1}"));
            var decodedUrlFormat = HttpUtility.UrlDecode(encodedUrlFormat);
            Debug.Assert(decodedUrlFormat != null);
            var formattedUrl = string.Format(decodedUrlFormat, "{0}", returnUrl);
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, formattedUrl);
        }

        private string SendFromUrl(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.SignUp.SendVerificationEmail(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }
    }
}