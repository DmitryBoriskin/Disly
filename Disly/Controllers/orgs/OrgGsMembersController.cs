using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class OrgGsMembersController : RootController
    {
        private SpecialistsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("OrgGsMembers");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new SpecialistsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>(),
                CurrentPage = currentPage
            };

            #region Создаем переменные (значения по умолчанию)
            string PageTitle = model.CurrentPage.Title;
            string PageDesc = model.CurrentPage.Desc;
            string PageKeyw = model.CurrentPage.Keyw;
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
        }

        // GET: GeneralSpecialists
        public ActionResult Index()
        {
            string _ViewName = (ViewName != string.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            if ((model.SitesInfo == null) || (model.SitesInfo != null && model.SitesInfo.Type != ContentLinkType.ORG.ToString().ToLower()) && model.SitesInfo.Alias != "main")
                return RedirectToRoute("Error", new { httpCode = 405 });

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = ""
            });

            // Список врачей организации, входящих в объект "Главный специалист"
            var filter = getFilter();
            var pfilter = FilterParams.Extend<PeopleFilter>(filter);
            pfilter.Orgs = new Guid[] { model.SitesInfo.ContentId };
            model.Members = _repository.getMainSpecialistMembers(pfilter);

            return View(model);
        }
    }
}