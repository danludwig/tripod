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
    public partial class SignUpController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignUpController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        #region SendVerificationEmail

        [HttpGet, Route("sign-up")]
        public virtual ViewResult SendVerificationEmail(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignUp.SendVerificationEmail());
            ViewBag.Purpose = EmailVerificationPurpose.CreateLocalUser;
            ViewBag.VerifyUrlFormat = VerifyUrlFormat(returnUrl);
            ViewBag.SendFromUrl = SendFromUrl(returnUrl);
            return View(MVC.Security.Views.SignUpSendVerificationEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
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
                return View(MVC.Security.Views.SignUpSendVerificationEmail, command);
            }

            command.VerifyUrlFormat = VerifyUrlFormat(returnUrl);
            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.SignUp.VerifyEmailSecret(command.CreatedTicket, returnUrl));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/validate/{fieldName?}", Order = 1)]
        public virtual ActionResult ValidateSendVerificationEmail(SendVerificationEmail command, string fieldName = null)
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

        #endregion
        #region VerifyConfirmEmailSecret

        [HttpGet, Route("sign-up/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyEmailSecret(string ticket, string returnUrl)
        {
            var confirmation = await _queries.Execute(new EmailVerificationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailVerification, object>>[]
                {
                    x => x.EmailAddress,
                }
            });
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignUp.VerifyEmailSecret(ticket, null));
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailVerificationPurpose.CreateLocalUser;
            if (Session.VerifyEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.EmailAddress.Value;
            return View(MVC.Security.Views.SignUpVerifyEmailSecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyEmailSecret(string ticket, VerifyEmailSecret command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignUp.VerifyEmailSecret(ticket, null));
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailVerificationPurpose.CreateLocalUser;
                if (Session.VerifyEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignUpVerifyEmailSecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.SignUp.CreateLocalMembership(command.Token, returnUrl));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/{ticket}/validate/{fieldName?}", Order = 2)]
        public virtual ActionResult ValidateVerifyEmailSecret(VerifyEmailSecret command, string fieldName = null)
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

        #endregion
        #region CreateLocalMembership

        [HttpGet, Route("sign-up/register", Order = 1)]
        public virtual async Task<ActionResult> CreateLocalMembership(string token, string returnUrl)
        {
            var userToken = await _queries.Execute(new EmailVerificationUserToken(token));
            if (userToken == null) return HttpNotFound();
            var verification = await _queries.Execute(new EmailVerificationBy(userToken.Value));
            if (verification == null) return HttpNotFound();

            // todo: verification cannot be expired, redeemed, or for different purpose

            ViewBag.Token = token;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EmailAddress = verification.EmailAddress.Value;
            return View(MVC.Security.Views.SignUpCreateLocalMembership);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/register", Order = 1)]
        public virtual async Task<ActionResult> CreateLocalMembership(CreateLocalMembership command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.Token = command.Token;
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignUpCreateLocalMembership, command);
            }

            await _commands.Execute(command);

            var signIn = new SignIn
            {
                UserNameOrVerifiedEmail = command.UserName,
                Password = command.Password
            };
            await _commands.Execute(signIn);
            Session.VerifyEmailTickets(null);
            Response.ClientCookie(signIn.SignedIn.Id, _queries);
            return this.RedirectToLocal(returnUrl, await MVC.UserSettings.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/register/validate/{fieldName?}", Order = 1)]
        public virtual ActionResult ValidateCreateLocalMembership(CreateLocalMembership command, string fieldName = null)
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
