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
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Врачи", Alias = "doctors" });

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
            var mainSpec = _repository.getMainSpecialistItem(Guid.Parse("8f82d333-81e9-4b38-adc7-2632e0842a67"));
            if (mainSpec != null)
            {
                model.MainSpec = mainSpec;
                filter.Domain = null;
                //Список врачей, входящих в модель "главный специалист"
                if (mainSpec.EmployeeMainSpecs != null)
                {
                    filter.Id = mainSpec.EmployeeMainSpecs.ToArray();
                    model.SpesialitsList = _repository.getPeopleList(filter);
                }

                //Получение экспертного состава
                if (mainSpec.EmployeeMainSpecs != null)
                {
                    filter.Id = mainSpec.EmployeeExpSoviet.ToArray();
                    model.ExpertsList = _repository.getPeopleList(filter);
                }

                //Получение членов общества (врачей по специальности)
                model.DoctorsList = null;
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

