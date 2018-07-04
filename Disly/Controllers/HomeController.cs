using cms.dbase;
using Disly.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

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
        //[OutputCache(Duration = 60, Location = OutputCacheLocation.Server)]
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

            //model.Materials = _repository.getMaterialsModule(); //Domain

            model.BannerArrayIndex = _repository.getBanners();
            model.SitemapPlate = _repository.getSiteMapList("plate");
            model.BenifitBanners = _repository.getBanners("benefits");

            var materials= _repository.getMaterialsModule();
            if (materials != null)
            {
                model.ModuleAnnouncement = materials.Where(w => w.GroupAlias == "announcement");
                model.ModuleNews = materials.Where(w => w.GroupAlias == "news");
                model.ModuleActual = materials.Where(w => w.GroupAlias == "actual");
                model.ModuleEvents = materials.Where(w => w.GroupAlias == "events");

                model.ModulePhoto = materials.Where(w => w.GroupAlias == "photo").FirstOrDefault();
                model.ModuleVideo = materials.Where(w => w.GroupAlias == "video").FirstOrDefault();
            }
            

            if (model.SitesInfo != null && model.SitesInfo.Alias == "main" && !IsSpecVersion)
            {
                model.ImportantMaterials = _repository.getMaterialsImportant();

                var NewInMedicine = _repository.getMaterialsGroupNewInMedicin();
                if (NewInMedicine != null)
                {
                    model.ModuleNewsWorld = NewInMedicine.Where(w => w.SmiType == "world").FirstOrDefault();
                    model.ModuleNewsChuv = NewInMedicine.Where(w => w.SmiType == "chuvashia").FirstOrDefault();
                    model.ModuleNewsRus = NewInMedicine.Where(w => w.SmiType == "russia").FirstOrDefault();
                }
                

                _ViewName = _ViewName.ToLower().Replace("views/", "views/_portal/");//спец вьюха для главного сайта 
            }
            else
            {
                model.Slider = _repository.getBanners("slider");
            }
            
            return View(_ViewName, model);
        }
    }
}

