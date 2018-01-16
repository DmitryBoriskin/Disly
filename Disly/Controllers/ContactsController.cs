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

            currentPage = _repository.getSiteMap("Contacts");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new ContatcsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                CurrentPage = currentPage
            };

            #region Создаем переменные (значения по умолчанию)
            string PageTitle = model.CurrentPage.Title;
            string PageDesc = model.CurrentPage.Desc;
            string PageKeyw = model.CurrentPage.Keyw;
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
        }


        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string t)
        {
            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = model.CurrentPage.Title;
            string PageDesc = model.CurrentPage.Desc;
            string PageKeyw = model.CurrentPage.Keyw;
            #endregion    

            model.Nav = new MaterialsGroup[]
            {
                new MaterialsGroup{Title="Контактная информация"},
                new MaterialsGroup{Title="Администрация", Alias="administration"},
                new MaterialsGroup{Title="Телефонный справочник", Alias="phone"},
                new MaterialsGroup{Title="Дополнительная информация", Alias="dop"}
            };
            model.Type = t;
            switch (t)
            {
                case "administration":
                    PageTitle = "Администрация";
                    model.Administrativ = _repository.getAdministrative(Domain);
                    break;
                case "dop":
                    PageTitle = "Дополнительная информация";
                    model.DopInfo = model.CurrentPage;  //Избавиться от DopInfo
                    break;
                case "phone":
                    PageTitle = "Телефонный правочник";
                    model.Structures = _repository.getStructures(); //Domain
                    break;
                default:
                    PageTitle = "Контактная информация";
                    model.OrgItem = _repository.getOrgInfo(); //Domain
                    break;
            }

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
            return View(_ViewName, model);
        }
    }
}

