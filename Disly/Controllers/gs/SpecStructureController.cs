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
    public class SpecStructureController : RootController
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
        public ActionResult Index(string tab)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("SpecStructure");
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

            model.Type = tab;
            var page = model.CurrentPage.FrontSection;

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = ""
            });

            int countSpec = _repository.getCountGSBySite(Domain);

            //Табы на странице
            model.Nav = new List<PageTabsViewModel>();
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Положение о специалисте" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = countSpec > 1 ? "Главные специалисты" : "Главный специалист", Alias = "specialists" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Экспертный состав", Alias = "experts" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Специалисты / члены общества", Alias = "doctors" });

            //Обработка активных табов
            if (model.Nav != null && model.Nav.Where(s => s.Alias == tab).Any())
            {
                var navItem = model.Nav.Where(s => s.Alias == tab).Single();
                navItem.Active = true;

                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = navItem.Title,
                    Url = ""
                });
            }

            //Получаем модель "главный специалист" (в нее может входить несколько врачей)
            var filter = getFilter();
            filter.Domain = null;
            var pfilter = FilterParams.Extend<PeopleFilter>(filter);
            var gs = _repository.getGSItem(model.SitesInfo.ContentId);
            if (gs != null)
            {
                model.MainSpec = gs;

                switch (tab)
                {
                    case "specialists":
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
                        break;
                    case "experts":
                        //Получение экспертного состава
                        model.ExpertsList = _repository.getGSMembers(gs.Id, GSMemberType.EXPERT);
                        if (model.ExpertsList != null && model.ExpertsList.Count() > 0)
                        {
                            foreach (var item in model.ExpertsList)
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
                        break;
                    case "doctors":
                        if (gs.Specialisations != null)
                        {
                            //Получение членов общества (врачей по специальности)
                            model.EmployeeList = _repository.getEmployeeList(gs.Specialisations);
                        }
                        break;
                }
            }

            ViewBag.SearchText = filter.SearchText;
            ViewBag.DepartGroup = filter.Group;
            ViewBag.Position = filter.Type;


            return View(_ViewName, model);
        }
    }
}

