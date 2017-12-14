using cms.dbase.Repository;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Linq;

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
                Oid = _repository.getOid()
            };

            model.DoctorsRegistry = repoRegistry.getVDoctors(model.Oid);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region Получаем данные из адресной строки
            string UrlPath = "/" + (String)RouteData.Values["path"];
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion

            var filter = getFilter();
            model.DoctorsList = _repository.getPeopleList(filter);
            model.DepartmentsSelectList = _repository.getDeparatamentsSelectList(); //Domain
            model.PeoplePosts = _repository.getPeoplePosts();//Domain

            #region Редирект на регистрацию
            if (model.DoctorsList != null)
            {
                foreach (var d in model.DoctorsList)
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

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "страница сайта";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion                   

            #region Метатеги
            ViewBag.Title = "Врачи";
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName, model);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid id)
        {
            #region Получаем данные из адресной строки
            string UrlPath = "/" + (String)RouteData.Values["path"];
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion

            var filter = getFilter();
            model.DoctorsItem = _repository.getPeopleItem(id);

            if(model.DoctorsItem != null)
            {

                #region Запись на приём
                if (string.IsNullOrEmpty(model.DoctorsItem.SNILS))
                    model.DoctorsItem.IsRedirectUrl = model.DoctorsRegistry
                            .Where(w => w.SNILS.Equals(model.DoctorsItem.SNILS))
                            .Where(w => w.Url != null)
                            .Select(s => s.Url)
                            .Any();
                #endregion

                // десериализация xml
                XmlSerializer serializer = new XmlSerializer(typeof(Employee));

                using (TextReader reader = new StringReader(model.DoctorsItem.XmlInfo))
                {
                    var result = (Employee)serializer.Deserialize(reader);
                    model.DoctorsItem.EmployeeInfo = result;
                }

            }


            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Доктор не найден";
            if(model.DoctorsItem != null)
                PageTitle = model.DoctorsItem.FIO;
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion                   

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName, model);
        }
    }
}

