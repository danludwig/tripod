using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Tripod.Domain.Guestbook;

namespace Tripod.Web.ApiControllers
{
    [RoutePrefix("api")]
    public class GuestbookController : ApiController
    {
        private readonly IProcessValidation _validation;
        private readonly IProcessCommands _commands;

        public GuestbookController(IProcessValidation validation, IProcessCommands commands)
        {
            _validation = validation;
            _commands = commands;
        }

        [HttpPost, Route("guestbook")]
        public async Task<HttpResponseMessage> Post(CreateGuestbookEntry command)
        {
            var validation = _validation.Validate(command);
            if (!validation.IsValid)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.Content = new StringContent(validation.Errors.First().ErrorMessage,
                    Encoding.UTF8, "text/plain");
                return response;
            }

            await _commands.Execute(command);
            return Request.CreateResponse(HttpStatusCode.OK,
                "The command completed successfully");
        }
    }
}
