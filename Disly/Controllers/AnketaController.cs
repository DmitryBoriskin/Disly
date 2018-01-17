using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class AnketaController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private WorksheetViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("Anketa");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new WorksheetViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
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

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var filter = getFilter();
            model.Item = _repository.getLastWorksheetItem();
            model.Child = (model.CurrentPage != null && model.CurrentPage.ParentId.HasValue) ? _repository.getSiteMapChild(model.CurrentPage.ParentId.Value) : null;

            ViewBag.Title = "Анкетирование";

            return View(model);
        }

    }
}
