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

            model = new WorksheetViewModel
            {
                SitesInfo = siteModel,
                MainMenu= mainMenu,
                Breadcrumbs = breadcrumb,
                BannerArrayLayout = bannerArrayLayout,
                CurrentPage = currentPage
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
            currentPage = _repository.getSiteMap("Anketa");
            if (currentPage == null)
                //throw new Exception("model.CurrentPage == null");
                return RedirectToRoute("Error", new { httpCode = 404 });

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            var filter = getFilter();
            model.Item = _repository.getLastWorksheetItem();
            model.Child = (model.CurrentPage != null && model.CurrentPage.ParentId.HasValue) ? _repository.getSiteMapChild(model.CurrentPage.ParentId.Value) : null;

            ViewBag.Title = "Анкетирование";

            return View(model);
        }
    }
}
