using System.Web.Optimization;

namespace Tripod.Web
{
    public static class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
#if DEBUG
            //BundleTable.EnableOptimizations = true;
#else
            //BundleTable.EnableOptimizations = false;
#endif

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/scripts/bootstrap.js",
            //          "~/scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bs").Include(
                      "~/scripts/bootstrap.js",
                      "~/scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/ng").Include(
                      "~/scripts/ui-bootstrap-tpls-{version}.js",
                      "~/app/_common/directives/InputPreFormatter.js",
                      "~/app/_common/directives/RemoveCssClass.js",
                      "~/app/_common/directives/TooltipToggle.js",
                      "~/app/_common/directives/FormContrib.js",
                      "~/app/_common/directives/ModelContrib.js",
                      "~/app/_common/directives/ServerError.js",
                      "~/app/_common/directives/ServerValidate.js",
                      "~/app/_common/directives/SubmitAction.js",
                      "~/app/_common/directives/MustEqual.js",
                      "~/app/_common/modules/Tripod.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                      "~/content/bootstrap.css",
                      "~/content/font-awesome.css",
                      "~/content/site.css"));
        }
    }
}
