using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Index(string tab) 
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.Type = tab;
            var page = model.CurrentPage.FrontSection;

            //Табы на странице
            model.Nav = new List<PageTabsViewModel>();
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Контактная информация" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Администрация", Alias = "administration" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Телефонный справочник", Alias = "phone" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Дополнительная информация", Alias = "info" });

            //Обработка активных табов
            if (model.Nav != null && model.Nav.Where(s => s.Alias == tab).Any())
            {
                var navItem = model.Nav.Where(s => s.Alias == tab).Single();
                navItem.Active = true;

                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = navItem.Title,
                    Url = ""
                });
            }

            switch (tab)
            {
                case "administration":
                    model.Administrativ = _repository.getAdministrative(Domain);
                    break;
                case "info":
                    break;
                case "phone":
                    model.Structures = _repository.getStructures();
                    break;
                default:
                    model.OrgItem = _repository.getOrgInfo();
                    break;
            }

            return View(_ViewName, model);
        }
    }
}

