using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class EmailAddressesController : Controller
    {
        [HttpGet, Route("sign-up")]
        public virtual ViewResult SignUp()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual ActionResult SignUp(SendConfirmationEmail command)
        {
            //var isValid = ModelState.IsValid;
            return View(MVC.EmailAddresses.Views.SignUp, command);
        }

        [HttpPost, Route("sign-up/validate")]
        public virtual ActionResult ValidateSignUp(SendConfirmationEmail command)
        {
            Response.StatusCode = 400;
            return Content("Here is a custom error message for Email address.", "application/json");
            //return Json(new
            //{
            //    Field = "EmailAddy",
            //    Message = "Here is a custom error message for Email address.",
            //});
        }

        //[HttpPost, Route("sign-up/validate/{field?}")]
        //public virtual ActionResult ValidateSendEmail(SendConfirmationEmail command, string field = null)
        //{
        //    var key = ModelState.Keys.FirstOrDefault(x => x.Equals(field));
        //    var isValid = !string.IsNullOrWhiteSpace(key) ? ModelState.IsValidField(key) : ModelState.IsValid;
        //    if (!isValid)
        //    {
        //        Response.StatusCode = 400;
        //        return Content("Here is a custom error message for Email address.", "application/json");
        //    }
        //    return Json(true);
        //}
    }
}