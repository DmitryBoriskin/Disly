using cms.dbase;
using System;
using System.Configuration;
using System.Reflection;
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

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            try { Domain = _repository.getSiteId(Request.Url.Host.ToLower().Replace("www.", "")); }
            catch
            {
                if (Request.Url.Host.ToLower().Replace("www.", "") != ConfigurationManager.AppSettings["BaseURL"]) filterContext.Result = Redirect("/Error/");
                else Domain = String.Empty;
            }

            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();

            ViewName = _repository.getView(Domain, ControllerName);
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
    }
}

