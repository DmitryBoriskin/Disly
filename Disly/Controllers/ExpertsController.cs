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
    public class ExpertsController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private ExpertsViewModel model;
        private OnlineRegistryRepository repoRegistry = new OnlineRegistryRepository("registryConnection");

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new ExpertsViewModel
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
        public ActionResult Index(string type)
        {
            #region Получаем данные из адресной строки
            string UrlPath = "/" + (String)RouteData.Values["path"];
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion

            model.Nav = new List<MaterialsGroup>();
            model.Nav.Add(new MaterialsGroup { Title = "Специализация" });
            model.Nav.Add(new MaterialsGroup { Title = "Главные специалисты", Alias = "specialists" });
            model.Nav.Add(new MaterialsGroup { Title = "Экспертный состав", Alias = "experts" });
            model.Nav.Add(new MaterialsGroup { Title = "Врачи", Alias = "doctors" });
            model.Nav.Add(new MaterialsGroup { Title = "Дополнительно", Alias = "info" });

            if(model.CurrentPage != null)
            {
                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = model.CurrentPage.Title,
                    Url = string.Format("/{0}/", model.CurrentPage.FrontSection)
                });
            }

            if (!string.IsNullOrEmpty(type) && model.Nav != null && model.Nav.Where(s => s.Alias == type).Any())
            {
                model.Type = type;
                var navItem = model.Nav.Where(s => s.Alias == type).Single();

                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = navItem.Title,
                    Url = navItem.Alias + "/"
                });
            }

            var filter = getFilter();
            filter.Type = null;

            var mainSpec = _repository.getMainSpecialistList(filter);
            if(mainSpec != null)
            {
                model.DoctorsList = mainSpec.Select(s => new People()
                {
                    Id = s.Id,
                    FIO = s.Name
                }).ToArray();
            }

            
            model.ExpertsList = _repository.getPeopleList(filter);
            model.SpesialitsList = null;
            model.SpecializationInfo = "no info";


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

