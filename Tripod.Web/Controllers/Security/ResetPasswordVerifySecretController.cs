using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class ResetPasswordVerifySecretController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public ResetPasswordVerifySecretController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-in/password/reset/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket, string returnUrl)
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
            ViewBag.ActionUrl = Url.Action(MVC.ResetPasswordVerifySecret.Post());
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailVerificationPurpose.ForgotPassword;
            if (Session.VerifyEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.EmailAddress.Value;
            return View(MVC.Security.Views.ResetPassword.VerifySecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password/reset/{ticket}")]
        public virtual async Task<ActionResult> Post(string ticket, VerifyEmailSecret command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.ResetPasswordVerifySecret.Post());
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailVerificationPurpose.ForgotPassword;
                if (Session.VerifyEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.ResetPassword.VerifySecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.ResetPassword.Index(command.Token, ticket, returnUrl));
        }
    }
}