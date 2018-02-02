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

            model = new EventViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Events");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
                model.Child = _repository.getSiteMapChild(currentPage.Id);
            }

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = ViewBag.Title,
                Url = ""
            });
            #endregion

            var filter = getFilter();
            //filter.Date = DateTime.Now;
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
            var page = "";

            #region currentPage
            currentPage = _repository.getSiteMap("Events");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
                page = currentPage.FrontSection;
            }
            #endregion

            model.Item = _repository.getEvent(num, alias);

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = ViewBag.Title,
                Url = string.Format("/{0}/", page)
            });
            if (model.Item != null)
            {
                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = model.Item.Title,
                    Url = ""
                });

                model.Item.Documents = _repository.getAttachDocuments(model.Item.Id);
            }
            

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