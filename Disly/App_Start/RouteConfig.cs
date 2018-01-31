using System.Web.Mvc;
using System.Web.Routing;

namespace Disly
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //images
            routes.IgnoreRoute("{*jpgfiles}", new { jpgfiles = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*jpegfiles}", new { jpegfiles = @".*\.jpeg(/.*)?" });
            routes.IgnoreRoute("{*pngfiles}", new { pngfiles = @".*\.png(/.*)?" });
            routes.IgnoreRoute("{*giffiles}", new { pngfiles = @".*\.gif(/.*)?" });
            //docs
            routes.IgnoreRoute("{*docfiles}", new { pngfiles = @".*\.doc(/.*)?" });
            routes.IgnoreRoute("{*docxfiles}", new { pngfiles = @".*\.docx(/.*)?" });
            //routes.IgnoreRoute("{*docfiles}", new { pngfiles = @".*\.doc(/.*)?" });

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

            // перенаправление
            routes.MapRoute(
               name: "Redirect",
               url: "Redirect/{action}/{*id}",
               defaults: new { controller = "Redirect", action = "Index", id = UrlParameter.Optional }
            );

            //Редирект со старых сайтов
            routes.MapRoute(
              name: "RedirectFromOld1",
              url: "pg_{pg}/id_{id}/{action}.aspx",
              defaults: new { controller = "RedirectFromOld", action = "Index", id = UrlParameter.Optional, pg = UrlParameter.Optional }
           );
            routes.MapRoute(
             name: "RedirectFromOld2",
             url: "id_{id}/{action}.aspx",
             defaults: new { controller = "RedirectFromOld", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
               name: "RedirectFromOld",
               url: "{action}.aspx",
               defaults: new { controller = "RedirectFromOld", action = "Index" }
            );

            routes.MapRoute(
                name: "Registry",
                url: "RedirectRegistry/{action}/{id}",
                defaults: new { controller = "RedirectRegistry", action = "Index", id = UrlParameter.Optional }
            );

            // карта сайта
            routes.MapRoute(
               name: "MapSite",
               url: "MapSite/{action}",
               defaults: new { controller = "MapSite", action = "Index" }
            );
            // Вакансии
            routes.MapRoute(
               name: "vacancy",
               url: "vacancy/",
               defaults: new { controller = "vacancy", action = "Index" },
               namespaces: new string[] { "Disly.Controllers" }
            );
            routes.MapRoute(
               name: "vacancyItem",
               url: "vacancy/{id}",
               defaults: new { controller = "vacancy", action = "Item" },
               namespaces: new string[] { "Disly.Controllers" }
            );
            // поиск
            routes.MapRoute(
               name: "Search",
               url: "Search/",
               defaults: new { controller = "Search", action = "Index" }
            );
            // поиск
            routes.MapRoute(
               name: "Photolist",
               url: "photolist/{id}",
               defaults: new { controller = "Service", action = "Photolist", id = UrlParameter.Optional }
            );

            // лпу
            routes.MapRoute(
               name: "LPU",
               url: "lpu/{action}/{*id}",
               defaults: new { controller = "LPU", action = "Index", id = UrlParameter.Optional }
            );

            // медицинские услуги портала
            routes.MapRoute(
               name: "MedicalServices",
               url: "MedicalServices/",
               defaults: new { controller = "MedicalServices", action = "Index", id = UrlParameter.Optional }
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

            // врачи портала
            routes.MapRoute(
               name: "PortalDoctors",
               url: "PortalDoctors/",
               defaults: new { controller = "PortalDoctors", action = "Index" }
            );

            routes.MapRoute(
               name: "PortalDoctorsItem",
               url: "PortalDoctors/{id}",
               defaults: new { controller = "PortalDoctors", action = "Item" }
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

            // Структура в гс
            routes.MapRoute(
              name: "SpecStructure",
              url: "SpecStructure/",
              defaults: new { controller = "SpecStructure", action = "Index" }
           );

            // Главные специалисты на портале
            routes.MapRoute(
               name: "PortalGsSites",
               url: "PortalGsSites/",
               defaults: new { controller = "PortalGsSites", action = "Index" }
            );
            routes.MapRoute(
              name: "OrgGsMembers",
              url: "OrgGsMembers/",
              defaults: new { controller = "OrgGsMembers", action = "Index" }
           );

            // события
            routes.MapRoute(
               name: "Events",
               url: "Events/{action}",
               defaults: new { controller = "EventsFront", action = "Index" }
            );
            routes.MapRoute(
               name: "EventsItem",
               url: "Events/{num}/{alias}",
               defaults: new { controller = "EventsFront", action = "Item", alias = UrlParameter.Optional }
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
               name: "PressCentrRssSettings",
               url: "press/RssSettings",
               defaults: new { controller = "Press", action = "RssSettings", alias = UrlParameter.Optional }
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

            // Обратная связь
            routes.MapRoute(
             name: "Feedback",
             url: "Feedback/{*action}",
             defaults: new { controller = "Feedback", action = "Index", path = UrlParameter.Optional },
             constraints: new { controller = "Feedback", action = "^Appeallist$|^Reviewlist$|^SendForm$|^AnswerForm$" } //Restriction for controller and action
             );

            // Анкета
            routes.MapRoute(
               name: "Anketa",
               url: "Feedback/Anketa/{*action}",
               defaults: new { controller = "Anketa", action = "Index", path = UrlParameter.Optional }
            );

            // Типовая страница (карта сайта)
            routes.MapRoute(
               name: "Page",
               url: "{*path}",
               defaults: new { controller = "Page", action = "Index", path = UrlParameter.Optional }
              // constraints: new { path = @"\d{6}" }
            );

            
            routes.MapRoute(
                name: "Service",
                url: "Service/{action}/{*id}",
                defaults: new { controller = "Service", action = "Index", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }//,
                //constraints: new { path = @"\d{6}" }
            );

            
        }
    }
}
