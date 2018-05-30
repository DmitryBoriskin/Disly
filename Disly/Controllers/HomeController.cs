using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class HomeController : RootController
    {
        private HomePageViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new HomePageViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArrayLayout = bannerArrayLayout,           
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns ></returns>
        public ActionResult Index()
        {
            #region currentPage

            if (currentPage != null)
            {
                ViewBag.Title = "Главная";
                ViewBag.Description = "";
                ViewBag.KeyWords = "";
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            
            model.Oid = _repository.getOid();
            model.Materials = _repository.getMaterialsModule(); //Domain
            model.BannerArrayIndex = _repository.getBanners();

            if (model.SitesInfo != null && model.SitesInfo.Alias == "main" && !IsSpecVersion)
            {
                model.ImportantMaterials = _repository.getMaterialsImportant();
                //model.MaterialsNewInMedicin = _repository.getMaterialsGroupNewInMedicin();
                _ViewName = _ViewName.ToLower().Replace("views/", "views/_portal/");//спец вьюха для главного сайта 
            }
            
            return View(_ViewName, model);
        }
    }
}

