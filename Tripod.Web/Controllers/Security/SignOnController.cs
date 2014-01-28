using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignOnController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOnController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        #region Index

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on")]
        public virtual ActionResult Index(string provider, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(provider)) return View(MVC.Errors.Views.BadRequest);

            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action(MVC.SignOn.Index(returnUrl)));
        }

        [HttpGet, Route("sign-on")]
        public virtual async Task<ActionResult> Index(string returnUrl)
        {
            // make sure we have external login info
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null)
                return RedirectToAction(MVC.SignIn.Index());

            var signOn = new SignOn
            {
                Principal = User,
            };
            await _commands.Execute(signOn);
            if (signOn.SignedOn != null)
                return this.RedirectToLocal(returnUrl, await MVC.User.ById(signOn.SignedOn.Id));

            // if user doesn't have an email claim, we need them to confirm an email address
            var emailClaim = await _queries.Execute(new ExternalCookieClaim(ClaimTypes.Email));
            if (emailClaim == null)
                return RedirectToAction(await MVC.SignOn.SendConfirmationEmail(returnUrl));

            // if user does have an email claim, create a confirmation against it
            var createEmailConfirmation = new CreateEmailConfirmation
            {
                Purpose = EmailConfirmationPurpose.CreateRemoteUser,
                EmailAddress = emailClaim.Value,
            };
            await _commands.Execute(createEmailConfirmation);
            return RedirectToAction(await MVC.SignOn.CreateRemoteMembership(createEmailConfirmation.CreatedEntity.Token, returnUrl));
        }

        #endregion
        #region SendConfirmationEmail

        [HttpGet, Route("sign-on/email", Order = 1)]
        public virtual async Task<ActionResult> SendConfirmationEmail(string returnUrl)
        {
            // make sure we still have a remote login
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null)
                return RedirectToAction(MVC.SignIn.Index());

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignOn.SendConfirmationEmail());
            ViewBag.Purpose = EmailConfirmationPurpose.CreateRemoteUser;
            ViewBag.SendFromUrl = SendFromUrl(returnUrl);
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            return View(MVC.Security.Views.SignOnSendConfirmationEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/email", Order = 1)]
        public virtual async Task<ActionResult> SendConfirmationEmail(SendConfirmationEmail command, string returnUrl, string loginProvider)
        {
            // todo: make sure we still have a remote login

            if (command == null || command.Purpose == EmailConfirmationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignOn.SendConfirmationEmail());
                ViewBag.LoginProvider = loginProvider;
                return View(MVC.Security.Views.SignOnSendConfirmationEmail, command);
            }

            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.AddConfirmEmailTicket(command.CreatedTicket);

            return RedirectToAction(await MVC.SignOn.VerifyConfirmEmailSecret(command.CreatedTicket, returnUrl));
        }

        private string SendFromUrl(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.SignIn.Index(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }

        #endregion
        #region VerifyConfirmEmailSecret

        [HttpGet, Route("sign-on/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyConfirmEmailSecret(string ticket, string returnUrl)
        {
            // todo: make sure we still have a remote login

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
            ViewBag.ActionUrl = Url.Action(MVC.SignOn.VerifyConfirmEmailSecret(ticket, null));
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailConfirmationPurpose.CreateRemoteUser;
            if (Session.ConfirmEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.Owner.Value;
            return View(MVC.Security.Views.SignOnVerifyConfirmEmailSecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyConfirmEmailSecret(string ticket, VerifyConfirmEmailSecret command, string returnUrl, string emailAddress)
        {
            // todo: make sure we still have a remote login

            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignOn.VerifyConfirmEmailSecret(ticket, null));
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailConfirmationPurpose.CreateRemoteUser;
                if (Session.ConfirmEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignOnVerifyConfirmEmailSecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.SignOn.CreateRemoteMembership(command.Token, returnUrl));
        }

        #endregion
        #region CreateRemoteMembership

        [HttpGet, Route("sign-on/register", Order = 1)]
        public virtual async Task<ActionResult> CreateRemoteMembership(string token, string returnUrl)
        {
            // make sure we still have a remote login
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null)
                return RedirectToAction(MVC.SignIn.Index());

            var userToken = await _queries.Execute(new EmailConfirmationUserToken(token));
            if (userToken == null) return HttpNotFound();
            var confirmation = await _queries.Execute(new EmailConfirmationBy(userToken.Value));
            if (confirmation == null) return HttpNotFound();
            var emailClaim = await _queries.Execute(new ExternalCookieClaim(ClaimTypes.Email));

            // todo: confirmation cannot be expired, redeemed, or for different purpose

            // if suggested username is already in use, use email address
            var user = await _queries.Execute(new UserBy(loginInfo.UserName));

            ViewBag.Token = token;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EmailAddress = confirmation.Owner.Value;
            ViewBag.UserName = user == null ? loginInfo.UserName : ViewBag.EmailAddress;
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            ViewBag.HasClaimsEmail = emailClaim != null;
            return View(MVC.Security.Views.SignOnCreateRemoteMembership);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/register", Order = 1)]
        public virtual async Task<ActionResult> CreateRemoteMembership(CreateRemoteMembership command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            // make sure we still have a remote login
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null)
                return RedirectToAction(MVC.SignIn.Index());

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var emailClaim = await _queries.Execute(new ExternalCookieClaim(ClaimTypes.Email));
                ViewBag.Token = command.Token;
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.EmailAddress = emailAddress;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                ViewBag.HasClaimsEmail = emailClaim != null;
                return View(MVC.Security.Views.SignOnCreateRemoteMembership, command);
            }

            await _commands.Execute(command);

            var signOn = new SignOn
            {
                Principal = User,
            };
            await _commands.Execute(signOn);
            return this.RedirectToLocal(returnUrl, await MVC.User.ById(signOn.SignedOn.Id));
        }

        [HttpPost, Route("sign-on/register/validate/{fieldName?}", Order = 1)]
        public virtual ActionResult ValidateCreateRemoteMembership(CreateRemoteMembership command, string fieldName = null)
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
