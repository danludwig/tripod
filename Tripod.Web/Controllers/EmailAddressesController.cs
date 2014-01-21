using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class EmailAddressesController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public EmailAddressesController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-up")]
        public virtual ViewResult SignUp()
        {
            return View(MVC.Authentication.Views.SignUp);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual async Task<ActionResult> SignUp(SendConfirmationEmail command)
        {
            if (!ModelState.IsValid) return View(MVC.Authentication.Views.SignUp, command);

            // todo: what if email matches a user account? error or redirect?

            await _commands.Execute(command);

            //return View(MVC.Authentication.Views.SignUp, command);
            Session.AddConfirmEmailTicket(command.CreatedTicket);
            return RedirectToAction(await MVC.EmailAddresses.Confirm(command.CreatedTicket));
        }

        [HttpPost, Route("sign-up/validate/{fieldName?}")]
        public virtual ActionResult SignUpValidate(SendConfirmationEmail command, string fieldName = null)
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

        [HttpGet, Route("sign-up/{ticket}")]
        public virtual async Task<ActionResult> Confirm(string ticket, string token = null)
        {
            var confirmation = await _queries.Execute(new EmailConfirmationBy(ticket, token)
            {
                EagerLoad = new Expression<Func<EmailConfirmation, object>>[]
                {
                    x => x.Owner,
                }
            });
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailConfirmationPurpose.CreatePassword;
            if (Session.ConfirmEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.Owner.Value;
            return View(MVC.Authentication.Views.Confirm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/{ticket}")]
        public virtual async Task<ActionResult> Confirm(string ticket, VerifyConfirmEmailSecret command)
        {
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailConfirmationPurpose.CreatePassword;
            if (!ModelState.IsValid) return View(MVC.Authentication.Views.Confirm, command);

            return View(MVC.Authentication.Views.Confirm, command);
        }

        [HttpPost, Route("sign-up/{ticket}/validate/{fieldName?}")]
        public virtual ActionResult ConfirmValidate(VerifyConfirmEmailSecret command, string fieldName = null)
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