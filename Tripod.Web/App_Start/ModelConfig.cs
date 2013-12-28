using System.Web.Mvc;

namespace Tripod.Web
{
    public static class ModelConfig
    {
        public static void Configure()
        {
            ModelBinders.Binders.DefaultBinder = new SecuredCommandModelBinder();
        }
    }
}