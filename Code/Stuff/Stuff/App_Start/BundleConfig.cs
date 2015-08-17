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
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.mask.min.js"
                        ));
            
            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство построения на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      //"~/Scripts/BootstrapValidator/language/ru_RU.js",
                      //"~/Scripts/bootstrapValidator.js",
                      "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/bootstrap-datepicker.ru.js",
                        "~/Scripts/bootstrap-timepicker.js",
                      "~/Scripts/validator.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                        "~/Scripts/site.js"));
            //bundles.Add(new ScriptBundle("~/bundles/fuelux").Include(
            //            "~/Scripts/fuelux.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/2pdf").Include(
                "~/Scripts/jquery-{version}.js",
                        "~/Scripts/pdf/jspdf.js",
                        "~/Scripts/pdf/jspdf.plugin.addimage.js",
                        "~/Scripts/pdf/jspdf.plugin.from_html.js",
                        "~/Scripts/pdf/jspdf.plugin.split_text_to_size.js",
                        "~/Scripts/pdf/jspdf.plugin.standard_fonts_metrics.js"
                        //, "~/Scripts/pdf.worker.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-theme.css",
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap-datepicker3.css",
                      "~/Content/bootstrap-timepicker.min.css",
                      //"~/Content/bootstrapValidator.css",
                      //"~/Content/fuelux.min.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/statement").Include(
                "~/Content/sattement.css"));

            bundles.Add(new StyleBundle("~/Content/birthday").Include(
                "~/Content/birthday.css"));

            //Views
            bundles.Add(new ScriptBundle("~/bundles/Department/Index").Include(
                      "~/Scripts/Views/Department/Index.js"));


            
        }
    }
}
