using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class UserController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public UserController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        #region ChangeUserName (default form)

        [HttpGet, Route("settings")]
        public virtual async Task<ActionResult> SettingsIndex()
        {
            var view = await _queries.Execute(new UserViewBy(User.Identity.GetAppUserId()));
            if (view == null) return HttpNotFound();

            var model = new ChangeUserNameModel
            {
                UserView = view,
                Command = new ChangeUserName
                {
                    UserId = view.UserId,
                    UserName = view.UserName,
                },
            };

            return View(MVC.Security.Views.UserSettingsIndex, model);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPut, Route("settings/username")]
        public virtual async Task<ActionResult> ChangeUserName(ChangeUserName command)
        {
            if (command == null) return View(MVC.Errors.BadRequest());

            var view = await _queries.Execute(new UserViewBy(command.UserId));
            if (view == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                var model = new ChangeUserNameModel
                {
                    UserView = view,
                    Command = command,
                };

                return View(MVC.Security.Views.UserSettingsIndex, model);
            }

            await _commands.Execute(command);
            Response.ClientCookie(command.SignedIn.Id, _queries);
            return RedirectToAction(await MVC.User.SettingsIndex());
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/username/validate")]
        public virtual ActionResult ValidateChangeUserName(ChangeUserName command)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, command.PropertyName(x => x.UserName));

            return new CamelCaseJsonResult(result);
        }

        #endregion
        #region EmailAddresses

        [Authorize]
        [HttpGet, Route("settings/emails")]
        public virtual async Task<ActionResult> Emails()
        {
            var user = await _queries.Execute(new UserViewBy(User.Identity.GetAppUserId()));
            var emails = await _queries.Execute(new EmailAddressViewsBy(User.Identity.GetAppUserId())
            {
                OrderBy = new Dictionary<Expression<Func<EmailAddressView, object>>, OrderByDirection>
                {
                    { x => x.IsPrimary, OrderByDirection.Descending },
                    { x => x.IsVerified, OrderByDirection.Descending },
                },
            });

            var model = new EmailAddressSettingsModel
            {
                UserView = user,
                EmailAddresses = emails.ToArray(),
            };

            return View(MVC.Security.Views.UserEmailAddresses, model);
        }

        #endregion
    }
}