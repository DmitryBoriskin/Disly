using cms.dbModel.entity;
using Disly.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class MapSiteController : RootController
    {
        private MapSiteViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new MapSiteViewModel
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
            model.List = _repository.getSiteMapListShort(); //Domain
            return View(model);
        }
    }
}