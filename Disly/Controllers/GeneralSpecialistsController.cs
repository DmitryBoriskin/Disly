using cms.dbModel.entity;
using Disly.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class GeneralSpecialistsController : RootController
    {
        private GeneralSpecialistViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new GeneralSpecialistViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>()
            };
        }

        // GET: GeneralSpecialists
        public ActionResult Index()
        {
            #region Получаем данные из адресной строки
            string UrlPath = "/" + (string)RouteData.Values["path"];
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion

            string frontSection = "mainspec";

            model.List = _repository.getMainSpecialistList();
            model.DopInfo = _repository.getSiteMap(frontSection);
            
            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != string.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "страница сайта";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion                   

            #region Метатеги
            ViewBag.Title = "Главные специалисты";
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = ViewBag.Title,
                Url = ""
            });

            return View(model);
        }
    }
}