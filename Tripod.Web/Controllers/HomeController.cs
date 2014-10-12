using System.Web.Mvc;

namespace Tripod.Web.Controllers
{
    public partial class HomeController : Controller
    {
        [HttpGet, Route("")]
        public virtual ViewResult Index()
        {
            return View();
        }

        [HttpGet, Route("about")]
        public virtual ViewResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet, Route("contact")]
        public virtual ViewResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Permit(Roles = "nooneisinthisrole")]
        [HttpGet, Route("admin")]
        public virtual ViewResult Admin()
        {
            return View();
        }
    }
}