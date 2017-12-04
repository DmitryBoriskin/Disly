using Disly.Models;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class EventsFrontController : RootController
    {
        private EventViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new EventViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray
            };

            #region Создаем переменные (значения по умолчанию)            
            string PageTitle = "События";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
        }

        // GET: Default
        public ActionResult Index()
        {
            var filter = getFilter();
            model.List = _repository.getEvents(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;
            
            return View(model);
        }

        // GET: /events/{num}/{alias}
        public ActionResult Item(int num, string alias)
        {
            model.Item = _repository.getEvent(num, alias);
            return View("Item", model);
        }
    }
}