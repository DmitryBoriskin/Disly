using System.Web.Mvc;
using System.Web.Routing;

namespace Disly
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ошибка
            routes.MapRoute(
               name: "Error",
               url: "Error/{*code}",
               defaults: new { controller = "Error", action = "Custom", code = UrlParameter.Optional }
            );

            // Главная страница
            routes.MapRoute(
               name: "Index",
               url: "",
               defaults: new { controller = "Home", action = "Index" }
            );

            // Структура
            routes.MapRoute(
               name: "Structure",
               url: "Structure/",
               defaults: new { controller = "Structure", action = "Index" }
            );

            // Контакты
            routes.MapRoute(
               name: "Contacts",
               url: "Contacts/",
               defaults: new { controller = "Contacts", action = "Index" }
            );

            // Материалы
            routes.MapRoute(
               name: "PressCentrItem",
               url: "Press/{date}/{day}/{alias}",
               defaults: new { controller = "Press", action = "Item", alias = UrlParameter.Optional },
               constraints: new { date = @"\d{6}", day = @"\d{2}" }
            );
            routes.MapRoute(
               name: "PressCentr",
               url: "Press/{category}/{*path}",
               defaults: new { controller = "Press", action = "Index", category = UrlParameter.Optional, path = UrlParameter.Optional }
            );

            // Календарь (мероприятия)
            routes.MapRoute(
               name: "Calendar",
               url: "Calendar/{*action}",
               defaults: new { controller = "Calendar", action = "Index", path = UrlParameter.Optional }
            );

            // Документы
            routes.MapRoute(
               name: "Documents",
               url: "Docs/{*action}",
               defaults: new { controller = "Documents", action = "Index", path = UrlParameter.Optional }
            );



            // Обратная связь
            routes.MapRoute(
               name: "FeedBack",
               url: "FeedBack/{*action}",
               defaults: new { controller = "FeedBack", action = "Index", path = UrlParameter.Optional }
            );




            // Типовая страница (карта сайта)
            routes.MapRoute(
               name: "Page",
               url: "{*path}",
               defaults: new { controller = "Page", action = "Index", path = UrlParameter.Optional }
               //constraints: new { path = @"\d{6}" }
            );
        }
    }
}
