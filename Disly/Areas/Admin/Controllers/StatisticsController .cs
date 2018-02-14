using Disly.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class StatisticsController : CoreController
    {
        StatisticViewModel model;
        FilterParams filter;
        //MaterialsGroup[] Groups;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter();

            model = new StatisticViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };
            if (AccountInfo != null)
            {
                model.Menu = _cmsRepository.getCmsMenu(AccountInfo.Id);
            }          



            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }
        
        public ActionResult Index()
        {
            model.StatMaterialsOrg= _cmsRepository.getStatisticMaterials(filter);
            model.StatMaterialsGs = _cmsRepository.getStatisticMaterialsGs(filter);
            model.StatFeedBack = _cmsRepository.getStatisticFeedBack();

            return View(model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search( DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            
            if (datestart.HasValue)
                query = AddFiltrParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());
            if (dateend.HasValue)
                query = AddFiltrParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());            

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
    }
}