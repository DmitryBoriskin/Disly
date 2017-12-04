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

            routes.MapRoute(
                name: "Registry",
                url: "RedirectRegistry/{action}/{id}",
                defaults: new { controller = "RedirectRegistry", action = "Index", id = UrlParameter.Optional }
            );

            // карта сайта
            routes.MapRoute(
               name: "MapSite",
               url: "MapSite/",
               defaults: new { controller = "MapSite", action = "Index" }
            );
            // поиск
            routes.MapRoute(
               name: "Search",
               url: "Search/",
               defaults: new { controller = "Search", action = "Index" }
            );

            // лпу
            routes.MapRoute(
               name: "LPU",
               url: "lpu/{action}/{*id}",
               defaults: new { controller = "LPU", action = "Index", id = UrlParameter.Optional }
            );

            // врачи портала
            routes.MapRoute(
               name: "PortalDoctors",
               url: "PortalDoctors/",
               defaults: new { controller = "PortalDoctors", action = "Index"}
            );

            routes.MapRoute(
               name: "PortalDoctorsItem",
               url: "PortalDoctors/{id}",
               defaults: new { controller = "Doctors", action = "Item" }
            );

            // медицинские услуги портала
            routes.MapRoute(
               name: "MedicalServices",
               url: "MedicalServices/",
               defaults: new { controller = "MedicalServices", action = "Index", id = UrlParameter.Optional }
            );

            // перенаправление
            routes.MapRoute(
               name: "Redirect",
               url: "Redirect/{action}/{*id}",
               defaults: new { controller = "Redirect", action = "Index", id = UrlParameter.Optional }
            );

            //голосование
            routes.MapRoute(
               name: "vote",
               url: "vote/",
               defaults: new { controller = "vote", action = "Index" },
               namespaces: new string[] { "Disly.Controllers" }
            );
            routes.MapRoute(
               name: "voteitem",
               url: "vote/{id}",
               defaults: new { controller = "vote", action = "Item" },
               namespaces: new string[] { "Disly.Controllers" }
            );
            routes.MapRoute(
               name: "givevote",
               url: "vote/givevote/{id}",
               defaults: new { controller = "vote", action = "givevote" },
               namespaces: new string[] { "Disly.Controllers" }
            );


            // Структура
            routes.MapRoute(
               name: "Structure",
               url: "Structure/",
               defaults: new { controller = "Structure", action = "Index" }
            );
            routes.MapRoute(
               name: "StructureItem",
               url: "Structure/{num}",
               defaults: new { controller = "Structure", action = "Item" }
            );
            routes.MapRoute(
               name: "StructureDepartItem",
               url: "Structure/{num}/{id}",
               defaults: new { controller = "Structure", action = "Department" }
            );

            //Врачи
            routes.MapRoute(
               name: "Doctors",
               url: "Doctors/",
               defaults: new { controller = "Doctors", action = "Index" }
            );

            routes.MapRoute(
               name: "DoctorsItem",
               url: "Doctors/{id}",
               defaults: new { controller = "Doctors", action = "Item" }
            );

            // события
            routes.MapRoute(
               name: "EventsItem",
               url: "Events/{num}/{alias}",
               defaults: new { controller = "EventsFront", action = "Item", alias = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Events",
               url: "Events/{action}",
               defaults: new { controller = "EventsFront", action = "Index" }
            );

            // Контакты
            routes.MapRoute(
               name: "contacts",
               url: "contacts/",
               defaults: new { controller = "Contacts", action = "Index" }
            );

            // как нас найти findus
            routes.MapRoute(
               name: "findus",
               url: "findus/",
               defaults: new { controller = "findus", action = "Index" }
            );

            // Материалы
            routes.MapRoute(
               name: "PressCentrItem",
               url: "Press/{year}/{month}/{day}/{alias}",
               defaults: new { controller = "Press", action = "Item", alias = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "PressCentrRss",
               url: "Press/Rss",
               defaults: new { controller = "Press", action = "Rss", alias = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "PressCentrCategory",
               url: "Press/{category}",
               defaults: new { controller = "Press", action = "Category", alias = UrlParameter.Optional }
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

            //Обратная связь
            routes.MapRoute(
             name: "Feedback",
             url: "Feedback/{*action}",
             defaults: new { controller = "Feedback", action = "Index", path = UrlParameter.Optional }
             );
            routes.MapRoute(
            name: "FeedbackItem",
            url: "Feedback/{id}",
            defaults: new { controller = "Feedback", action = "Item", path = UrlParameter.Optional }
            );

            // Типовая страница (карта сайта)
            routes.MapRoute(
               name: "Page",
               url: "{*path}",
               defaults: new { controller = "Page", action = "Index", path = UrlParameter.Optional }
               //constraints: new { path = @"\d{6}" }
            );
            
            routes.MapRoute(
                name: "Service",
                url: "Service/{action}/{*id}",
                defaults: new { controller = "Service", action = "Index", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
