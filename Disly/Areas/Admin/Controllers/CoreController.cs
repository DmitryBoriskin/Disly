using cms.dbase;
using cms.dbModel.entity;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Portal.Code;
using System.Drawing.Imaging;
using System.Collections.Generic;

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
        public AccountModel AccountInfo;
        public SettingsModel SettingsInfo;
        public ResolutionsModel UserResolutionInfo;//права пользователей
        public cmsLogModel[] LogInfo;
        public SitesModel SiteInfo;
        public string ControllerName;
        public string ActionName;

        public int page_size = 40;
        public int last_items = 10;

        protected Guid? orgId;
        protected Guid? mainSpecialist;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            cmsRepository.DislyEvent += CmsRepository_DislyEvent;

            base.OnActionExecuting(filterContext);

            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            Guid _PageId;

            try
            {
                Domain = _cmsRepository.getSiteId(Request.Url.Host.ToLower().Replace("www.", ""));
            }
            catch (Exception ex)
            {
                if (Request.Url.Host.ToLower().Replace("www.", "") != ConfigurationManager.AppSettings["BaseURL"])
                    filterContext.Result = Redirect("/Error/");
                else Domain = String.Empty;

                AppLogger.Debug("CoreController: Не получилось определить Domain", ex);
            }
            ViewBag.Domain = Domain;

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

            if (ControllerName.ToLower() != "templates" && ControllerName.ToLower() != "documents" && ControllerName.ToLower() != "services")
            {
                UserResolutionInfo = _accountRepository.getCmsUserResolutioInfo(_userId, ControllerName);
                if (UserResolutionInfo == null)
                    throw new Exception("У вас нет прав доступа к странице!");

                // Если нет прав на проссмотр, то направляем на главную
                if (!UserResolutionInfo.Read)
                    filterContext.Result = Redirect("/Admin/");
            }

            #endregion

            #region Ограничение пользователя (не администратора портала и не разработчика) на доступ только к своим сайтам (доменам)
            int IsRedirect = 0;

            SiteInfo = _cmsRepository.getSite(Domain); // информация по сайту

            if (AccountInfo.Group != "developer" && AccountInfo.Group != "administrator")
            {
                if (AccountInfo.Domains != null && AccountInfo.Domains.Count() > 0)
                {
                    foreach (DomainList domain in AccountInfo.Domains)
                    {
                        if (domain.SiteId == Domain) { IsRedirect++; }
                    }
                }


                //перенаправляем на первый из своих доменов
                if (IsRedirect == 0)
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

            // идентификатор организации
            orgId = _cmsRepository.getOrgLinkByDomain();

            // идентификатор главного специалиста
            mainSpecialist = _cmsRepository.getMainSpecLinkByDomain(Domain);
        }

        private void CmsRepository_DislyEvent(object sender, DislyEventArgs e)
        {
            switch (e.EventLevel)
            {
                case LogLevelEnum.Debug:
                    AppLogger.Debug(e.Message, e.Exception);
                    break;
                case LogLevelEnum.Error:
                    AppLogger.Error(e.Message, e.Exception);
                    break;
                case LogLevelEnum.Warn:
                    AppLogger.Warn(e.Message, e.Exception);
                    break;
                case LogLevelEnum.Info:
                    AppLogger.Info(e.Message, e.Exception);
                    break;
            }
        }

        public CoreController()
        {
            _accountRepository = new AccountRepository("cmsdbConnection");

            Guid userId = Guid.Empty;
            var domainUrl = "";

            if (System.Web.HttpContext.Current != null)
            {
                var context = System.Web.HttpContext.Current;

                if (context.Request != null && context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Host))
                    domainUrl = context.Request.Url.Host.ToLower().Replace("www.", "");

                if (context.User != null && context.User.Identity != null && !string.IsNullOrEmpty(context.User.Identity.Name))
                {
                    try
                    {
                        userId = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Не удалось определить идентификатор пользователя" + ex);
                    }
                }
            }
            _cmsRepository = new cmsRepository("cmsdbConnection", userId, RequestUserInfo.IP, domainUrl);

        }

        /// <summary>
        /// Формируем новую строку запроса из queryStr="?size=40&searchtext=поиск&page=2" исключая exclude
        /// </summary>
        /// <param name="queryStr"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public string UrlQueryExclude(string queryStr, string exclude)
        {
            if (string.IsNullOrEmpty(queryStr))
                return "";

            var qparams = HttpUtility.ParseQueryString(queryStr);

            var queryParams = new Dictionary<string, string>();
            if (qparams.AllKeys != null && qparams.AllKeys.Count() > 0)
            {
                foreach (var p in qparams.AllKeys)
                {
                    if (p != null)
                        queryParams.Add(p, qparams[p]);
                }
            }

            var urlParams = String.Join("&", queryParams
                            .Where(p => p.Key != exclude)
                            .Select(p => String.Format("{0}={1}", p.Key, p.Value))
                            );

            return urlParams;
        }

        public string AddFiltrParam(string query, string name, string val)
        {
            //var q1 = query;
            ////string search_Param = @"\b" + name + @"=[\w]*[\b]*&?";
            //string search_Param = @"\b" + name + @"=(.*?)(&|$)";
            //string normal_Query = @"&$";

            //Regex delParam = new Regex(search_Param, RegexOptions.CultureInvariant);
            //Regex normalQuery = new Regex(normal_Query);
            //query = delParam.Replace(query, String.Empty);
            //query = normalQuery.Replace(query, String.Empty);

            //if (val != String.Empty)
            //{
            //    if (query.IndexOf("?") > -1) query += "&" + name + "=" + val;
            //    else query += "?" + name + "=" + val;
            //}

            //query = query.Replace("?&", "?").Replace("&&", "&");

            //return query;

            var returnUrl = "";
            //Формируем строку с параметрами фильтра без пейджера и параметра(чтоб убрать дублирование)
            returnUrl = UrlQueryExclude(query, "page");
            returnUrl = UrlQueryExclude(query, name);

            var page = 1;
            if(!string.IsNullOrEmpty(Request.QueryString["page"]))
                int.TryParse(Request.QueryString["page"], out page);

            //Добавляем параметр name
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(val))
                returnUrl = !string.IsNullOrEmpty(returnUrl)
                    ? string.Format("{0}&{1}={2}", returnUrl, name, val)
                    : string.Format("{0}={1}", name, val);

            //Добавляем page
            if (page > 1)
                returnUrl = !string.IsNullOrEmpty(returnUrl)
                    ? string.Format("{0}&page={1}", returnUrl, page) 
                    : string.Format("page={0}", page);


            if (!string.IsNullOrEmpty(returnUrl))
            {
                var s = (returnUrl.IndexOf("?") > -1)? "&" : "?";
                returnUrl = s + returnUrl;
            }

            return returnUrl.ToLower();
        }


        /// <summary>
        /// Формирование фильтра по сторке url
        /// </summary>
        /// <param name="defaultPageSize"></param>
        /// <returns></returns>
        public FilterParams getFilter(int defaultPageSize = 20)
        {

            //string return_url = HttpUtility.UrlDecode(Request.Url.Query);
            //// если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            //try
            //{
            //    return_url = (Convert.ToInt32(Request.QueryString["page"]) == 1) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            //}
            //catch
            //{
            //    return_url = addFiltrParam(return_url, "page", String.Empty);
            //}
            //try
            //{
            //    return_url = (Convert.ToInt32(Request.QueryString["size"]) == defaultPageSize) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            //}
            //catch
            //{
            //    return_url = addFiltrParam(return_url, "size", String.Empty);
            //}
            //return_url = (!Convert.ToBoolean(Request.QueryString["disabled"])) ? addFiltrParam(return_url, "disabled", String.Empty) : return_url;
            //return_url = String.IsNullOrEmpty(Request.QueryString["searchtext"]) ? addFiltrParam(return_url, "searchtext", String.Empty) : return_url;
            //// Если парамметры из адресной строки равны значениям по умолчанию - удаляем их из URL
            //if (return_url.ToLower() != HttpUtility.UrlDecode(Request.Url.Query).ToLower())
            //    Response.Redirect(StartUrl + return_url);

            FilterParams fltr = new FilterParams()
            {
                Domain = Domain,
                Type = Request.QueryString["type"],
                Category = Request.QueryString["category"],
                Group = Request.QueryString["group"],
                Lang = Request.QueryString["lang"],
                SearchText = Request.QueryString["searchtext"],
                Page = 1,
                Size = defaultPageSize,
                Disabled = null
            };

            if (!String.IsNullOrEmpty(Request.QueryString["disabled"]))
            {
                var disabled = false;
                bool.TryParse(Request.QueryString["disabled"], out disabled);
                fltr.Disabled = disabled;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["page"]))
            {
                int page = 1;
                int.TryParse(Request.QueryString["page"], out page);
                if (page > 1)
                    fltr.Page = page;
            }
            if (!String.IsNullOrEmpty(Request.QueryString["size"]))
            {
                int size = defaultPageSize;
                int.TryParse(Request.QueryString["size"], out size);
                if (size > 0)
                    fltr.Size = size;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["datestart"]))
            {
                DateTime datestart = DateTime.MinValue;
                var res = DateTime.TryParse(Request.QueryString["datestart"], out datestart);

                if (datestart != DateTime.MinValue)
                    fltr.Date = datestart;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["dateend"]))
            {
                DateTime dateend = DateTime.MinValue;
                DateTime.TryParse(Request.QueryString["dateend"], out dateend);

                if (dateend != DateTime.MinValue)
                    fltr.DateEnd = dateend;
            }

            return fltr;
        }

        /// <summary>
        /// Проверка прикрепляемых файлов на допустимые форматы
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool AttachedFileExtAllowed(string fileName)
        {
            //new string[] { "jpeg", "jpg", "png", "gif", "pdf", "rtf", "txt", "doc", "docx", "xls", "xlsx", "ods", "odt", "tar", "zip", "7z" };
            string[] exts = Settings.DocTypes.Split(',');

            var ext = fileName.Split('.').Any() ? fileName.Split('.').Last() : "";

            if (!string.IsNullOrEmpty(ext) && exts.Contains(ext.ToLower()))
                return true;

            return false;
        }
        
        /// <summary>
        /// Проверка прикрепляемых изображений на допустимые форматы
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool AttachedPicExtAllowed(string fileName)
        {
            //new string[] { "jpeg", "jpg", "png", "gif"};
            string[] exts = Settings.PicTypes.Split(',');

            var ext = fileName.Split('.').Any() ? fileName.Split('.').Last() : "";

            if (!string.IsNullOrEmpty(ext) && exts.Contains(ext.ToLower()))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            if (codecs != null && codecs.Count() > 0)
            {
                foreach (var enc in codecs)
                {
                    if (enc.MimeType.ToLower() == mimeType.ToLower())
                        return enc;
                }

            }
            return null;
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