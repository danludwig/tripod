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

        #region SendConfirmationEmail

        [HttpGet, Route("sign-up")]
        public virtual ViewResult SendConfirmationEmail(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignUp.SendConfirmationEmail());
            ViewBag.Purpose = EmailConfirmationPurpose.CreateLocalUser;
            ViewBag.ConfirmUrlFormat = ConfirmUrlFormat(returnUrl);
            ViewBag.SendFromUrl = SendFromUrl(returnUrl);
            return View(MVC.Security.Views.SignUpSendConfirmationEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual async Task<ActionResult> SendConfirmationEmail(SendConfirmationEmail command, string returnUrl, string loginProvider)
        {
            if (command == null || command.Purpose == EmailConfirmationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignUp.SendConfirmationEmail());
                return View(MVC.Security.Views.SignUpSendConfirmationEmail, command);
            }

            command.ConfirmUrlFormat = ConfirmUrlFormat(returnUrl);
            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.ConfirmEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.SignUp.VerifyConfirmEmailSecret(command.CreatedTicket, returnUrl));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/validate/{fieldName?}", Order = 1)]
        public virtual ActionResult ValidateSendConfirmationEmail(SendConfirmationEmail command, string fieldName = null)
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

        private string ConfirmUrlFormat(string returnUrl)
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
            var url = Url.Action(MVC.SignUp.SendConfirmationEmail(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }

        #endregion
        #region VerifyConfirmEmailSecret

        [HttpGet, Route("sign-up/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyConfirmEmailSecret(string ticket, string returnUrl)
        {
            var confirmation = await _queries.Execute(new EmailConfirmationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailConfirmation, object>>[]
                {
                    x => x.Owner,
                }
            });
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignUp.VerifyConfirmEmailSecret(ticket, null));
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailConfirmationPurpose.CreateLocalUser;
            if (Session.ConfirmEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.Owner.Value;
            return View(MVC.Security.Views.SignUpVerifyConfirmEmailSecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyConfirmEmailSecret(string ticket, VerifyConfirmEmailSecret command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignUp.VerifyConfirmEmailSecret(ticket, null));
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailConfirmationPurpose.CreateLocalUser;
                if (Session.ConfirmEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignUpVerifyConfirmEmailSecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.SignUp.CreateLocalMembership(command.Token, returnUrl));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/{ticket}/validate/{fieldName?}", Order = 2)]
        public virtual ActionResult ValidateVerifyConfirmEmailSecret(VerifyConfirmEmailSecret command, string fieldName = null)
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
            var userToken = await _queries.Execute(new EmailConfirmationUserToken(token));
            if (userToken == null) return HttpNotFound();
            var confirmation = await _queries.Execute(new EmailConfirmationBy(userToken.Value));
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation cannot be expired, redeemed, or for different purpose

            ViewBag.Token = token;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EmailAddress = confirmation.Owner.Value;
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
                UserName = command.UserName,
                Password = command.Password
            };
            await _commands.Execute(signIn);
            Session.ConfirmEmailTickets(null);
            Response.ClientCookie(signIn.SignedIn.Id, _queries);
            return this.RedirectToLocal(returnUrl, await MVC.User.ById(signIn.SignedIn.Id));
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
