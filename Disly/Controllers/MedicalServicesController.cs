using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class MedicalServicesController : RootController
    {
        private MedicalServicesViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new MedicalServicesViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            string PageTitle = "Медицинские услуги";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
        }

        // GET: PortalMedicalServices
        public ActionResult Index(string type)
        {
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "Медицинские услуги",
                Url = "/medicalservices"
            });

            model.Nav = new MaterialsGroup[]
            {
                new MaterialsGroup{Title = "Медицинские услуги"},
                new MaterialsGroup{Title = "Платные услуги", Alias="paid"},
                new MaterialsGroup{Title = "Дополнительная информация", Alias = "dop"}
            };

            model.Type = type;
            var sibling = _repository.getSiteMap("medicalservices");
            switch (type)
            {
                case "paid":
                    model.Breadcrumbs.Add(new Breadcrumbs
                    {
                        Title = "Платные услуги",
                        Url = ""
                    });
                    model.Info = _repository.getSiteMap(sibling.Path, type);
                    break;
                case "dop":
                    model.Breadcrumbs.Add(new Breadcrumbs
                    {
                        Title = "Дополнительная информация",
                        Url = ""
                    });
                    model.Info = _repository.getSiteMap(sibling.Path, type);
                    break;
                default:
                    model.Breadcrumbs.Add(new Breadcrumbs
                    {
                        Title = "Медицинские услуги",
                        Url = ""
                    });
                    model.MedicalServices = _repository.getMedicalServices(Domain);
                    break;
            }


            return View(model);
        }
    }
}