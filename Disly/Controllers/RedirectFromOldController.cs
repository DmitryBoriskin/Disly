using System;
using System.Web.Mvc;
using cms.dbase;
using cms.dbModel.entity;

namespace Disly.Controllers
{

    public class RedirectFromOldController : RootController
    {
        protected string baseUrl = Settings.BaseURL;
        protected string redirectUrl = "";
        protected string siteId = "";
        protected string action = "";

        //public string Domain;
        //public string ControllerName;
        //public string ActionName;
        //public string ViewName;
        //public string StartUrl;

        protected int? oldId = null;
        protected string siteName = String.Empty;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var Domain = Request.Url.Host;
            ActionName = RouteData.Values["action"].ToString().ToLower();

            redirectUrl = string.Format("http://{0}", baseUrl);
            var port = Request.Url.Port;
            if (baseUrl == "localhost")
                redirectUrl = string.Format("{0}:{1}", redirectUrl, port);

        }

        public ActionResult Index()
         {
            return Redirect(redirectUrl);
        }

        public ActionResult Page()
        {
            var pageIdstr = Request.Params["id"];

            //Если Id это число в адресе http://www.med.cap.ru/Page.aspx?id=555860
            int pageId;
            if (int.TryParse(pageIdstr, out pageId))
            {
                var menu = _repository.getSiteMapByOldId(pageId);
                if (menu != null)
                {
                    redirectUrl = string.Format("http://{0}{1}{2}", Domain, menu.Path, menu.Alias);
                }
            }

#warning Если путь типа http://www.med.cap.ru/Page.aspx?id=555860/content

            return Redirect(redirectUrl);
        }

        public ActionResult News()
        {
            var pageIdstr = Request.Params["id"];

            int pageId;
            if (int.TryParse(pageIdstr, out pageId))
            {
                var material = _repository.getMaterialsByOldId(pageId);
                if (material != null)
                {
                    redirectUrl = string.Format("http://{0}/{1}/{2}/{3}/{4}/{5}", Domain, "press", material.Year, material.Month, material.Day, material.Alias);
                }
            }
            return Redirect(redirectUrl);
        }

        //Hospitals.aspx?lib=557311
        //О нас
        public ActionResult Hospitals()
        {
            return Redirect(redirectUrl);
        }

        //Services.aspx?lib=557238
        public ActionResult Services()
        {
            return Redirect(redirectUrl);
        }
        //Vacancies.aspx?lib=557281
        public ActionResult Vacancies()
        {
            return Redirect(redirectUrl);
        }


        //www.cheb-gb1.med.cap.ru
        //О нас
        public ActionResult AboutOrgs()
        {
            return Redirect("/about");
        }
        //Как нас найти
        public ActionResult SheduleTravel()
        {
            return Redirect("/findus");
        }
        //Контакты
        public ActionResult Contacts()
        {
            return Redirect("/contacts");
        }
        //Структура
        public ActionResult Branches()
        {
            return Redirect("/structure");
        }
        //Врачи
        public ActionResult Doctors()
        {
            return Redirect("/doctors");
        }
        //Расписание работы
        public ActionResult Timetable()
        {
            return Redirect(redirectUrl);
        }
       
        //Вопросы и ответы
        public ActionResult Questions()
        {
            return Redirect("/feedback/appeallist");
        }
        //Архив
        public ActionResult Archives()
        {
            return Redirect(redirectUrl);
        }
       
        //Политика персональных данных
        public ActionResult PersonalDataPolicy()
        {
            return Redirect(redirectUrl);
        }



    }
 }