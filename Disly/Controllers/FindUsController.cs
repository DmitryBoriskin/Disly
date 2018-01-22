using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class FindUsController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private ContatcsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("FindUs");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new ContatcsViewModel
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
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.OrgItem = _repository.getOrgInfo(null);
            model.Structures = _repository.getStructures();

            return View(_ViewName, model);
        }
    }
}

