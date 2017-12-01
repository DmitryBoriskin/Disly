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
            model.List = _cmsRepository.getMainSpecialistList(filter);

            #region администратор сайта
            if (model.Account.Group.ToLower() == "admin")
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
            if (model.Account.Group.ToLower() == "admin")
            {
                ViewBag.MainSpecId = mainSpecialist;
                if (mainSpecialist != null && !id.Equals((Guid)mainSpecialist))
                {
                    return RedirectToAction("Item", new { id = mainSpecialist });
                }
            }
            #endregion

            model.Item = _cmsRepository.getMainSpecialistItem(id);

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

            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, MainSpecialistViewModel binData)
        {
            ErrorMassege userMessage = new ErrorMassege();
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
            query = addFiltrParam(query, "page", String.Empty);

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
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]
            {
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }
    }
}