using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class UserEmailVerifySecretController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public UserEmailVerifySecretController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("settings/confirm/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket)
        {
            var verification = await _queries.Execute(new EmailVerificationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailVerification, object>>[]
                {
                    x => x.EmailAddress,
                }
            });
            if (verification == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.ActionUrl = Url.Action(MVC.UserEmailVerifySecret.Post());
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailVerificationPurpose.AddEmail;
            if (Session.VerifyEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = verification.EmailAddress.Value;
            return View(MVC.Security.Views.User.AddEmailVerifySecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/confirm/{ticket}")]
        public virtual async Task<ActionResult> Post(string ticket, VerifyEmailSecret command, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ActionUrl = Url.Action(MVC.UserEmailVerifySecret.Post());
                ViewBag.Ticket = command.Ticket;
                ViewBag.Purpose = EmailVerificationPurpose.AddEmail;
                if (Session.VerifyEmailTickets().Contains(command.Ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.User.AddEmailVerifySecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.UserEmailConfirm.Index(command.Token, command.Ticket));
        }
    }
}