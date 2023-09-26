using System.Web;
using System.Web.Optimization;

namespace EcommerceASP
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        //"~/UI/slider/splide.min.js",
                        "~/UI/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/UI/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/UI/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/UI/bootstrap-4.3.1/js/bootstrap.bundle.min.js",
                      "~/UI/bootstrap-4.3.1/js/bootstrap.min.js"));

            //bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
            //          "~/UI/bootstrap-4.3.1/css/bootstrap.min.css"));

            //Ajax
            bundles.Add(new ScriptBundle("~/UI/microsoft/ajax/javascripts").Include(
                         "~/UI/jquery.unobtrusive-ajax.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));

            //slim Scroll
            bundles.Add(new ScriptBundle("~/UI/slimScroll/javascripts").Include(
                "~/UI/slimScroll/jquery.slimscroll.min.js"));

            // Layout CSS Page
            bundles.Add(new StyleBundle("~/Content/themes/regular/css").Include(
               //"~/Content/layout.css",
               "~/Content/paging.css"));

            //Loading CSS
            bundles.Add(new StyleBundle("~/Content/themes/regular/loading").Include(
                        "~/Content/loading.css"));

            //Slider CSS
            bundles.Add(new StyleBundle("~/UI/slider/css").Include(
               "~/UI/slider/splide.min.css"
               ));

            //Slider JS
            bundles.Add(new ScriptBundle("~/UI/slider/js").Include(
                "~/UI/slider/splide.min.js"
                ));

            //DatePicker
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datepicker/js").Include(
                "~/UI/bootstrap-datepicker/js/bootstrap-datepicker.min.js"));
            //DatePicker CSS
            bundles.Add(new StyleBundle("~/bundles/bootstrap-datepicker/css").Include(
                        "~/UI/bootstrap-datepicker/css/bootstrap-datepicker3.css"));
            //TimePicker
            bundles.Add(new ScriptBundle("~/bundles/timepicker/js").Include(
                "~/UI/bootstrap-timepicker/js/bootstrap-timepicker.min.js"));
            //TimePicker CSS
            bundles.Add(new StyleBundle("~/bundles/timepicker/css").Include(
                        "~/UI/bootstrap-timepicker/css/bootstrap-timepicker.min.css"));

            //select2 CSS
            bundles.Add(new StyleBundle("~/UI/select2/css").Include(
               "~/UI/selected/select2.css"));

            //select2 JS
            bundles.Add(new ScriptBundle("~/UI/select2/js").Include(
                "~/UI/selected/select2.full.js"
                ));

            //fancybox_fancybox JS
            bundles.Add(new ScriptBundle("~/UI/fancybox_fancybox/js").Include(
                "~/UI/fancybox_fancybox/fancybox_fancybox.js"
                ));

            //fancybox_fancybox CSS
            bundles.Add(new StyleBundle("~/UI/fancybox_fancybox/css").Include(
               "~/UI/fancybox_fancybox/fancybox_fancybox.css"
               ));
        }
    }
}