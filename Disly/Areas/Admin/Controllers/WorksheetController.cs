using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class WorksheetController : CoreController
    {
        AnketaViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            ViewBag.DataPath = Settings.UserFiles + Domain + "/Worksheet/";

            filter = getFilter();

            model = new AnketaViewModel()
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
            #endregion
        }


        /// <summary>
        /// GET: Список событий
        /// </summary>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult Index(string category, string type)
        {
            // Наполняем модель данными
            var lastAnketaId = Guid.NewGuid();

            var afilter = FilterParams.Extend<AnketaFilter>(filter);
            model.List = _cmsRepository.getAnketasList(afilter);

            //if (model.List != null && model.List.Data != null && model.List.Data.Count() > 0)
            //    lastAnketaId = model.List.Data.First().Id;



            //return RedirectToAction("Item", new { ID = lastAnketaId });
            return View(model);
        }

        /// <summary>
        /// GET: Форма редактирования/добавления события
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getAnketaItem(Id);
            ViewBag.DataPath = Settings.UserFiles + Domain + "/Worksheet/" + Id.ToString() + "/";
            if (model.Item == null)
                model.Item = new AnketaModel()
                {
                    Id = Id,
                    DateBegin = DateTime.Now
                };

            //Заполняем для модели связи с другими объектами
            //var eventFilter = FilterParams.Extend<EventFilter>(filter);
            //eventFilter.RelId = Id;
            //eventFilter.RelType = ContentType.EVENT;
            //var eventsList = _cmsRepository.getEventsList(eventFilter);

            //model.Item.Links = new ObjectLinks()
            //{
            //    Sites = (eventsList != null) ? eventsList.Data : null,
            //};

            ViewBag.Backlink = StartUrl + Request.Url.Query;
            return View("Item", model);
        }


        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string filter, bool enabeld, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "filter", filter);
            if (enabeld) query = AddFiltrParam(query, "enabeld", String.Empty);
            else query = AddFiltrParam(query, "enabeld", enabeld.ToString().ToLower());

            query = AddFiltrParam(query, "page", String.Empty);
            query = AddFiltrParam(query, "size", size);

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

        /// <summary>
        /// Создание новой записи
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            //  При создании записи сбрасываем номер страницы
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "page", String.Empty);

            return Redirect(StartUrl + "Item/" + Guid.NewGuid() + "/" + query);
        }

        /// <summary>
        /// Сохранение изменений в записи
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="bindData"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, AnketaViewModel bindData, HttpPostedFileBase upload)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                var res = false;
                var getAnketa = _cmsRepository.getAnketaItem(Id);

                bindData.Item.Id = Id;
                bindData.Item.Alias = "anketa_" + Id.ToString();
                
                //Определяем Insert или Update
                if (getAnketa != null)
                {
                    userMessage.info = "Запись обновлена";
                    res = _cmsRepository.updateCmsAnketa(bindData.Item);
                }
                else
                {
                    userMessage.info = "Запись добавлена";
                    bindData.Item.ContentLink = Guid.Parse("7ce963d1-9740-4594-99f2-d210052f97f6");
                    bindData.Item.ContentLinkType = ContentLinkType.SITE.ToString().ToLower();
                    res = _cmsRepository.insertCmsAnketa(bindData.Item);
                }
                //Сообщение пользователю
                if (res)
                {
                    string currentUrl = Request.Url.PathAndQuery;
                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                         new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                         new ErrorMassegeBtn { url = currentUrl, text = "ок"}
                    };
                }
                else
                {
                    userMessage.info = "Произошла ошибка";
                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                         new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                         new ErrorMassegeBtn {  url = "#", text = "ок", action = "false"}
                    };
                }
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            }

            model.Item = _cmsRepository.getAnketaItem(Id);

            model.ErrorInfo = userMessage;

            return View("Item", model);
        }

        //Удалить!!!
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {
            // записываем информацию о результатах
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";
            //В случае ошибки
            userMessage.info = "Ошибка, Запись не удалена";
            userMessage.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.Item = _cmsRepository.getAnketaItem(Id);
            if (model.Item != null)
            {
                
                var res = _cmsRepository.deleteCmsAnketa(Id);
                if (res)
                {
                   
                    // записываем информацию о результатах
                    userMessage.title = "Информация";
                    userMessage.info = "Запись удалена";

                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                        new ErrorMassegeBtn { url = StartUrl, text = "ок" }
                    };
                }
            }

            model.ErrorInfo = userMessage;

            return View(model);
        }

    }
}