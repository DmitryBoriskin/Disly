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
    public class SpecContactsController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private SpecContactsViewModel model;
        private OnlineRegistryRepository repoRegistry = new OnlineRegistryRepository("registryConnection");

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SpecContactsViewModel()
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Струтура";
            ViewBag.Description = "Струтура сайта главного специалиста";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string tab)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("SpecContacts");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion


            if ((model.SitesInfo == null) || (model.SitesInfo != null && model.SitesInfo.Type != ContentLinkType.SPEC.ToString().ToLower()))
                return RedirectToRoute("Error", new { httpCode = 405 });

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            var page = model.CurrentPage.FrontSection;

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = ""
            });

            //Получаем модель "главный специалист" (в нее может входить несколько врачей)
            var filter = getFilter();
            filter.Domain = null;
            var pfilter = FilterParams.Extend<PeopleFilter>(filter);
            var mainSpec = _repository.getMainSpecialistItem(model.SitesInfo.ContentId);
            if (mainSpec != null)
            {
                model.MainSpec = mainSpec;
                if(mainSpec.EmployeeMainSpecs != null)
                {
                    pfilter.Id = mainSpec.EmployeeMainSpecs.ToArray();
                    model.SpesialitsList = _repository.getPeopleList(pfilter);
                }
            }

            return View(_ViewName, model);
        }
    }
}

