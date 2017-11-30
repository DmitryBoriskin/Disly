using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class FeedbackController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private FeedbackViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new FeedbackViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray
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
            model.List = _repository.getFeedbacksList(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;

            #region Создаем переменные (значения по умолчанию)            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Обратная связь";
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

        public ActionResult Item(Guid id)
        {
            ViewBag.Alias = (RouteData.Values["alias"] != null) ? RouteData.Values["alias"] : String.Empty;
            model.Item = _repository.getFeedbackItem(id);

            #region Создаем переменные (значения по умолчанию)
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = (model.Item != null) ? model.Item.Title : "Новости";
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
    }
}
