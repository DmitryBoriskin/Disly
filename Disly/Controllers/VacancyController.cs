using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class VacancyController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private VacancyViewModel model;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new VacancyViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
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
            currentPage = _repository.getSiteMap("Vacancy");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            var filter = getFilter();
            if (Domain == "main")
                filter.Domain = null;
            model.List = _repository.getVacancy(filter);

            model.Breadcrumbs.Add(new Breadcrumbs()
            {
                Title = ViewBag.Title,
                Url = string.Format("/{0}/", model.CurrentPage.FrontSection)
            });

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;

            return View(_ViewName, model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Item(Guid id)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Vacancy");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.Item = _repository.getVacancyItem(id);
           
            if (model.Item != null)
            {
                if (model.Breadcrumbs != null)
                {
                    model.Breadcrumbs.Add(new Breadcrumbs()
                    {
                        Title = model.Item.Profession,
                        Url = ""
                    });
                }
                model.Documents = _repository.getAttachDocuments(id);
                ViewBag.Title = model.Item.Profession;
            }
            else
                return new HttpNotFoundResult();

            return View(model);
        }
    }
}
