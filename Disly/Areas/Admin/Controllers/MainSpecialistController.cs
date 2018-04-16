using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class MainSpecialistController : CoreController
    {
        GSViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);


            ViewBag.DataPath = Settings.UserFiles + Domain + "/mainspecialist/";

            // наполняем фильтр
            filter = getFilter();

            model = new GSViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
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
            var sfilter = FilterParams.Extend<GSFilter>(filter);
            model.List = _cmsRepository.getGSList(sfilter);

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

            model.Item = _cmsRepository.getGSItem(id);
            // Полный список специализаций
            model.Spesializations = _cmsRepository.getEmployeePosts();

            //Это для картинок, вставленных в tinymce
            ViewBag.DataPath = ViewBag.DataPath + id.ToString() + "/";

            //Че за хуйня??? бред без единой проверки
            if (model.Item != null)
            {
                var specs = model.Item.Specialisations;
                // список сотрудников для данных специализаций
                model.EmployeeList = _cmsRepository.getEmployeeList(specs.ToArray());

                model.Item.Specialists = _cmsRepository.getGSMembers(model.Item.Id, GSMemberType.SPEC);
                model.Item.Experts = _cmsRepository.getGSMembers(model.Item.Id, GSMemberType.EXPERT);
            }

            // список всех сотрудников
            model.AllDoctors = _cmsRepository.getEmployeeList();

            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, GSViewModel binData)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            bool result = false;

            if (ModelState.IsValid)
            {
                // проверяем на существование главного специалиста
                bool isExist = _cmsRepository.getGSItem(id) != null;
                binData.Item.Id = id;

                if (isExist)
                {
                    result = _cmsRepository.updateGS(binData.Item);
                }
                else
                {
                    result = _cmsRepository.createGS(binData.Item);
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
                model.Spesializations = _cmsRepository.getEmployeePosts();
                model.Item = _cmsRepository.getGSItem(id);
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
            var result = _cmsRepository.deleteGS(id);

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

        #region Прикрепление/открепление доктора к гс
        /// <summary>
        /// Форма данных доктора для прикрепления к гс 
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewGSMember(Guid objId)
        {
            //Получение главного специалиста
            var mainSpec = _cmsRepository.getGSItem(objId);

            var model = new GSMemberViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            if (mainSpec != null)
            {
                model.Member = new GSMemberModel()
                {
                    GSId = objId,
                    //MemberType = objType
                };

                //if(objType == GSMemberType.SPEC)
                //{
                //    // список сотрудников для специализаций главного специалиста
                //    model.EmployeeList = _cmsRepository.getEmployeeList(mainSpec.Specialisations);
                //}

               model.EmployeeList = _cmsRepository.getEmployeeList();
            }

            return PartialView("Part/AddDoctor", model);
        }

        /// <summary>
        /// Прикрепление/изменение доктора к гс
        /// </summary>
        /// <param name="bindData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveGSMember(GSMemberModel bindData)
        {
            if ( ModelState.IsValid)
            {
                if (bindData.Orgs != null && bindData.Orgs.Count() > 0)
                {
                    foreach(var org in bindData.Orgs)
                    {
                        if(!string.IsNullOrEmpty(org.Url))
                        {
                            org.Url = org.Url.Replace("http://", "");
                            org.Url = org.Url.Replace("https://", "");
                        }
                    }
                } 
                var res = _cmsRepository.addGSMember(bindData);
                if (res)
                    //return Json("Success");
                    return View("Modal/Success");
            }

            //return Response.Status = "OK";
            //return Json("An Error Has Occourred");
            return View("Modal/Error");
        }

        /// <summary>
        /// Удаление доктора из гс
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteGSMember(Guid id)
        {
            var res = _cmsRepository.deleteGSMember(id);
            if (res)
                return Json("Success");

            //return Response.Status = "OK";
            return Json("An Error Has Occourred"); //Ne
        }
        #endregion

        #region Прикрепление/открепление объектов к гс

        /// <summary>
        /// Получение списка гс по параметрам для отображения в модальном окне
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SpecListModal(Guid objId, ContentType objType)
        {
            var filtr = new GSFilter()
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
                SpecList = _cmsRepository.getGSWithCheckedFor(filtr),
            };

            return PartialView("Modal/Spec", model);
        }

        /// <summary>
        /// Прикрепление объектов к гс
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
            return Json("An Error Has Occourred"); //Ne
        }
        #endregion
    }
}