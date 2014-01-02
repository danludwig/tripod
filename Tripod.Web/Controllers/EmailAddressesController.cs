using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class EmailAddressesController : Controller
    {
        [HttpGet, Route("account/register2")]
        public virtual ViewResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/register2")]
        public virtual ActionResult SendEmail(SendConfirmationEmail command)
        {
            var isValid = ModelState.IsValid;
            return View(MVC.EmailAddresses.Views.Index, command);
        }

        [HttpPost, Route("account/register2/validate")]
        public virtual ActionResult ValidateSendEmail(SendConfirmationEmail command)
        {
            Response.StatusCode = 400;
            return Content("Here is a custom error message for Email address.", "application/json");
            //return Json(new
            //{
            //    Field = "EmailAddy",
            //    Message = "Here is a custom error message for Email address.",
            //});
        }

        //[HttpPost, Route("account/register2/validate/{field?}")]
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