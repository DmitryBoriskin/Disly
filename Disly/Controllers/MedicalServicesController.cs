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
        public ActionResult Index()
        {
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "Медицинские услуги",
                Url = ""
            });
            model.MedicalServices = _repository.getMedicalServices(Domain);

            return View(model);
        }
    }
}