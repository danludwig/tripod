using System;
using System.Web.Mvc;

namespace Tripod.Web.Controllers
{
    [RoutePrefix("errors")]
    public partial class ErrorsController : Controller
    {
        [Route("unexpected"), Route("500"), Route("")]
        public virtual ActionResult Unexpected()
        {
            return View(MVC.Shared.Views.Error);
        }

        [Route("throw")]
        public virtual ActionResult Throw()
        {
            var ex = new Exception("This is a test exception thrown on purpose from the web server.");
            throw ex;
        }

        [Route("400")]
        public virtual ActionResult BadRequest()
        {
            return View();
        }

        [Route("403")]
        public virtual ActionResult Forbidden()
        {
            return View();
        }

        [Route("404")]
        public virtual ActionResult NotFound()
        {
            return View();
        }
    }
}