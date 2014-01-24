using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignUpStep2Controller : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public SignUpStep2Controller(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-up/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket)
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

            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailConfirmationPurpose.CreateLocalUser;
            if (Session.ConfirmEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.Owner.Value;
            return View(MVC.Authentication.Views.Confirm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket, VerifyConfirmEmailSecret command, string emailAddress = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailConfirmationPurpose.CreateLocalUser;
                if (Session.ConfirmEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Authentication.Views.Confirm, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.EmailAddresses.Password(command.Token));
        }

        [HttpPost, Route("sign-up/{ticket}/validate/{fieldName?}")]
        public virtual ActionResult Validate(VerifyConfirmEmailSecret command, string fieldName = null)
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
    }
}
