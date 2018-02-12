﻿using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class MainSpecialistController : CoreController
    {
        MainSpecialistViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);


            ViewBag.DataPath = Settings.UserFiles + Domain + "/mainspecialist/";

            // наполняем фильтр
            filter = getFilter();

            model = new MainSpecialistViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                EmployeePostList = _cmsRepository.getEmployeePosts()
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

        // GET: Admin/MainSpecialist
        public ActionResult Index()
        {
            // наполняем модель данными
            var sfilter = FilterParams.Extend<MainSpecialistFilter>(filter);
            model.List = _cmsRepository.getMainSpecialistList(sfilter);

            #region администратор сайта
            if (model.Account.Group == "admin")
            {
                if (mainSpecialist != null)
                {
                    return RedirectToAction("Item", new { id = mainSpecialist });
                }

                return View(model);
            }
            #endregion

            ViewBag.MainSpecId = Guid.NewGuid();

            return View(model);
        }

        // GET: Admin/MainSpecialist/Item/{id}
        public ActionResult Item(Guid id, string specialisations)
        {
            #region администратор сайта
            if (model.Account.Group == "admin")
            {
                ViewBag.MainSpecId = mainSpecialist;
                if (mainSpecialist != null && !id.Equals((Guid)mainSpecialist))
                {
                    return RedirectToAction("Item", new { id = mainSpecialist });
                }
            }
            #endregion

            model.Item = _cmsRepository.getMainSpecialistItem(id);

            ViewBag.DataPath = ViewBag.DataPath + id.ToString() + "/";

            if (model.Item != null)
            {
                IEnumerable<int> specs;
                if (specialisations != null)
                {
                    specs = specialisations.Split(',').Select(Int32.Parse).ToArray();
                    model.Item.Specialisations = specs.ToArray();
                }
                else
                {
                    specs = model.Item.Specialisations;
                }

                // список сотрудников для данных специализаций
                model.EmployeeList = _cmsRepository.getEmployeeList(specs.ToArray());
            }

            // список всех сотрудников
            model.AllDoctors = _cmsRepository.getEmployeeList();

            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, MainSpecialistViewModel binData)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            bool result = false;

            if (ModelState.IsValid)
            {
                // проверяем на существование главного специалиста
                bool isExist = _cmsRepository.getMainSpecialistItem(id) != null;
                binData.Item.Id = id;

                if (isExist)
                {
                    result = _cmsRepository.updateMainSpecialist(binData.Item);
                }
                else
                {
                    result = _cmsRepository.createMainSpecialist(binData.Item);
                }

                //Сообщение пользователю
                if (result)
                    userMessage.info = "Запись обновлена";
                else
                    userMessage.info = "Произошла ошибка";

                userMessage.buttons = new ErrorMassegeBtn[]
                {
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMessage.buttons = new ErrorMassegeBtn[]
                {
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            string specialisations = null;

            if (binData.Item.Specialisations != null)
            {
                specialisations = string.Join(",", binData.Item.Specialisations);
            }

            if (specialisations != null)
                return RedirectToAction("Item", new { id = id, specialisations = specialisations });
            else
            {
                model.ErrorInfo = userMessage;
                model.Item = _cmsRepository.getMainSpecialistItem(id);
                return View("Item", model);
            }
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            var result = _cmsRepository.deleteMainSpecialist(id);

            // записываем информацию о результатах
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]
            {
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;

            return RedirectToAction("Index");
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
        public ActionResult Search(string searchtext, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "searchtext", searchtext);
            query = AddFiltrParam(query, "page", String.Empty);
            query = AddFiltrParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }


        //Получение списка организаций по параметрам для отображения в модальном окне
        [HttpGet]
        public ActionResult SpecListModal(Guid objId, ContentType objType)
        {
            var filtr = new MainSpecialistFilter()
            {
                Domain = null,
                RelId = objId,
                RelType = objType,
                //Size = 1000
            };

            var model = new MainSpecModalViewModel()
            {
                ObjctId = objId,
                ObjctType = objType,
                SpecList = _cmsRepository.getMainSpecWithCheckedFor(filtr),
            };

            return PartialView("Modal/Spec", model);
        }

        [HttpPost]
        public ActionResult UpdateLinkToSpec(ContentLinkModel data)
        {
            if (data != null)
            {
                var res = _cmsRepository.updateContentLink(data);
                if (res)
                    return Json("Success");
            }

            //return Response.Status = "OK";
            return Json("An Error Has occourred"); //Ne
        }
    }
}