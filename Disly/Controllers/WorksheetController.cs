using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class WorksheetController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private WorksheetViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new WorksheetViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                CurrentPage = currentPage
            };
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
