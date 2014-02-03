using System;
using System.Collections.Generic;
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
    [Authorize]
    public partial class UserController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public UserController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        #region ChangeUserName (default form)

        [HttpGet, Route("settings")]
        public virtual async Task<ActionResult> SettingsIndex()
        {
            var view = await _queries.Execute(new UserViewBy(User.Identity.GetAppUserId()));
            if (view == null) return HttpNotFound();

            var model = new ChangeUserNameModel
            {
                UserView = view,
                Command = new ChangeUserName
                {
                    UserId = view.UserId,
                    UserName = view.UserName,
                },
            };

            return View(MVC.Security.Views.UserSettingsIndex, model);
        }

        [ValidateAntiForgeryToken]
        [HttpPut, Route("settings/username")]
        public virtual async Task<ActionResult> ChangeUserName(ChangeUserName command)
        {
            if (command == null) return View(MVC.Errors.BadRequest());

            var view = await _queries.Execute(new UserViewBy(command.UserId));
            if (view == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                var model = new ChangeUserNameModel
                {
                    UserView = view,
                    Command = command,
                };

                return View(MVC.Security.Views.UserSettingsIndex, model);
            }

            await _commands.Execute(command);
            Response.ClientCookie(command.SignedIn.Id, _queries);
            return RedirectToAction(await MVC.User.SettingsIndex());
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/username/validate")]
        public virtual ActionResult ValidateChangeUserName(ChangeUserName command)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, command.PropertyName(x => x.UserName));

            return new CamelCaseJsonResult(result);
        }

        #endregion
        #region EmailAddresses (& SendVerificationEmail)

        [HttpGet, Route("settings/emails")]
        public virtual async Task<ActionResult> Emails()
        {
            var user = await _queries.Execute(new UserViewBy(User.Identity.GetAppUserId()));
            var emails = await _queries.Execute(new EmailAddressViewsBy(User.Identity.GetAppUserId())
            {
                OrderBy = new Dictionary<Expression<Func<EmailAddressView, object>>, OrderByDirection>
                {
                    { x => x.IsPrimary, OrderByDirection.Descending },
                    { x => x.IsVerified, OrderByDirection.Descending },
                },
            });

            var model = new EmailAddressSettingsModel
            {
                UserView = user,
                EmailAddresses = emails.ToArray(),
                SendVerificationEmail = new SendVerificationEmail
                {
                    Purpose = EmailVerificationPurpose.AddEmail,
                    SendFromUrl = SendFromUrl(),
                    VerifyUrlFormat = VerifyUrlFormat(),
                },
            };

            ViewBag.ActionUrl = Url.Action(MVC.User.SendVerificationEmail());
            ViewBag.Purpose = model.SendVerificationEmail.Purpose;
            return View(MVC.Security.Views.UserEmailAddresses, model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/emails")]
        public virtual async Task<ActionResult> SendVerificationEmail(SendVerificationEmail command)
        {
            if (command == null || command.Purpose == EmailVerificationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                var user = await _queries.Execute(new UserViewBy(User.Identity.GetAppUserId()));
                var emails = await _queries.Execute(new EmailAddressViewsBy(User.Identity.GetAppUserId())
                {
                    OrderBy = new Dictionary<Expression<Func<EmailAddressView, object>>, OrderByDirection>
                    {
                        { x => x.IsPrimary, OrderByDirection.Descending },
                        { x => x.IsVerified, OrderByDirection.Descending },
                    },
                });

                var model = new EmailAddressSettingsModel
                {
                    UserView = user,
                    EmailAddresses = emails.ToArray(),
                    SendVerificationEmail = command,
                };

                TempData.Alerts("**Could not send verification email due to error(s) below.**", AlertFlavor.Danger);
                ViewBag.ActionUrl = Url.Action(MVC.User.SendVerificationEmail());
                return View(MVC.Security.Views.UserEmailAddresses, model);
            }

            command.VerifyUrlFormat = VerifyUrlFormat();
            command.SendFromUrl = SendFromUrl();
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.User.VerifyEmailSecret(command.CreatedTicket));
        }

        private string VerifyUrlFormat()
        {
            Debug.Assert(Request.Url != null);
            var encodedUrlFormat = Url.Action(MVC.User.RedeemEmailVerification("{0}"));
            var decodedUrlFormat = HttpUtility.UrlDecode(encodedUrlFormat);
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, decodedUrlFormat);
        }

        private string SendFromUrl()
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.User.Emails());
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }

        #endregion
        #region VerifyConfirmEmailSecret

        [HttpGet, Route("settings/emails/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyEmailSecret(string ticket)
        {
            var verification = await _queries.Execute(new EmailVerificationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailVerification, object>>[]
                {
                    x => x.Owner,
                }
            });
            if (verification == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.ActionUrl = Url.Action(MVC.User.VerifyEmailSecret(ticket));
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailVerificationPurpose.AddEmail;
            if (Session.VerifyEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = verification.Owner.Value;
            return View(MVC.Security.Views.AddEmailVerifyEmailSecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/emails/{ticket}", Order = 2)]
        public virtual async Task<ActionResult> VerifyEmailSecret(VerifyEmailSecret command, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ActionUrl = Url.Action(MVC.User.VerifyEmailSecret(command.Ticket));
                ViewBag.Ticket = command.Ticket;
                ViewBag.Purpose = EmailVerificationPurpose.AddEmail;
                if (Session.VerifyEmailTickets().Contains(command.Ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.AddEmailVerifyEmailSecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.User.RedeemEmailVerification(command.Token));
        }

        #endregion
        #region RedeemEmailVerification

        [HttpGet, Route("settings/emails/confirm", Order = 1)]
        public virtual async Task<ActionResult> RedeemEmailVerification(string token)
        {
            var userToken = await _queries.Execute(new EmailVerificationUserToken(token));
            if (userToken == null) return HttpNotFound();
            var verification = await _queries.Execute(new EmailVerificationBy(userToken.Value));
            if (verification == null) return HttpNotFound();

            // todo: verification cannot be expired, redeemed, or for different purpose

            ViewBag.Token = token;
            ViewBag.EmailAddress = verification.Owner.Value;
            return View(MVC.Security.Views.AddEmailRedeemEmailVerification);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/emails/confirm", Order = 1)]
        public virtual async Task<ActionResult> RedeemEmailVerification(RedeemEmailVerification command, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).First();
                var message = string.Format("Could not confirm email address: **{0}**", firstError);
                TempData.Alerts(message, AlertFlavor.Danger, true);
                ViewBag.Token = command.Token;
                ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.AddEmailRedeemEmailVerification, command);
            }

            await _commands.Execute(command);

            Session.VerifyEmailTickets(null);
            return this.RedirectToLocal(await MVC.User.Emails());
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/emails/reject", Order = 1)]
        public virtual async Task<ActionResult> RejectEmailOwnership(RejectEmailVerification command, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            if (ModelState.IsValid)
            {
                await _commands.Execute(command);
            }

            var message = string.Format("The email address confirmation for **{0}** was rejected.", emailAddress);
            TempData.Alerts(message, AlertFlavor.Success, true);
            Session.VerifyEmailTickets(null);
            return this.RedirectToLocal(await MVC.User.Emails());
        }

        #endregion
        #region DeleteEmailAddress

        [ValidateAntiForgeryToken]
        [HttpDelete, Route("settings/emails/{emailAddressId}")]
        public virtual async Task<ActionResult> DeleteEmailAddress(int emailAddressId, DeleteEmailAddress command)
        {
            if (command == null) return View(MVC.Errors.BadRequest());

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).First();
                var message = string.Format("Could not delete email address: **{0}**", firstError);
                TempData.Alerts(message, AlertFlavor.Danger, true);
            }
            else
            {
                var email = await _queries.Execute(new EmailAddressBy(emailAddressId));
                if (email != null)
                {
                    await _commands.Execute(command);
                    var message = string.Format("Successfully deleted email address **{0}**.", email.Value);
                    TempData.Alerts(message, AlertFlavor.Success, true);
                }
            }

            return RedirectToAction(await MVC.User.Emails());
        }

        #endregion
    }
}