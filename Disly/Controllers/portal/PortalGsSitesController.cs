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

            currentPage = _repository.getSiteMap("PortalGsSites");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new SpecialistsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>(),
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

        // GET: GeneralSpecialists
        public ActionResult Index()
        {
            if ((model.SitesInfo == null) || (model.SitesInfo != null && model.SitesInfo.Type != ContentLinkType.ORG.ToString().ToLower()))
                return RedirectToRoute("Error", new { httpCode = 405 });

            string _ViewName = (ViewName != string.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = ""
            });

            //Список объектов "Главный специалист"
            var filter = getFilter();
            filter.Domain = null;
            model.List = _repository.getMainSpecialistList(filter);

            return View(model);
        }
    }
}