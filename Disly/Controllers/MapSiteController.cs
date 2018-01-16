using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class MapSiteController : RootController
    {
        private SiteMapViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("MapSite");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new SiteMapViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
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

        // GET: MapSite
        public ActionResult Index()
        {
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "Карта сайта",
                Url = ""
            });
            model.List = _repository.getSiteMapListShort(null); //Domain
            return View(model);
        }
    }
}