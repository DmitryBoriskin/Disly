using System.Web;
using System.Web.Optimization;

namespace Disly
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // --------- Скрипты ---------            
            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                "~/Content/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/plugins/bootstrap/js/bootstrap-toggle.js",
                "~/Content/plugins/bootstrap/js/bootstrap-select.js",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.js",
                "~/Content/plugins/jquery/jquery.mask.min.js",
                "~/Content/plugins/Disly/DislyControls.js",
                "~/scripts/cms/disly_5.js"                
                ));
            
            bundles.Add(new ScriptBundle("~/bundles/popUp_js").Include(
                "~/Content/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/plugins/bootstrap/js/bootstrap-toggle.js",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.js",
                "~/Content/plugins/Disly/DislyControls.js",
                "~/Scripts/cms/disly_5_popup.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/plugins/jquery/jquery.js",
                "~/Content/plugins/jquery/jquery.ui.js"));

            //Select2 plugin
            bundles.Add(new ScriptBundle("~/bundles/select2/js").Include(
               "~/scripts/plugins/select2/select2.min.js",
               "~/scripts/plugins/select2/i18n/ru.js"));
            bundles.Add(new StyleBundle("~/bundles/select2/css").Include(
              "~/scripts/plugins/select2/css/select2.css",
              "~/scripts/plugins/select2/css/select2_custom.css"));


            // --------- Стили ---------
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/plugins/bootstrap/css/bootstrap.css",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.css",
                "~/Content/plugins/bootstrap/css/bootstrap-select.css", 
                "~/Content/plugins/Disly/DislyControls.css",
                "~/Content/css/styles.css"
                ));


            bundles.Add(new StyleBundle("~/bundles/popUp_css").Include(
                "~/Content/plugins/bootstrap/css/bootstrap.min.css",
                "~/Content/plugins/mCustomScrollbar/jquery.mCustomScrollbar.css",
                "~/Content/plugins/bootstrap/css/bootstrap-select.css", 
                "~/Content/plugins/Disly/DislyControls.css",
                "~/Content/css/styles_popUp.css"));

        }
    }
}
