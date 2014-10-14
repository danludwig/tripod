using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class UserEmailConfirmController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public UserEmailConfirmController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("settings/confirm")]
        public virtual async Task<ActionResult> Index(string token, string ticket)
        {
            var verification = await _queries.Execute(new EmailVerificationBy(ticket));
            if (verification == null) return HttpNotFound();

            // todo: verification cannot be expired, redeemed, or for different purpose

            ViewBag.EmailAddress = verification.EmailAddress.Value;
            ViewBag.Ticket = ticket;
            ViewBag.Token = token;
            return View(MVC.Security.Views.User.AddEmailConfirm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/confirm")]
        public virtual async Task<ActionResult> Post(RedeemEmailVerification command, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).First();
                var message = string.Format("Could not confirm email address: **{0}**", firstError);
                TempData.Alerts(message, AlertFlavor.Danger, true);
                ViewBag.EmailAddress = emailAddress;
                ViewBag.Ticket = command.Ticket;
                ViewBag.Token = command.Token;
                return View(MVC.Security.Views.User.AddEmailConfirm, command);
            }

            await _commands.Execute(command);

            Session.VerifyEmailTickets(null);
            return this.RedirectToLocal(await MVC.UserEmails.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/emails/reject")]
        public virtual async Task<ActionResult> Reject(RejectEmailVerification command, string emailAddress)
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
            return this.RedirectToLocal(await MVC.UserEmails.Index());
        }
    }
}