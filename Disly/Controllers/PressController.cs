using cms.dbase;
using Disly.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PressController : RootController
    {
        private NewsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new NewsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Group = _repository.getMaterialsGroup()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Press");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            ViewBag.Category = (RouteData.Values["category"] != null) ? RouteData.Values["category"] : String.Empty;
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);

            ViewBag.FilterSearchText = filter.SearchText;
            ViewBag.FilterDate = filter.Date;
            ViewBag.FilterDateEnd = filter.DateEnd;

            return View(_ViewName, model);
        }

        public ActionResult Item(string year, string month, string day, string alias)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Press");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            ViewBag.Day = day.ToString();
            ViewBag.Alias = (RouteData.Values["alias"] != null) ? RouteData.Values["alias"] : String.Empty;
            model.Item = _repository.getMaterialsItem(year, month, day, alias); //,Domain

            if (model.Item != null)
                model.Item.Documents = _repository.getAttachDocuments(model.Item.Id);

            return View(_ViewName, model);
        }

        public ActionResult Category(string category)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Press");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            var filter = getFilter();
            filter.Disabled = false;
            filter.Category = category;
            model.List = _repository.getMaterialsList(filter);

            ViewBag.FilterCategory = filter.Category;
            ViewBag.FilterSearchText = filter.SearchText;
            ViewBag.FilterDate = filter.Date;
            ViewBag.FilterDateEnd = filter.DateEnd;

            return View(_ViewName, model);
        }

        public ActionResult RssSettings()
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            ViewBag.Category = (RouteData.Values["category"] != null) ? RouteData.Values["category"] : String.Empty;
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;

            ViewBag.SiteUrl = _repository.getSiteDefaultDomain(Domain);

            return View(model);
        }


        public ActionResult Rss()
        {
            Response.ContentType = "text/xml";
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);
            if (model.List != null)
            {
                ViewBag.LastDatePublish = model.List.Data[0].Date;
            }
            ViewBag.Domain = _repository.getSiteDefaultDomain(Domain);

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            return View("rss", model);
        }

    }
}

