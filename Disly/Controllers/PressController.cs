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
                Group=_repository.getMaterialsGroup()
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Category = (RouteData.Values["category"] != null) ? RouteData.Values["category"] : String.Empty;
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart=filter.Date;
            ViewBag.NewsSearchDateFin=filter.DateEnd;

            #region Создаем переменные (значения по умолчанию)            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Новости";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion            
            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
            return View(_ViewName, model);
        }

        public ActionResult Item(string year, string month, string day, string alias)
        {
            ViewBag.Day = day.ToString();
            ViewBag.Alias = (RouteData.Values["alias"] != null) ? RouteData.Values["alias"] : String.Empty;
            model.Item = _repository.getMaterialsItem(year, month, day, alias); //,Domain

            #region Создаем переменные (значения по умолчанию)            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle =(model.Item!=null)?model.Item.Title: "Новости";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion
            
            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName, model);
        }

        public ActionResult Category(string category)
        {
            ViewBag.CurrentCategory = category;         
            var filter = getFilter();
            filter.Disabled = false;
            filter.Category = category;
            ViewBag.Filter = filter;

            model.List = _repository.getMaterialsList(filter);

            #region Создаем переменные (значения по умолчанию)            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Новости";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion            
            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
            return View(_ViewName, model);
        }

        public ActionResult RssSettings()
        {
            ViewBag.Category = (RouteData.Values["category"] != null) ? RouteData.Values["category"] : String.Empty;
            var filter = getFilter();
            filter.Disabled = false;
            model.List = _repository.getMaterialsList(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;

            ViewBag.SiteUrl = _repository.getDomainSite();

            #region Создаем переменные (значения по умолчанию)            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Rss ленты";
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
            ViewBag.Domain = _repository.getDomainSite();            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";            
            return View("rss", model);
        }

    }
}

