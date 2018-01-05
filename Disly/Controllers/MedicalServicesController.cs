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
                CurrentPage = currentPage,
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
            var sibling = _repository.getSiteMap("medicalservices");

            var neededEls = _repository.getSiteMapSiblings(sibling.Path);
            model.Nav = new List<MaterialsGroup>();
            model.Nav.Add(new MaterialsGroup { Title = "Медицинские услуги" });
            model.Nav.Add(new MaterialsGroup { Title = "Дополнительно", Alias = "dop" });

            if (neededEls != null)
            {
                foreach (var n in neededEls)
                {
                    if (n.Equals("paid"))
                    {
                        model.Nav.Add(new MaterialsGroup { Title = "Платные услуги", Alias = "paid" });
                    }
                    if (n.Equals("dop"))
                    {
                        model.Nav.Add(new MaterialsGroup { Title = "Дополнительная информация", Alias = "dop" });
                    }
                }
            }
            
            model.Type = type;
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
                    model.Info = _repository.getSiteMap(sibling.Path, sibling.Alias);
                    break;
                default:
                    model.MedicalServices = _repository.getMedicalServices(Domain);
                    break;
            }


            return View(model);
        }
    }
}