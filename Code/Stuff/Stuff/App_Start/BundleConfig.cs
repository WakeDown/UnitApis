using System.Web;
using System.Web.Optimization;

namespace Stuff
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. по адресу: http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        //"~/Scripts/jquery.d.js",
                        "~/Scripts/jquery.mask.min.js"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство построения на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      //"~/Scripts/BootstrapValidator/language/ru_RU.js",
                      //"~/Scripts/bootstrapValidator.js",
                      "~/Scripts/validator.js",
                      "~/Scripts/respond.js"));

            //bundles.Add(new ScriptBundle("~/bundles/fuelux").Include(
            //            "~/Scripts/fuelux.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                      "~/Content/font-awesome.css",
                      //"~/Content/bootstrapValidator.css",
                      //"~/Content/fuelux.min.css",
                      "~/Content/site.css"));

            //Views
            bundles.Add(new ScriptBundle("~/bundles/Department/Index").Include(
                      "~/Scripts/Views/Department/Index.js"));
        }
    }
}
