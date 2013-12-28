using System.Web.Mvc;

namespace Tripod.Web
{
    public class SecuredCommandModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type modelType)
        {
            var baseModel = base.CreateModel(controllerContext, bindingContext, modelType);
            var commandModel = baseModel as IDefineSecuredCommand;
            if (commandModel != null) commandModel.Principal = controllerContext.HttpContext.User;
            return baseModel;
        }
    }
}