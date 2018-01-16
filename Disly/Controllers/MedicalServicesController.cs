using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class MedicalServicesController : RootController
    {
        private MedicalServicesViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("MedicalServices");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new MedicalServicesViewModel
            {
                SitesInfo = siteModel,
                CurrentPage = currentPage,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>()
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

        // GET: PortalMedicalServices
        public ActionResult Index(string tab)
        {
            var page = model.CurrentPage.FrontSection;

            model.Type = tab;

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = string.Format("/{0}/", page)
            });

            //Табы на странице
            model.Nav = new List<PageTabsViewModel>();
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Медицинские услуги" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Дополнительно", Alias = "info" });

            //Обработка активных табов
            if (model.Nav != null && model.Nav.Where(s => s.Alias == tab).Any())
            {
                var navItem = model.Nav.Where(s => s.Alias == tab).Single();
                navItem.Active = true;

                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = navItem.Title,
                    Url = navItem.Alias + "/"
                });
            }

            //Получение перечня прикрепленных к организации услуг
            model.MedicalServices = _repository.getMedicalServices(Domain);

            return View(model);
        }
    }
}