using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PortalGsSitesController : RootController
    {
        private SpecialistsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new SpecialistsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>(),
                CurrentPage = currentPage
            };
        }

        // GET: GeneralSpecialists
        public ActionResult Index()
        {
            if ((model.SitesInfo == null) || (model.SitesInfo != null && model.SitesInfo.Type != ContentLinkType.ORG.ToString().ToLower()))
                return RedirectToRoute("Error", new { httpCode = 405 });

            if (model.CurrentPage == null)
                throw new Exception("model.CurrentPage == null");

            var page = model.CurrentPage.FrontSection;

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = "" //string.Format("/{0}/", page)
            });

            //Список объектов "Главный специалист"
            var filter = getFilter();
            filter.Domain = null;
            model.List = _repository.getMainSpecialistList(filter);

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != string.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Главные специалисты";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion


            return View(model);
        }
    }
}