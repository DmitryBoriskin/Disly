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
    public class DoctorsController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private DoctorsViewModel model;
        private OnlineRegistryRepository repoRegistry = new OnlineRegistryRepository("registryConnection");

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new DoctorsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Oid = _repository.getOid()
            };

            model.DoctorsRegistry = repoRegistry.getVDoctors(model.Oid);

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
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
            currentPage = _repository.getSiteMap("Doctors");
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

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            var filter = getFilter();
            var pfilter = FilterParams.Extend<PeopleFilter>(filter);
            model.DoctorsList = _repository.getOrgPeopleList(pfilter);
            model.DepartmentsSelectList = _repository.getDeparatamentsSelectList(); //Domain
            model.Spesialisations = _repository.getSpecialisations();//Domain

            #region Редирект на регистрацию
            if (model.DoctorsList != null && model.DoctorsList.Doctors != null && model.DoctorsList.Doctors.Count() > 0 && model.DoctorsRegistry!=null)
            {
                foreach (var d in model.DoctorsList.Doctors)
                {
                    d.IsRedirectUrl = model.DoctorsRegistry
                        .Where(w => w.SNILS.Equals(d.SNILS))
                        .Where(w => w.Url != null)
                        .Select(s => s.Url)
                        .Any();
                }
            }
            #endregion

            ViewBag.SearchText = filter.SearchText;
            ViewBag.DepartGroup = filter.Group;
            ViewBag.Position = filter.Type;

            return View(_ViewName, model);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid id)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Doctors");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            var filter = getFilter();
            model.DoctorsItem = _repository.getPeopleItem(id);

            if (model.DoctorsItem != null)
            {

                #region Запись на приём
                if (string.IsNullOrEmpty(model.DoctorsItem.SNILS) && model.DoctorsRegistry!=null)
                    model.DoctorsItem.IsRedirectUrl = model.DoctorsRegistry
                            .Where(w => w.SNILS.Equals(model.DoctorsItem.SNILS))
                            .Where(w => w.Url != null)
                            .Select(s => s.Url)
                            .Any();
                #endregion
                
                var currentOrg = _repository.getCurrentOrgImportGuid();

                // десериализация xml
                XmlSerializer serializer = new XmlSerializer(typeof(Employee));

                foreach (var info in model.DoctorsItem.XmlInfo)
                {
                    using (TextReader reader = new StringReader(info))
                    {
                        var result = (Employee)serializer.Deserialize(reader);
                        if  (currentOrg != null && currentOrg.Id != Guid.Empty)
                        {
                            if (result.UZ.ID.Equals(currentOrg.Id))
                            {
                                // берём только ту запись по сотруднику на сайте которой организации находимся
                                result.EmployeeRecords = result.EmployeeRecords
                                                            .Where(w => w.Organisation.ToLower().Equals(currentOrg.Title.ToLower()))
                                                            .ToArray();

                                model.DoctorsItem.EmployeeInfo = result;
                            }
                        }
                        else
                        {
                            model.DoctorsItem.EmployeeInfo = result;
                        }
                    }
                }
            }

            ViewBag.Title = "Доктор не найден";
            if (model.DoctorsItem != null)
                ViewBag.Title = model.DoctorsItem.FIO;

            return View(_ViewName, model);
        }
    }
}

