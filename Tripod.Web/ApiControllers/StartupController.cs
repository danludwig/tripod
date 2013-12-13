using System.Web.Http;

namespace Tripod.Web.ApiControllers
{
    [RoutePrefix("api")]
    public class StartupController : ApiController
    {
        [HttpGet, Route("startup/{id?}", Name = "Startup.Get")]
        public string Get(string id = null)
        {
            return string.IsNullOrWhiteSpace(id)
                ? Url.Route("Startup.Get", null)
                : Url.Route("Startup.Get", new { id });
        }
    }
}
