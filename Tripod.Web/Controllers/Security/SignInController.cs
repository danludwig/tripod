using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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

        #region Index

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

        #endregion
        #region SendVerificationEmail

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
                return View(MVC.Security.Views.SignInSendVerificationEmail, command);
            }

            command.VerifyUrlFormat = VerifyUrlFormat(returnUrl);
            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.SignIn.VerifyEmailSecret(command.CreatedTicket, returnUrl));
        }

        private string VerifyUrlFormat(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var encodedUrlFormat = Url.Action(MVC.SignIn.ResetPassword("{0}", "{1}"));
            var decodedUrlFormat = HttpUtility.UrlDecode(encodedUrlFormat);
            Debug.Assert(decodedUrlFormat != null);
            var formattedUrl = string.Format(decodedUrlFormat, "{0}", returnUrl);
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, formattedUrl);
        }

        private string SendFromUrl(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.SignIn.SendVerificationEmail(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }

        #endregion
        #region VerifyConfirmEmailSecret

        [HttpGet, Route("sign-in/password/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyEmailSecret(string ticket, string returnUrl)
        {
            var confirmation = await _queries.Execute(new EmailVerificationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailVerification, object>>[]
                {
                    x => x.Owner,
                }
            });
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignIn.VerifyEmailSecret(ticket, null));
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailVerificationPurpose.ForgotPassword;
            if (Session.VerifyEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.Owner.Value;
            return View(MVC.Security.Views.SignInVerifyEmailSecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyEmailSecret(string ticket, VerifyEmailSecret command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignIn.VerifyEmailSecret(ticket, null));
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailVerificationPurpose.ForgotPassword;
                if (Session.VerifyEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignInVerifyEmailSecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.SignIn.ResetPassword(command.Token, returnUrl));
        }

        #endregion
        #region ResetPassword

        [HttpGet, Route("sign-in/password/recover", Order = 1)]
        public virtual async Task<ActionResult> ResetPassword(string token, string returnUrl)
        {
            var userToken = await _queries.Execute(new EmailVerificationUserToken(token));
            if (userToken == null) return HttpNotFound();
            var verification = await _queries.Execute(new EmailVerificationBy(userToken.Value));
            if (verification == null) return HttpNotFound();

            // todo: verification cannot be expired, redeemed, or for different purpose

            ViewBag.Token = token;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EmailAddress = verification.Owner.Value;
            return View(MVC.Security.Views.SignInRedeemEmailVerification);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password/recover", Order = 1)]
        public virtual async Task<ActionResult> ResetPassword(ResetPassword command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            var userToken = await _queries.Execute(new EmailVerificationUserToken(command.Token));
            if (userToken == null) return HttpNotFound();
            var verification = await _queries.Execute(new EmailVerificationBy(userToken.Value));
            if (verification == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Token = command.Token;
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignInRedeemEmailVerification, command);
            }

            await _commands.Execute(command);

            var signIn = new SignIn
            {
                UserNameOrVerifiedEmail = verification.Owner.Value,
                Password = command.Password
            };
            await _commands.Execute(signIn);
            Session.VerifyEmailTickets(null);
            Response.ClientCookie(signIn.SignedIn.Id, _queries);
            return this.RedirectToLocal(returnUrl, await MVC.UserSettings.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password/recover/validate/{fieldName?}", Order = 1)]
        public virtual ActionResult ValidateResetPassword(ResetPassword command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState, fieldName);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, fieldName);

            return new CamelCaseJsonResult(result);
        }

        #endregion
    }
}