using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class SearchController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private TypePageViewModel model;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new TypePageViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
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
        public ActionResult Index(string searchtext)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Search");
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

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            ViewBag.SearchText = (searchtext != null) ? searchtext.Replace("%20", " ") : String.Empty;

            model.Item = _repository.getSiteMap(_path, _alias);
            if (model.Item != null)
            {
                //if (model.Item.FrontSection.ToLower() != "page")
                //{
                //    return Redirect("/" + model.Item.FrontSection);
                //}
                model.Child = _repository.getSiteMapChild(model.Item.Id);
                model.Documents = _repository.getAttachDocuments(model.Item.Id);
            }

            return View(_ViewName, model);
        }
    }
}
