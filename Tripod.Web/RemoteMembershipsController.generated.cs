// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace Tripod.Web.Controllers
{
    public partial class RemoteMembershipsController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RemoteMembershipsController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Threading.Tasks.Task<System.Web.Mvc.ActionResult> Disassociate()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Disassociate);
            return System.Threading.Tasks.Task.FromResult(callInfo as ActionResult);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public RemoteMembershipsController Actions { get { return MVC.RemoteMemberships; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "RemoteMemberships";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "RemoteMemberships";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Disassociate = "Disassociate";
            public readonly string RemoveAccountList = "RemoveAccountList";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Disassociate = "Disassociate";
            public const string RemoveAccountList = "RemoveAccountList";
        }


        static readonly ActionParamsClass_Disassociate s_params_Disassociate = new ActionParamsClass_Disassociate();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Disassociate DisassociateParams { get { return s_params_Disassociate; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Disassociate
        {
            public readonly string command = "command";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _AddForm = "_AddForm";
                public readonly string _DeleteForm = "_DeleteForm";
                public readonly string _SignOnForm = "_SignOnForm";
                public readonly string _SignOnList = "_SignOnList";
                public readonly string _Table = "_Table";
                public readonly string Delete = "Delete";
            }
            public readonly string _AddForm = "~/Views/RemoteMemberships/_AddForm.cshtml";
            public readonly string _DeleteForm = "~/Views/RemoteMemberships/_DeleteForm.cshtml";
            public readonly string _SignOnForm = "~/Views/RemoteMemberships/_SignOnForm.cshtml";
            public readonly string _SignOnList = "~/Views/RemoteMemberships/_SignOnList.cshtml";
            public readonly string _Table = "~/Views/RemoteMemberships/_Table.cshtml";
            public readonly string Delete = "~/Views/RemoteMemberships/Delete.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_RemoteMembershipsController : Tripod.Web.Controllers.RemoteMembershipsController
    {
        public T4MVC_RemoteMembershipsController() : base(Dummy.Instance) { }

        partial void DisassociateOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, Tripod.Domain.Security.DeleteRemoteMembership command);

        public override System.Threading.Tasks.Task<System.Web.Mvc.ActionResult> Disassociate(Tripod.Domain.Security.DeleteRemoteMembership command)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Disassociate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "command", command);
            DisassociateOverride(callInfo, command);
            return System.Threading.Tasks.Task.FromResult(callInfo as ActionResult);
        }

        partial void RemoveAccountListOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        public override System.Web.Mvc.ActionResult RemoveAccountList()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.RemoveAccountList);
            RemoveAccountListOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
