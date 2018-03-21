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
    public class SpecMembersController : RootController
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
        public ActionResult Index()
        {
            #region currentPage
            currentPage = _repository.getSiteMap("SpecMembers");
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
                        Url = parentPage.Alias
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
                //Список врачей, входящих в модель "главный специалист"
                model.SpesialitsList = _repository.getGSMembers(gs.Id, GSMemberType.SPEC);
                if (model.SpesialitsList != null && model.SpesialitsList.Count() > 0)
                {
                    foreach (var item in model.SpesialitsList)
                    {
                        if (item.People != null)
                        {
                            var fltr = new OrgFilter()
                            {
                                PeopleId = item.People.Id
                            };
                            item.Orgs = _repository.getOrgs(fltr);
                        }
                    }
                }
            }

            ViewBag.SearchText = filter.SearchText;
            ViewBag.DepartGroup = filter.Group;
            ViewBag.Position = filter.Type;

            ViewBag.GeneralDomain = Settings.GeneralDomain;

            return View(_ViewName, model);
        }
    }
}

