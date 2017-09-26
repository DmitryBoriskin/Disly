using cms.dbase;
using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace Disly.Areas.Admin.Controllers
{
    [Authorize]
    public class CoreController : Controller
    {
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected AccountRepository _accountRepository { get; private set; }
        protected cmsRepository _cmsRepository { get; private set; }
              
        public string Domain;
        public string StartUrl;
        public int defaultpageSize = 20;
        public AccountModel AccountInfo;
        public SettingsModel SettingsInfo;
        public ResolutionsModel UserResolutionInfo;//права пользователей
        public cmsLogModel[] LogInfo;
        public SitesModel SiteInfo;
        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            string _ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            string _ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            Guid _PageId;

            try { Domain = _cmsRepository.getSiteId(Request.Url.Host.ToLower().Replace("www.", "")); }
            catch
            {
                if (Request.Url.Host.ToLower().Replace("www.", "") != ConfigurationManager.AppSettings["BaseURL"]) filterContext.Result = Redirect("/Error/");
                else Domain = String.Empty;
            }

            StartUrl = "/Admin/" + (String)RouteData.Values["controller"] + "/";

            #region Настройки сайта
            //SettingsInfo = _cmsRepository.getCmsSettings();
            // Сайт, на котором находимся
            //if (Domain != String.Empty) SettingsInfo.ThisSite = _cmsRepository.getSite(Domain);
            #endregion
            #region Данные об авторизованном пользователе
            Guid _userId = new Guid();
            try { _userId = new Guid(System.Web.HttpContext.Current.User.Identity.Name); }
            catch { FormsAuthentication.SignOut(); }
            AccountInfo = _accountRepository.getCmsAccount(_userId);
            // Список доменов, доступных пользователю
            AccountInfo.Domains = _accountRepository.getUserDomains(_userId);
            #endregion
            #region Права пользователя
            UserResolutionInfo = _accountRepository.getCmsUserResolutioInfo(_userId, _ControllerName);
            // Если нет прав на проссмотр, то направляем на главную
            try { if (!UserResolutionInfo.Read) filterContext.Result = Redirect("/Admin/"); }
            catch { }
            #endregion
            #region Ограничение пользователя (не администратора портала и не разработчика) на доступ только к своим сайтам (доменам)
            int IsRedirect = 0;
            if (AccountInfo.Group.ToLower() != "developer" && AccountInfo.Group.ToLower() != "administrator")
            {
                foreach (DomainList domain in AccountInfo.Domains)
                {
                    if (domain.SiteId == Domain) { IsRedirect++; }
                }
                //перенаправляем на первый из своих доменов
                if(IsRedirect==0)
                {
                    string url = "http://" + AccountInfo.Domains[0].DomainName + "/Admin/";
                    Response.Redirect(url, true);
                }
            }
            #endregion
            #region  Логи
            try
            {
                _PageId = Guid.Parse(filterContext.RouteData.Values["id"].ToString());
                LogInfo = _cmsRepository.getCmsPageLog(_PageId);
            }
            catch { }
            #endregion
        }
        
        public CoreController()
        {
            _accountRepository = new AccountRepository("cmsdbConnection");
            _cmsRepository = new cmsRepository("cmsdbConnection");
        }
        
        public string addFiltrParam(string query, string name, string val)
        {
            //string search_Param = @"\b" + name + @"=[\w]*[\b]*&?";
            string search_Param = @"\b" + name + @"=(.*?)(&|$)";
            string normal_Query = @"&$";

            Regex delParam = new Regex(search_Param, RegexOptions.CultureInvariant);
            Regex normalQuery = new Regex(normal_Query);
            query = delParam.Replace(query, String.Empty);
            query = normalQuery.Replace(query, String.Empty);
            
            if (val != String.Empty)
            {
                if (query.IndexOf("?") > -1) query += "&" + name + "=" + val;
                else query += "?" + name + "=" + val;
            }

            query = query.Replace("?&", "?").Replace("&&", "&");

            return query;
        }

        public FilterParams getFilter(int defaultPageSize = 20)
        {

            string return_url = HttpUtility.UrlDecode(Request.Url.Query);
            // если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            return_url = (Convert.ToInt32(Request.QueryString["page"]) == 1) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            return_url = (Convert.ToInt32(Request.QueryString["size"]) == defaultpageSize) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            return_url = (Convert.ToBoolean(Request.QueryString["disabled"]) == false) ? addFiltrParam(return_url, "disabled", String.Empty) : return_url;
            return_url = String.IsNullOrEmpty(Request.QueryString["searchtext"]) ? addFiltrParam(return_url, "searchtext", String.Empty) : return_url;

            // Если парамметры из адресной строки равны значениям по умолчанию - удаляем их из URL
            if (return_url.ToLower() != HttpUtility.UrlDecode(Request.Url.Query).ToLower())
                Response.Redirect(StartUrl + return_url);


            DateTime? DateNull = new DateTime?();

            FilterParams result = new FilterParams()
            {
                Page = (Convert.ToInt32(Request.QueryString["page"]) > 0) ? Convert.ToInt32(Request.QueryString["page"]) : 1,
                Size = (Convert.ToInt32(Request.QueryString["size"]) > 0) ? Convert.ToInt32(Request.QueryString["size"]) : defaultPageSize,
                Type = (String.IsNullOrEmpty(Request.QueryString["type"])) ? String.Empty : Request.QueryString["type"],
                Categoty = (String.IsNullOrEmpty(Request.QueryString["category"])) ? String.Empty : Request.QueryString["category"],
                Group = (String.IsNullOrEmpty(Request.QueryString["group"])) ? String.Empty : Request.QueryString["group"],
                Lang = (String.IsNullOrEmpty(Request.QueryString["lang"])) ? String.Empty : Request.QueryString["lang"],
                Date = (String.IsNullOrEmpty(Request.QueryString["date"])) ? DateNull : DateTime.Parse(Request.QueryString["date"]),
                DateEnd = (String.IsNullOrEmpty(Request.QueryString["dateend"])) ? DateNull : DateTime.Parse(Request.QueryString["dateend"]),
                SearchText = (String.IsNullOrEmpty(Request.QueryString["searchtext"])) ? String.Empty : Request.QueryString["searchtext"],
                Disabled = (String.IsNullOrEmpty(Request.QueryString["disabled"])) ? false : Convert.ToBoolean(Request.QueryString["disabled"])
            };
            
            if (result.Date != DateNull && result.DateEnd == DateNull)
            {
                DateTime OneDay = (DateTime)result.Date;
                result.DateEnd = OneDay.AddDays(1);
            }

            return result;
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultiButtonAttribute : ActionNameSelectorAttribute
        {
            public string MatchFormKey { get; set; }
            public string MatchFormValue { get; set; }
            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                return controllerContext.HttpContext.Request[MatchFormKey] != null &&
                    controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue;
            }
        }
    }
}