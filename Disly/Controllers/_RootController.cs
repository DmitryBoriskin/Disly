using cms.dbase;
using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class RootController : Controller
    {
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected FrontRepository _repository { get; private set; }
        
        public string Domain;
        public string ControllerName;
        public string ActionName;
        public string ViewName;
        public string StartUrl;

        protected SitesModel siteModel;
        protected SiteMapModel[] siteMapArray;
        protected BannersModel[] bannerArray;        
        protected List<Breadcrumbs> breadcrumb;
        protected SiteMapModel currentPage;

        protected string MedCap;
        protected string Quote;
        protected string Concept;
        protected string Coordination;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            try { Domain = _repository.getSiteId(Request.Url.Host.ToLower().Replace("www.", "")); }
            catch
            {
                if (Request.Url.Host.ToLower().Replace("www.", "") != ConfigurationManager.AppSettings["BaseURL"]) filterContext.Result = Redirect("/Error/");
                else Domain = String.Empty;
            }

            #region Получаем данные из адресной строки
            string UrlPath = "/" + (String)RouteData.Values["path"];
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion
            currentPage = _repository.getSiteMap(_path, _alias, Domain);
            
            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            ViewName = _repository.getView(Domain, ControllerName);

            siteModel = _repository.getSiteInfo(Domain);
            siteMapArray = _repository.getSiteMapList(Domain);            

            breadcrumb=_repository.getBreadCrumbCollection(((System.Web.HttpRequestWrapper)Request).RawUrl, Domain);
            bannerArray = _repository.getBanners(Domain);

            ViewBag.MedCap = MedCap = Settings.MedCap;
            ViewBag.Quote = Quote = Settings.Quote;
            ViewBag.Concept = Concept = Settings.Concept;
            ViewBag.Coordination = Coordination = Settings.Coordination;
            ViewBag.ControllerName = ControllerName;
        }

        public RootController()
        {
            _repository = new FrontRepository("cmsdbConnection");
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
        
        public FilterParams getFilter(int defaultPageSize = 20)
        {
            string return_url = HttpUtility.UrlDecode(Request.Url.Query);
            // если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["page"]) == 1) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            }
            catch
            {
                return_url = addFiltrParam(return_url, "page", String.Empty);
            }
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["size"]) == defaultPageSize) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            }
            catch
            {
                return_url = addFiltrParam(return_url, "size", String.Empty);
            }
            return_url = (!Convert.ToBoolean(Request.QueryString["disabled"])) ? addFiltrParam(return_url, "disabled", String.Empty) : return_url;
            return_url = String.IsNullOrEmpty(Request.QueryString["searchtext"]) ? addFiltrParam(return_url, "searchtext", String.Empty) : return_url;
            // Если парамметры из адресной строки равны значениям по умолчанию - удаляем их из URL
            if (return_url.ToLower() != HttpUtility.UrlDecode(Request.Url.Query).ToLower())
                Response.Redirect(StartUrl + return_url);

            DateTime? DateNull = new DateTime?();

            FilterParams result = new FilterParams()
            {
                Domain = Domain,
                Page = (Convert.ToInt32(Request.QueryString["page"]) > 0) ? Convert.ToInt32(Request.QueryString["page"]) : 1,
                Size = (Convert.ToInt32(Request.QueryString["size"]) > 0) ? Convert.ToInt32(Request.QueryString["size"]) : defaultPageSize,
                Type = (String.IsNullOrEmpty(Request.QueryString["type"])) ? String.Empty : Request.QueryString["type"],
                Category = (String.IsNullOrEmpty(Request.QueryString["category"])) ? String.Empty : Request.QueryString["category"],
                Group = (String.IsNullOrEmpty(Request.QueryString["group"])) ? String.Empty : Request.QueryString["group"],
                Lang = (String.IsNullOrEmpty(Request.QueryString["lang"])) ? String.Empty : Request.QueryString["lang"],
                Date = (String.IsNullOrEmpty(Request.QueryString["date"])) ? DateNull : DateTime.Parse(Request.QueryString["date"]),
                DateEnd = (String.IsNullOrEmpty(Request.QueryString["dateend"])) ? DateNull : DateTime.Parse(Request.QueryString["dateend"]),
                SearchText = (String.IsNullOrEmpty(Request.QueryString["searchtext"])) ? String.Empty : Request.QueryString["searchtext"],
                Disabled = (String.IsNullOrEmpty(Request.QueryString["disabled"])) ? false : Convert.ToBoolean(Request.QueryString["disabled"])
            };

#warning  Зачем прибавляли 1 день?
            //if (result.Date != DateNull && result.DateEnd == DateNull)
            //{
            //    result.DateEnd = ((DateTime)result.Date).AddDays(1);
            //}

            return result;
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
    }
}

