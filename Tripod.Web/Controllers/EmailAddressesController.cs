using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class EmailAddressesController : Controller
    {
        [HttpGet, Route("sign-up")]
        public virtual ViewResult SignUp()
        {
            return View(MVC.Authentication.Views.SignUp);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual ActionResult SignUp(SendConfirmationEmail command)
        {
            // todo: what if email matches a user account? error or redirect?
            //var isValid = ModelState.IsValid;
            return View(MVC.Authentication.Views.SignUp, command);
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
    }
}