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


        protected int? oldId = null;
        protected string siteName = String.Empty;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            redirectUrl = Request.Url.Host;

            if (Domain == "localhost")
            {
                var port = Request.Url.Port;
                redirectUrl = string.Format("{0}:{1}", redirectUrl, port);
            }

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
                    redirectUrl = string.Format("/{0}{1}", menu.Path, menu.Alias);
                }
            }

#warning Если путь типа http://www.med.cap.ru/Page.aspx?id=555860/content

            return RedirectPermanent(redirectUrl);
        }

        public ActionResult News(int? id, int? pg)
        {
            MaterialsModel material = null;
            if (id.HasValue)
            {
                //Ссылки типа: http://www.rkod.med.cap.ru/pg_11/id_918558/News.aspx , http://www.rkod.med.cap.ru/id_918558/News.aspx
                material = _repository.getMaterialsByOldId(id.Value);
            }

            var pageIdstr = Request.Params["id"];
            int newsid = 0;
            if (!string.IsNullOrEmpty(pageIdstr) && int.TryParse(pageIdstr, out newsid))
            {
                //Ссылки типа: http://www.med.cap.ru/News.aspx?id=918579
                material = _repository.getMaterialsByOldId(newsid);
            }

            if (material != null)
            {
                redirectUrl = string.Format("/press/{0}/{1}/{2}/{3}", material.Year, material.Month, material.Day, material.Alias);
            }
            else
            {
                redirectUrl = "/press/";
            }

            return RedirectPermanent(redirectUrl);
        }

        //Hospitals.aspx?lib=557311
        //О нас
        public ActionResult Hospitals()
        {
            return RedirectPermanent("/lpu/");
        }

        //Services.aspx?lib=557238
        public ActionResult Services()
        {
            return RedirectPermanent(redirectUrl);
        }
        //Vacancies.aspx?lib=557281
        public ActionResult Vacancies()
        {
            return RedirectPermanent("/vacancy/");
        }

        //www.cheb-gb1.med.cap.ru
        //О нас
        public ActionResult AboutOrgs()
        {
            return RedirectPermanent("/about/");
        }
        //Как нас найти
        public ActionResult SheduleTravel()
        {
            return RedirectPermanent("/findus/");
        }
        //Контакты
        public ActionResult Contacts()
        {
            return RedirectPermanent("/contacts/");
        }
        //Структура
        public ActionResult Branches()
        {
            return RedirectPermanent("/structure/");
        }
        //Врачи портала
        public ActionResult Doctorsnew()
        {
            return RedirectPermanent("/portaldoctors/");
        }

        //Врачи конкретной лпу
        public ActionResult Doctors()
        {
            //нет соответствия докторов
            //Doctors.aspx?id=3943
            //http://www.cheb-gb1.med.cap.ru/id_1786/Doctors.aspx
            return RedirectPermanent("/doctors/");
        }
        //Расписание работы
        public ActionResult Timetable()
        {
            return RedirectPermanent(redirectUrl);
        }

        //Вопросы и ответы
        public ActionResult Questions()
        {
            return RedirectPermanent("/feedback/appeallist/");
        }
        //Архив
        public ActionResult Archives()
        {
            return RedirectPermanent(redirectUrl);
        }

        //Политика персональных данных
        public ActionResult PersonalDataPolicy()
        {
            return RedirectPermanent(redirectUrl);
        }

    }
}