using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class ContactsController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private ContatcsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new ContatcsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs= breadcrumb,
                BannerArray = bannerArray
            };
        }


        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string t)
        {
            #region Получаем данные из адресной строки
            string UrlPath = "/" + (String)RouteData.Values["path"];
            //string UrlPath = Request.Path;
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion     

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Контакты";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion    


            model.Nav = new MaterialsGroup[]{
                new MaterialsGroup{Title="Контактная информация"},
                new MaterialsGroup{Title="Администрация", Alias="administration"},
                new MaterialsGroup{Title="Телефонный справочник", Alias="phone"},
                new MaterialsGroup{Title="Дополнительная информация", Alias="dop"}
            };
            model.Type= t;
            switch (t)
            {
                case "administration":
                    PageTitle = "Администрация";
                    model.Administrativ = _repository.getAdministrativ(Domain);                    
                    break;
                case "dop":
                    PageTitle = "Дополнительная информация";
                    model.DopInfo = _repository.getSiteMap("/", "contacts", Domain);                    
                    break;
                case "phone":
                    PageTitle = "Телефонный правочник";
                    model.Structures = _repository.getStructures(Domain);
                    break;
                default:
                    PageTitle = "Контактная информация";
                    model.OrgItem = _repository.getOrgInfo(Domain);
                    break;
            }
                              
            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
            return View(_ViewName,model);            
        }
    }
}

