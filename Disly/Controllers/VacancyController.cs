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

            currentPage = _repository.getSiteMap("Vacancy");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null"); //Заменить потом все на  return new HttpNotFoundResult();

            model = new VacancyViewModel
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

            model.Breadcrumbs.Add(new Breadcrumbs()
            {
                Title = PageTitle,
                Url = string.Format("/{0}/", model.CurrentPage.FrontSection)
            });
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            var filter = getFilter();
            if (Domain == "main")
                filter.Domain = null;
            model.List = _repository.getVacancy(filter);

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
                ViewBag.Title = model.Item.Post;
            }
            else
                return new HttpNotFoundResult();

            return View(model);
        }
    }
}
