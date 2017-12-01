using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Disly.Controllers
{
    public class PortalDoctorsController : RootController
    {
        private PortalDoctorsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new PortalDoctorsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray
            };
        }

        // GET: PortalDoctors
        public ActionResult Index()
        {
            var filter = getFilter();
            model.DoctorList = _repository.getDoctorsList(filter);
            model.PeoplePosts = _repository.getPeoplePosts(Domain);
            
            ViewBag.SearchText = filter.SearchText;
            ViewBag.Position = filter.Type;

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion                   

            #region Метатеги
            ViewBag.Title = "Врачи";
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
            
            return View(model);
        }

        // GET: portaldoctors/id
        public ActionResult Item(Guid id)
        {
            model.Doctor = _repository.getPeopleItem(id);

            // десериализация xml
            XmlSerializer serializer = new XmlSerializer(typeof(Employee));

            using (TextReader reader = new StringReader(model.Doctor.XmlInfo))
            {
                var result = (Employee)serializer.Deserialize(reader);
                model.Doctor.EmployeeInfo = result;
            }

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = model.Doctor.FIO;
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion                   

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View("Index", model);
        }
    }
}