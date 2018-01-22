using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
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
    public class VacancyController : CoreController
    {
        VacanciesViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter();

            model = new VacanciesViewModel()
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


        /// <summary>
        /// GET: Список событий
        /// </summary>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult Index(string category, string type)
        {
            // Наполняем модель данными
            if (Domain == "main")
                filter.Domain = null;
            model.List = _cmsRepository.getVacanciesList(filter);
            return View(model);
        }

        /// <summary>
        /// GET: Форма редактирования/добавления записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getVacancy(Id);
            ViewBag.DataPath = Settings.UserFiles + Domain + "/vacancy/"+Id.ToString()+"/";
            if (model.Item == null)
                model.Item = new VacancyModel()
                {
                    Date = DateTime.Now
                };
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
            query = addFiltrParam(query, "filter", filter);
            if (enabeld) query = addFiltrParam(query, "enabeld", String.Empty);
            else query = addFiltrParam(query, "enabeld", enabeld.ToString().ToLower());

            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);

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
            query = addFiltrParam(query, "page", String.Empty);

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
        public ActionResult Save(Guid Id, VacanciesViewModel bindData)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                var res = false;
                var getFeedback = _cmsRepository.getVacancy(Id);

                bindData.Item.Id = Id;
                //Определяем Insert или Update
                if (getFeedback != null)
                {
                    res = _cmsRepository.updateCmsVacancy(bindData.Item);
                    userMessage.info = "Запись обновлена";
                }
                else
                {
                    res = _cmsRepository.insertCmsVacancy(bindData.Item);
                    userMessage.info = "Запись добавлена";
                }
                //Сообщение пользователю
                if (res)
                {
                    string currentUrl = Request.Url.PathAndQuery;
                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                     new ErrorMassegeBtn { url = StartUrl, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = currentUrl, text = "ок"}
                    };
                }
                else
                {
                    userMessage.info = "Произошла ошибка";
                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                     new ErrorMassegeBtn { url = StartUrl, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false"}
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

            //model.Item = _cmsRepository.getFeedback(Id);
            model.ErrorInfo = userMessage;

            return View("Item", model);
        }

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

            model.Item = _cmsRepository.getVacancy(Id);
            if (model.Item != null)
            {
                var res = _cmsRepository.deleteCmsVacancy(Id);
                if (res)
                {
                    // записываем информацию о результатах
                    userMessage.title = "Информация";
                    userMessage.info = "Запись удалена";

                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                        new ErrorMassegeBtn { url = StartUrl, text = "ок" }
                    };
                    model.ErrorInfo = userMessage;
                    model.Item = new VacancyModel()
                    {
                        Id = Id,
                        Date = DateTime.Now
                    };
                    return View(model);
                }
            }

            model.ErrorInfo = userMessage;

            return View(model);

        }
    }
}