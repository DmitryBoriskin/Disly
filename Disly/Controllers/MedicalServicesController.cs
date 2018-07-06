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

            model = new MedicalServicesViewModel
            {
                SitesInfo = siteModel,
                CurrentPage = currentPage,
                MainMenu= mainMenu,
                BannerArrayLayout = bannerArrayLayout,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: PortalMedicalServices
        public ActionResult Index(string tab)
        {
            var page = "";

            #region currentPage
            currentPage = _repository.getSiteMap("MedicalServices");
            if (currentPage == null)
                //throw new Exception("model.CurrentPage == null");
                return RedirectToRoute("Error", new { httpCode = 404 });

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
                page = currentPage.FrontSection;
            }
            #endregion

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = ViewBag.Title,
                Url = ""
            });

            //Табы на странице
            model.Nav = new List<PageTabsViewModel>();
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Медицинские услуги" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Дополнительно", Alias = "info" });

            model.Type = tab;
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
            //model.MedicalServices = _repository.getMedicalServices(Domain);
            model.MedicalServices = _repository.getMedicalServicesOptim(model.SitesInfo.ContentId);
            

            return View(model);
        }
    }
}