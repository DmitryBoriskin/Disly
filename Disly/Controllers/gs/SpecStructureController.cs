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
               // Breadcrumbs = breadcrumb,
                Breadcrumbs = new List<Breadcrumbs>()
            };
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string tab)
        {
            if ((model.SitesInfo == null) || (model.SitesInfo != null && model.SitesInfo.Type != ContentLinkType.SPEC.ToString().ToLower()))
                return RedirectToRoute("Error", new { httpCode = 405});

            model.Type = tab;
            if (model.CurrentPage == null)
                throw new Exception("model.CurrentPage == null");

            var page = model.CurrentPage.FrontSection;

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = string.Format("/{0}/", page)
            });

            //Табы на странице
            model.Nav = new List<PageTabsViewModel>();
            model.Nav.Add(new PageTabsViewModel {Page = page, Title = "Информация" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Главные специалисты", Alias = "specialists" });
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
                    Url = navItem.Alias + "/"
                });
            }

            //Получаем модель "главный специалист" (в нее может входить несколько врачей)
            var filter = getFilter();
            filter.Domain = null;
            var pfilter = FilterParams.Extend<PeopleFilter>(filter);
            var mainSpec = _repository.getMainSpecialistItem(model.SitesInfo.ContentId);
            if (mainSpec != null)
            {
                model.MainSpec = mainSpec;

                switch (tab)
                {
                    case "specialists":
                        //Список врачей, входящих в модель "главный специалист"
                        if (mainSpec.EmployeeMainSpecs != null)
                        {
                            pfilter.Id = mainSpec.EmployeeMainSpecs.ToArray();
                            model.SpesialitsList = _repository.getPeopleList(pfilter);
                        }
                        break;
                    case "experts":
                        //Получение экспертного состава
                        if (mainSpec.EmployeeMainSpecs != null)
                        {
                            pfilter.Id = mainSpec.EmployeeExpSoviet.ToArray();
                            model.ExpertsList = _repository.getPeopleList(pfilter);
                        }
                        break;
                    case "doctors":
                        if (mainSpec.Specialisations != null)
                        {
                            //Получение членов общества (врачей по специальности)
                            pfilter.Specialization = mainSpec.Specialisations;
                            model.DoctorsList = _repository.getPeopleList(pfilter);
                        }
                        break;
                }
            }


            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Специалисты";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            ViewBag.SearchText = filter.SearchText;
            ViewBag.DepartGroup = filter.Group;
            ViewBag.Position = filter.Type;

            return View(_ViewName, model);
        }
    }
}

