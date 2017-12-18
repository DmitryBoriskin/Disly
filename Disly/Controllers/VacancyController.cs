using cms.dbase;
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
                Breadcrumbs= breadcrumb,
                BannerArray = bannerArray
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var filter = getFilter();            
            model.List = _repository.getVacancy(filter);


            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Вакансии";//(model.Item != null)? model.Item.Title: null;
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion            
          
            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName,model);            
        }
        public ActionResult Item(Guid id)
        {
            model.Item=_repository.getVacancyItem(id);


            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Вакансии";//(model.Item != null)? model.Item.Title: null;
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(model);
        }
    }
}

