using System.Web;
using System.Web.Optimization;

namespace Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/wickedpicker.js",
                        "~/Scripts/jquery.timepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularJs").Include(
            "~/Scripts/lodash.js",
            "~/Scripts/angular.js",
            "~/Scripts/ngRoute.js",
            "~/Scripts/restangular.js",
            "~/Scripts/angular-drag-and-drop-lists.js",
            "~/Scripts/ui-bootstrap-custom-0.14.3.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/core")
           .Include(
           "~/Scripts/ngApp/hucksterAdminApp.js",
           "~/Scripts/ngApp/huckster.menuEdit.js",
           "~/Scripts/ngApp/huckster.suburbEdit.js",
           "~/Scripts/ngApp/huckster.deliveryHoursEdit.js"
           ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery.timepicker.css",
                      "~/Content/wickedpicker.css",
                      "~/Content/site.css"));
        }
    }
}
