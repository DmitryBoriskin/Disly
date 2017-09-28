using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class OrgsController : CoreController
    {
        OrgsViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            model = new OrgsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/Orgs
        public ActionResult Index()
        {
            filter = getFilter();
            model.OrgList = _cmsRepository.getOrgs(filter);//+ список организаций
            return View(model);
        }

        
        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool disabled, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "disabled", disabled.ToString().ToLower());
            return Redirect(StartUrl + query);
        }

        /// <summary>
        /// Очищаем фильтр
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            //  При создании записи сбрасываем номер страницы
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "page", String.Empty);
            return Redirect(StartUrl + "item/" + Guid.NewGuid() + "/" + query);
        }


        
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getOrgItem(Id);    //+ список структур        
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, OrgsViewModel back_model)
        {
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                _cmsRepository.setOrgs(id, back_model.Item);
                userMassege.info = "Запись сохранена";
                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            model.ErrorInfo = userMassege;
            return View("Item", model);
        }

        // GET: Admin/Orgs/structure/{Guid}
        public ActionResult Structure(Guid id)
        {
            ViewBag.Title = "Структурное подразделение";
            model.StructureItem = _cmsRepository.getStructure(id);//+ список подразделений      
            model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(id, ViewBag.ActionName);
            //model.StructureItem
            return View("Structure", model);
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }
        //[HttpPost]
        //public ActionResult()
        // GET: Admin/Orgs/department/{Guid}
        public ActionResult Department(Guid id)
        {
            ViewBag.Title = "Отделение";            
            model.DepartmentItem=_cmsRepository.getDepartamentItem(id);
            model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(id, ViewBag.ActionName);
            return View("department", model);
        }
        
        /// <summary>
        /// Добавление телефонного номера отделению
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-new-phone-depart")]
        public ActionResult AddPhone()
        {
            string IdDepartment = Request["DepartmentItem.Id"];
            string PhoneLabel = Request["new_phone_label"];
            string PhoneValue = Request["new_phone_value"];
            _cmsRepository.insDepartmentsPhone(Guid.Parse(IdDepartment), PhoneLabel, PhoneValue);
            return Redirect(((System.Web.HttpRequestWrapper)Request).RawUrl);
        }
        public ActionResult DelPhoneDepart(int id)
        {
            _cmsRepository.delDepartmentsPhone(id);
            return null;
        }


    }
}