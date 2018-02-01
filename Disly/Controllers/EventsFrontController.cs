using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class EventsFrontController : RootController
    {
        private EventViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("Events");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new EventViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Child = _repository.getSiteMapChild(currentPage.Id),
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

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = string.Format("/{0}/", model.CurrentPage.FrontSection)
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            var filter = getFilter();
            filter.Date = DateTime.Now;
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
            if (model.Item != null)
            {
                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = model.Item.Title,
                    Url = ""
                });
            }
            if (model.Item != null)
                model.Item.Documents = _repository.getAttachDocuments(model.Item.Id);

            return View("Item", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Archive()
        {
            var filter = getFilter();
            filter.DateEnd = DateTime.Now;
            model.List = _repository.getEvents(filter);

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;

            ViewBag.Title = "Архив событий";
            return View(model);
        }
    }
}