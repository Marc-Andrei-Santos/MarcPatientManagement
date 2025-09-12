using System.Web;
using System.Web.Optimization;

namespace AL
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/lib/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                 "~/Scripts/lib/jquery.validate*",
                 "~/Scripts/lib/jquery.validate.unobtrusive*"));

            // Modernizr
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-"));

            // Bootstrap JS
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/lib/bootstrap.js"));

            // Bootstrap CSS
            bundles.Add(new StyleBundle("~/Content/CSS/bootstrap").Include(
                "~/Content/CSS/bootstrap/bootstrap.css"));

            // Site CSS
            bundles.Add(new StyleBundle("~/Content/CSS/site").Include(
                "~/Content/CSS/site/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/medication").Include(
                "~/Scripts/Page/Medication/validation.js"));
        }
    }
}
