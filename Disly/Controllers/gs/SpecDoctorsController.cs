using cms.dbase.Repository;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace Disly.Controllers
{
    public class SpecDoctorsController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private SpecStructureViewModel model;
        private OnlineRegistryRepository repoRegistry = new OnlineRegistryRepository("registryConnection");

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SpecStructureViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArrayLayout = bannerArrayLayout,
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Струтура";
            ViewBag.Description = "Струтура сайта главного специалиста";
            ViewBag.KeyWords = "";
            #endregion
        }

        public ActionResult Index()
        {
            #region currentPage
            currentPage = _repository.getSiteMap("SpecDoctors");
            if (currentPage == null)
                //throw new Exception("model.CurrentPage == null");
                return RedirectToRoute("Error", new { httpCode = 404 });

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            //Хлебные крошки
            if (currentPage.ParentId.HasValue)
            {
                var parentPage = _repository.getPageInfo(currentPage.ParentId.Value);
                if (parentPage != null)
                    model.Breadcrumbs.Add(new Breadcrumbs
                    {
                        Title = parentPage.Title,
                        Url = "/" + parentPage.Alias
                    });
            }

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = ViewBag.Title,
                Url = ""
            });

            //int countSpec = _repository.getCountGSBySite(Domain);

            //Получаем модель "главный специалист" (в нее может входить несколько врачей)
            var filter = getFilter();
            filter.Domain = null;
            var pfilter = FilterParams.Extend<PeopleFilter>(filter);
            var gs = _repository.getGSItem(model.SitesInfo.ContentId);
            if (gs != null)
            {
                model.MainSpec = gs;
                if (gs.Specialisations != null && gs.Specialisations.Count() > 0)
                {
                    var docfilter = FilterParams.Extend<PeopleFilter>(filter);
                    docfilter.Specializations = gs.Specialisations;
                    model.DoctorsList = _repository.getDoctorsList(docfilter);
                   
                    var specfiltr = new SpecialisationFilter()
                    {
                        Specializations = gs.Specialisations
                    };
                    model.Specializations = _repository.getSpecialisations(specfiltr);

                    ViewBag.SearchText = filter.SearchText;
                    ViewBag.Position = filter.Type;
                }
            }

            ViewBag.SearchText = filter.SearchText;
            ViewBag.DepartGroup = filter.Group;
            ViewBag.Position = filter.Type;

            return View(_ViewName, model);
        }

    }
}

