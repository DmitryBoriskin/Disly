using Disly.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;
using IServ;
using System.IO;

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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-presscentr-org")]
        public ActionResult DownloadPressOrg()
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";


            model.StatMaterialsOrg = _cmsRepository.getStatisticMaterials(filter);

            string _Path = Settings.UserFiles + "main/statistic/";

            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(Server.MapPath(_Path)); }
            string FullName = _Path + "StatPressCentrOrg" + DateTime.Today.ToString("ddMMyyyy") + ".csv";

            userMessage.info = "Документ создан - <a href="+ FullName + " targe='_blank'>Ссылка на скачивание</a>";
            userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            model.ErrorInfo = userMessage;
            

            string AbonentInfo = "Организация;Все;Анонсы;Актуально;Новости;Сми о нас;Мастер классы;Наши гости;Мероприятия;Фото;Видео;Новое в медицине\r\n";
            foreach (var item in model.StatMaterialsOrg)
            {
                AbonentInfo +=  item.Title 
                         +";" + item.CountAll
                         +";" + item.CountAnnouncement
                         +";" + item.CountActual
                         +";" + item.CountNews
                         +";" + item.CountSmi
                         +";" + item.CountMasterClasses
                         +";" + item.CountGuests
                         +";" + item.CountEvents
                         +";" + item.CountPhoto
                         +";" + item.CountVideo
                         +";" + item.CountNewInMedicin;

                AbonentInfo += "\r\n";
            }            
            Functions.SaveStringInFile(AbonentInfo, Server.MapPath(FullName));

            return View("index",model);
        }

        
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-presscentr-gs")]
        public ActionResult DownloadPressGs()
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";


            model.StatMaterialsGs = _cmsRepository.getStatisticMaterialsGs(filter);

            string _Path = Settings.UserFiles + "main/statistic/";

            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(Server.MapPath(_Path)); }
            string FullName = _Path + "StatPressCentrGs" + DateTime.Today.ToString("ddMMyyyy") + ".csv";

            userMessage.info = "Документ создан - <a href=" + FullName + " targe='_blank'>Ссылка на скачивание</a>";
            userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            model.ErrorInfo = userMessage;


            string AbonentInfo = "Организация;Все;Анонсы;Актуально;Новости;Сми о нас;Мастер классы;Наши гости;Мероприятия;Фото;Видео;Новое в медицине\r\n";
            foreach (var item in model.StatMaterialsGs)
            {
                AbonentInfo += item.Title
                         + ";" + item.CountAll
                         + ";" + item.CountAnnouncement
                         + ";" + item.CountActual
                         + ";" + item.CountNews
                         + ";" + item.CountSmi
                         + ";" + item.CountMasterClasses
                         + ";" + item.CountGuests
                         + ";" + item.CountEvents
                         + ";" + item.CountPhoto
                         + ";" + item.CountVideo
                         + ";" + item.CountNewInMedicin;

                AbonentInfo += "\r\n";
            }
            Functions.SaveStringInFile(AbonentInfo, Server.MapPath(FullName));

            return View("index", model);

        }


        
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-feed-back")]
        public ActionResult DownloadFeedBackStat()
        {

            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            model.StatFeedBack = _cmsRepository.getStatisticFeedBack();

            string _Path = Settings.UserFiles + "main/statistic/";

            if (!Directory.Exists(_Path)) { Directory.CreateDirectory(Server.MapPath(_Path)); }
            string FullName = _Path + "StatFeedBack" + DateTime.Today.ToString("ddMMyyyy") + ".csv";

            userMessage.info = "Документ создан - <a href=" + FullName + " targe='_blank'>Ссылка на скачивание</a>";
            userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            model.ErrorInfo = userMessage;

            string AbonentInfo = "Медицинские организации;Поступившие вопросы;Отвеченые вопросы;Не отвеченые вопросы;Поступившие отзывы;Опубликованные отзывы;Неопбуликованные отзывы\r\n";

            foreach (var item in model.StatFeedBack)
            {
                AbonentInfo += item.Title
                         + ";" + item.RewiewCount
                         + ";" + item.RewiewAnswerCount
                         + ";" + item.RewiewNoAnswerCount
                         + ";" + item.AppealCount
                         + ";" + item.AppealPublish
                         + ";" + item.AppealNoPublish;

                AbonentInfo += "\r\n";
            }
            Functions.SaveStringInFile(AbonentInfo, Server.MapPath(FullName));

            return View("index", model);
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