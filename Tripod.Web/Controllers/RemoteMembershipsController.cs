using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class RemoteMembershipsController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public RemoteMembershipsController(IProcessQueries queries, IProcessCommands commands)
        {
            _commands = commands;
            _queries = queries;
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/disassociate")]
        public virtual async Task<ActionResult> Disassociate(DeleteRemoteMembership command)
        {
            if (ModelState.IsValid)
            {
                await _commands.Execute(command);
                return RedirectToAction(await MVC.Account.Manage(AccountController.ManageMessageId.RemoveLoginSuccess));
            }
            ViewBag.ShowRemoveButton = true;
            return View(MVC.RemoteMemberships.Views.Delete, command);
        }

        [ChildActionOnly]
        [HttpGet, Route("account/external-logins")]
        public virtual ActionResult RemoveAccountList()
        {
            var user = _queries.Execute(new UserBy(User)
            {
                EagerLoad = new Expression<Func<User, object>>[]
                {
                    x => x.LocalMembership,
                    x => x.RemoteMemberships,
                }
            }).Result;

            var linkedAccounts = user.RemoteMemberships.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey)).ToArray();
            ViewBag.ShowRemoveButton = user.LocalMembership != null || user.RemoteMemberships.Count > 1;
            return PartialView(MVC.Account.Views._RemoveAccountsPartial, linkedAccounts);
        }
    }
}