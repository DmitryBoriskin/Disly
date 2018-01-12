using cms.dbModel.entity;
using Disly.Models;
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

            model = new SiteMapViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>()
            };
            #region Создаем переменные (значения по умолчанию)
            string PageTitle = "Карта сайта";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
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