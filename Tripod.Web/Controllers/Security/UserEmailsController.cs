using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    [Authorize]
    public partial class UserEmailsController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public UserEmailsController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("settings/emails")]
        public virtual async Task<ActionResult> Index()
        {
            var user = await _queries.Execute(new UserViewBy(User.Identity.GetUserId<int>()));
            var emails = await _queries.Execute(new EmailAddressViewsBy(User.Identity.GetUserId<int>())
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
                    SendFromUrl = Url.AbsoluteAction(Request.Url, await MVC.UserEmails.Index()),
                    VerifyUrlFormat = VerifyUrlFormat(),
                },
            };

            ViewBag.ActionUrl = Url.Action(MVC.UserEmails.Post());
            ViewBag.Purpose = model.SendVerificationEmail.Purpose;
            return View(MVC.Security.Views.User.EmailAddresses, model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/emails")]
        public virtual async Task<ActionResult> Post(SendVerificationEmail command)
        {
            if (command == null || command.Purpose == EmailVerificationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                var user = await _queries.Execute(new UserViewBy(User.Identity.GetUserId<int>()));
                var emails = await _queries.Execute(new EmailAddressViewsBy(User.Identity.GetUserId<int>())
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
                ViewBag.ActionUrl = Url.Action(MVC.UserEmails.Post());
                return View(MVC.Security.Views.User.EmailAddresses, model);
            }

            command.VerifyUrlFormat = VerifyUrlFormat();
            command.SendFromUrl = Url.AbsoluteAction(Request.Url, await MVC.UserEmails.Index());
            await _commands.Execute(command);

            Session.VerifyEmailTickets(command.CreatedTicket);

            return RedirectToAction(await MVC.UserEmailVerifySecret.Index(command.CreatedTicket));
        }

        private string VerifyUrlFormat(string returnUrl = null)
        {
            Debug.Assert(Request.Url != null);
            var encodedUrlFormat = Url.Action(MVC.SignUpCreateUser.Index("{0}", "{1}", "{2}"));
            var decodedUrlFormat = HttpUtility.UrlDecode(encodedUrlFormat);
            Debug.Assert(decodedUrlFormat != null);
            var formattedUrl = string.Format(decodedUrlFormat, "{0}", "{1}", returnUrl);
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, formattedUrl);
        }

        [ValidateAntiForgeryToken]
        [HttpPut, Route("settings/emails/{emailAddressId}")]
        public virtual async Task<ActionResult> Put(int emailAddressId, UpdateEmailAddress command)
        {
            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).First();
                var message = string.Format("Could not update email address: **{0}**", firstError);
                TempData.Alerts(message, AlertFlavor.Danger, true);
            }
            else
            {
                var email = await _queries.Execute(new EmailAddressBy(emailAddressId));
                if (email != null)
                {
                    await _commands.Execute(command);
                    var message = string.Format("Successfully updated email address **{0}**.", email.Value);
                    TempData.Alerts(message, AlertFlavor.Success, true);

                    // changing the primary email address also changes gravatar
                    if (email.IsPrimary != command.IsPrimary)
                        Response.ClientCookie(User.Identity.GetUserId(), _queries);
                }
            }

            return RedirectToAction(await MVC.UserEmails.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpDelete, Route("settings/emails/{emailAddressId}")]
        public virtual async Task<ActionResult> Delete(int emailAddressId, DeleteEmailAddress command)
        {
            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).First();
                var message = string.Format("Could not remove email address: **{0}**", firstError);
                TempData.Alerts(message, AlertFlavor.Danger, true);
            }
            else
            {
                var email = await _queries.Execute(new EmailAddressBy(emailAddressId));
                if (email != null)
                {
                    await _commands.Execute(command);
                    var message = string.Format("Successfully removed email address **{0}**.", email.Value);
                    TempData.Alerts(message, AlertFlavor.Success, true);
                }
            }

            return RedirectToAction(await MVC.UserEmails.Index());
        }
    }
}