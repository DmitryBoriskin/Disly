﻿using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class VoteController : CoreController
    {
        VoteViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new VoteViewModel()
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

            //Справочник всех доступных категорий
            MaterialsGroup[] GroupsValues = _cmsRepository.getAllMaterialGroups();
            ViewBag.AllGroups = GroupsValues;

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Vote
        public ActionResult Index(string category, string type)
        {
            // Наполняем фильтр значениями
            var filter = getFilter();
            model.List = _cmsRepository.getVoteList(filter);
            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            ViewBag.VoteId = Id.ToString();
            model.Item = _cmsRepository.getVoteItem(Id);
            ViewBag.DataPath = Settings.UserFiles + Domain + "/Vote/" + Id.ToString() + "/";
            if (model.Item == null)
            {
                ViewBag.DateStart = DateTime.Today;
            }
            return View("Item", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="disabled"></param>
        /// <param name="size"></param>
        /// <param name="date"></param>
        /// <param name="dateend"></param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool enabled, string size, DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "searchtext", searchtext);
            query = AddFiltrParam(query, "disabled", (!enabled).ToString().ToLower());
            if (datestart.HasValue)
                query = AddFiltrParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());
            if (dateend.HasValue)
                query = AddFiltrParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());
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

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            //  При создании записи сбрасываем номер страницы
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "page", String.Empty);

            return Redirect(StartUrl + "Item/" + Guid.NewGuid() + "/" + query);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, VoteViewModel bindData)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            bindData.Item.Id = Id;



            if (ModelState.IsValid)
            {
                var getVote = _cmsRepository.getVoteItem(Id);

                //Определяем Insert или Update
                if (getVote != null)
                {
                    if (_cmsRepository.updVote(Id, bindData.Item))
                    {
                        userMessage.info = "Запись обновлена";
                    }
                    else
                        userMessage.info = "Произошла ошибка";
                }
                else
                {
                    if (_cmsRepository.insVote(Id, bindData.Item))
                    {
                        userMessage.info = "Запись создана";
                    }
                    else
                        userMessage.info = "Произошла ошибка";

                }

                model.Item = _cmsRepository.getVoteItem(Id);
                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "/admin/vote/item/"+Id, text = "ок",}
                 };
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";
                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
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
            ErrorMessage userMassege = new ErrorMessage
            {
                title = "Информация"
            };
            if (_cmsRepository.delVote(Id))
                userMassege.info = "Запись Удалена";
            else
                userMassege.info = "Произошла ошибка";

            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "ок"}
            };

            model.ErrorInfo = userMassege;

            //return RedirectToAction("Index");
            return View("Item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-new-answer")]
        public ActionResult AddAnswer()
        {
            string IdVote = Request["Item_VoteId"];
            string Variant = Request["s_answer"];
            _cmsRepository.insAnswer(Guid.Parse(IdVote), Variant);

            return Redirect(((System.Web.HttpRequestWrapper)Request).RawUrl);
        }
        /// <summary>
        /// удаление варианта ответа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult delanswer(string id)
        {
            _cmsRepository.delVoteAnswer(Guid.Parse(id));
            return null;
        }


    }
}