using System.Web;
using System.Web.Optimization;

namespace WebSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angularJs").Include(
                        "~/Scripts/lodash.js",
                        "~/Scripts/angular.js",
                        "~/Scripts/ngRoute.js",
                        "~/Scripts/restangular.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/core")
               .Include(
               "~/Scripts/ngApp/hucksterApp.js",
               "~/Scripts/ngApp/huckster.orderForm.js",
               "~/Scripts/ngApp/huckster.orderFinalise.js"
               ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/normalize.css",
                      "~/Content/bootstrap.css",
                      "~/Content/style.css",
                      "~/fonts/stylesheet.css"));
        }
    }
}
