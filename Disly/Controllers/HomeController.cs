using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Web.Mvc;
using System.Runtime.Caching;

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
                MainMenu= mainMenu,
                //MainMenu= mainMenu,
                BannerArrayLayout = bannerArrayLayout,           
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns ></returns>
        //[OutputCache(Duration = 60, Location = OutputCacheLocation.Server, VaryByHeader = "host")]
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

            model.BannerArrayIndex = _repository.getBanners();
            model.SitemapPlate = _repository.getSiteMapList("plate");
            model.BenifitBanners = _repository.getBanners("benefits");

            //model.PressModuleIndex=_repository.getMaterialsModuleNew();
            model.ModuleIndex = GetPressModuleIndex();

            if (model.SitesInfo != null && model.SitesInfo.Alias == "main" && !IsSpecVersion)
            {
                model.ImportantMaterials = _repository.getMaterialsImportant();
                
                _ViewName = _ViewName.ToLower().Replace("views/", "views/_portal/");//спец вьюха для главного сайта 
            }
            else
            {
                model.Slider = _repository.getBanners("slider");
            }
            
            return View(_ViewName, model);
        }


        private IndexModel GetPressModuleIndex()
        {
            ObjectCache cache = MemoryCache.Default;            

            IndexModel d = (IndexModel)cache["PressModuleIndex" + Domain];
            if (d == null)
            {
                d = _repository.getMaterialsModuleNew();
                cache.Set("PressModuleIndex" + Domain, d, DateTime.Now.AddMinutes(5));
            }
            return d;
        }


    }
}

