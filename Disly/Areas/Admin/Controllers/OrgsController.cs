using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class OrgsController : CoreController
    {
        //ovp- это вьюха объединяющая в себе структурное подразделение и департамент(отдел)
        OrgsViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter();

            model = new OrgsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Types = _cmsRepository.getOrgTypesList(new OrgTypeFilter() { }),
                DepartmentAffiliations = _cmsRepository.getDepartmentAffiliations(),
                MedicalServices = _cmsRepository.getMedicalServices()
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/Orgs
        public ActionResult Index()
        {
            #region администратор сайта
            //model.Account.Domains
            #endregion

            var orgfilter = FilterParams.Extend<OrgFilter>(filter);
            model.OrgList = _cmsRepository.getOrgs(orgfilter);//+ список организаций
            return View(model);
        }
             
        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool disabled, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "disabled", disabled.ToString().ToLower());
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
            query = addFiltrParam(query, "page", String.Empty);
            return Redirect(StartUrl + "item/" + Guid.NewGuid() + "/" + query);
        }        

        public ActionResult Item(Guid Id)
        {            
            model.Item = _cmsRepository.getOrgItem(Id);    //+ список структур    +списо административного персонала
            
            // типы организаций
            if (model.Item != null)
            {
                ViewBag.Titlecoord = model.Item.ShortTitle;
                ViewBag.Xcoord = model.Item.GeopointX;
                ViewBag.Ycoord = model.Item.GeopointY;
            }
            
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, OrgsViewModel back_model, HttpPostedFileBase upload)
        {
            ErrorMassege userMessege = new ErrorMassege();
            userMessege.title = "Информация";
            model.Item = _cmsRepository.getOrgItem(id);
            #region координаты
            double MapX = 0;
            double MapY = 0;

            if (back_model.Item.GeopointX != null) { MapX = (double)back_model.Item.GeopointX; }
            if (back_model.Item.GeopointY != null) { MapY = (double)back_model.Item.GeopointY; }
            ViewBag.Titlecoord = back_model.Item.ShortTitle;
            ViewBag.Xcoord = MapX;
            ViewBag.Ycoord = MapY;
            try
            {
                if (back_model.Item.Address != String.Empty && (MapX == 0 || MapY == 0))
                {
                    var CoordResult = Spots.Coords(back_model.Item.Address);
                    back_model.Item.GeopointX = CoordResult.GeopointX;
                    back_model.Item.GeopointY = CoordResult.GeopointY;
                }
            }
            catch { }

            #region Логотип
            if (ModelState.IsValid)
            {
                // путь для сохранения изображения
                string savePath = Settings.UserFiles + Settings.OrgDir;

                int width = 80; // ширина
                int height = 0; // высота

                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                    if (!validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        model.ErrorInfo = new ErrorMassege()
                        {
                            title = "Ошибка",
                            info = "Вы не можете загружать файлы данного формата",
                            buttons = new ErrorMassegeBtn[]
                            {
                                new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                            }
                        };

                        return View("Item", model);
                    }

                    Photo photoNew = new Photo()
                    {
                        Name = id.ToString() + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, id.ToString(), width, height)
                    };

                    back_model.Item.Logo = photoNew;
                }
            }
            #endregion

            #endregion
            if (model.Item != null)
            {
                #region обновление
                if (ModelState.IsValid)
                {
                    _cmsRepository.updateOrg(id, back_model.Item); //, AccountInfo.id, RequestUserInfo.IP
                    userMessege.info = "Запись сохранена";
                    userMessege.buttons = new ErrorMassegeBtn[]
                    {
                        new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                        new ErrorMassegeBtn { url = "/admin/orgs/item/"+id, text = "ок", action = "false" }
                    };
                }
                else
                {
                    userMessege.info = "Произошла ошибка";
                    userMessege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
                }
                #endregion
            }
            else
            {
                #region создание
                if (ModelState.IsValid)
                {
                    if (_cmsRepository.insertOrg(id, back_model.Item)) //, AccountInfo.id, RequestUserInfo.IP
                    {
                        userMessege.info = "Запись создана";
                        userMessege.buttons = new ErrorMassegeBtn[]{
                            new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                            new ErrorMassegeBtn { url = "/admin/orgs/item"+id, text = "ок", action = "false" }
                        };
                    }
                    else
                    {
                        userMessege.info = "Произошла ошибка";
                        userMessege.buttons = new ErrorMassegeBtn[]{
                            new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                        };
                    }
                }
                #endregion
            }

            model.Item = _cmsRepository.getOrgItem(id);

            model.ErrorInfo = userMessege;
            return View("Item", model);
        }



        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ErrorMassege userMassage = new ErrorMassege();
            userMassage.title = "Информация";
            if (_cmsRepository.deleteOrg(id)) //, AccountInfo.id, RequestUserInfo.IP
            {
                userMassage.info = "Запись Удалена";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" },
                    new ErrorMassegeBtn { url = "/admin/orgs/", text = "Вернуться в список"}
                };
            }
            else
            {
                userMassage.info = "Произошла ошибка";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            model.ErrorInfo = userMassage;
            return View("item", model);

        }


        // GET: Admin/Orgs/structure/{Guid}
        public ActionResult Structure(Guid id)
        {
            ViewBag.Title = "Структурное подразделение";
            model.StructureItem = _cmsRepository.getStructure(id);//+ список подразделений      
            if (model.StructureItem != null)
            {
                #region координаты
                ViewBag.Titlecoord = model.StructureItem.Title;
                ViewBag.Xcoord = model.StructureItem.GeopointX;
                ViewBag.Ycoord = model.StructureItem.GeopointY;
                #endregion
                model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(id, ViewBag.ActionName);
            }
            return View("Structure", model);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-structure-btn")]
        public ActionResult StructureSave(Guid id, OrgsViewModel back_model)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";
            model.StructureItem = _cmsRepository.getStructure(id);
            if (ModelState.IsValid)
            {

                #region координаты
                double MapX = 0;
                double MapY = 0;

                if (back_model.StructureItem.GeopointX != null) { MapX = (double)back_model.StructureItem.GeopointX; }
                if (back_model.StructureItem.GeopointY != null) { MapY = (double)back_model.StructureItem.GeopointY; }
                ViewBag.Titlecoord = back_model.StructureItem.Title;
                ViewBag.Xcoord = MapX;
                ViewBag.Ycoord = MapY;
                try
                {
                    if (back_model.StructureItem.Adress != String.Empty && (MapX == 0 || MapY == 0))
                    {
                        var CoordResult = Spots.Coords(back_model.StructureItem.Adress);
                        back_model.StructureItem.GeopointX = CoordResult.GeopointX;
                        back_model.StructureItem.GeopointY = CoordResult.GeopointY;
                    }
                }
                catch { }
                #endregion



                if (model.StructureItem == null)
                {
                    #region создание
                    var OrgId = Request.Params["orgid"];
                    if (OrgId != null)
                    {
                        ViewBag.OrgId = OrgId;
                        Guid OrgGuid = Guid.Parse(OrgId);
                        if (_cmsRepository.insertStructure(id, OrgGuid, back_model.StructureItem)) //, AccountInfo.id, RequestUserInfo.IP
                        {
                            userMessage.info = "Запись создана";
                            userMessage.buttons = new ErrorMassegeBtn[]{
                                 new ErrorMassegeBtn { url = "/admin/orgs/structure/"+id, text = "ок"}
                             };
                        }
                        else { userMessage.info = "Произошла ошибка"; }
                    }
                    else { userMessage.info = "Произошла ошибка"; } 
                    #endregion
                }
                else
                {
                    #region обновление                    
                    if (_cmsRepository.updateStructure(id, back_model.StructureItem)) //, AccountInfo.id, RequestUserInfo.IP
                    {
                        userMessage.info = "Запись обновлена";
                        userMessage.buttons = new ErrorMassegeBtn[]{
                                 new ErrorMassegeBtn { url = "/admin/orgs/structure/"+id, text = "ок"}
                             };
                    }
                    else { userMessage.info = "Произошла ошибка"; } 
                    #endregion
                }
            }
            model.ErrorInfo = userMessage;
            return View("Structure", model);
        }
                
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-structure-btn")]
        public ActionResult StructureDelete(Guid id)
        {
            ErrorMassege userMassage = new ErrorMassege();
            userMassage.title = "Информация";
            Guid ParentOrgId= _cmsRepository.getAdministrativ(id).OrgId;            
            if (_cmsRepository.delAdministrativ(id))
            {
                userMassage.info = "Запись Удалена";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" },
                    new ErrorMassegeBtn { url = "/admin/orgs/item/"+ParentOrgId, text = "Вернуться в организацию"}
                };
            }
            else {
                userMassage.info = "Произошла ошибка";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            model.ErrorInfo = userMassage;
            return View("structure", model);
        }


        #region Ovp
        public ActionResult Ovp(Guid id)
        {
            ViewBag.Title = "ОВП/ОФП";
            var OrgId = Request.Params["orgid"];
            model.StructureItem = _cmsRepository.getStructure(id);  
            
            if (model.StructureItem != null)
            {
                if (!model.StructureItem.Ovp)
                {
                    return Redirect("/admin/orgs/structure/" + id);//если струкутра с таким id уже существует и не является типом OVP
                }
                else
                {
                    if (model.StructureItem.Departments.Single() != null)
                    {
                        model.DepartmentItem = _cmsRepository.getDepartamentItem(model.StructureItem.Departments.Single().Id);
                        model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(id, ViewBag.ActionName);



                        var _peopList = _cmsRepository.getPersonsThisDepartment(model.StructureItem.Departments.Single().Id);

                        if (_peopList != null)
                        {
                            model.PeopleList = new SelectList(_peopList, "Id", "FIO");
                        }


                        model.PeopleLStatus = new SelectList(
                           new List<SelectListItem>
                           {
                            new SelectListItem { Text = "Не выбрано", Value =""},
                            new SelectListItem { Text = "Начальник отделения", Value ="boss"},
                            new SelectListItem { Text = "Старшая медсестра", Value = "sister" },
                           }, "Value", "Text"
                       );

                    }                    
                }
            }
            else
            {
                

            }
            return View("Ovp", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-ovp-btn")]
        public ActionResult SaveOvp(Guid id, OrgsViewModel back_model)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";
            model.StructureItem = _cmsRepository.getStructure(id);
            #region координаты
            double MapX = 0;
            double MapY = 0;

            if (back_model.StructureItem.GeopointX != null) { MapX = (double)back_model.StructureItem.GeopointX; }
            if (back_model.StructureItem.GeopointY != null) { MapY = (double)back_model.StructureItem.GeopointY; }
            ViewBag.Titlecoord = back_model.StructureItem.Title;
            ViewBag.Xcoord = MapX;
            ViewBag.Ycoord = MapY;
            try
            {
                if (back_model.StructureItem.Adress != String.Empty && (MapX == 0 || MapY == 0))
                {
                    var CoordResult = Spots.Coords(back_model.StructureItem.Adress);
                    back_model.StructureItem.GeopointX = CoordResult.GeopointX;
                    back_model.StructureItem.GeopointY = CoordResult.GeopointY;
                }
            }
            catch { }
            #endregion
            
            if (model.StructureItem == null)
            {
                #region создание
                if (ModelState.IsValid)
                {
                    var OrgId = Request.Form["orgid"];
                    if (OrgId != null)
                    {
                        Guid OrgGuid = Guid.Parse(OrgId);
                        if(_cmsRepository.insOvp(id, OrgGuid, back_model.StructureItem)) //, AccountInfo.id, RequestUserInfo.IP
                        {
                            userMessage.info = "Запись создана";
                            userMessage.buttons = new ErrorMassegeBtn[]{
                                 new ErrorMassegeBtn { url = "/admin/orgs/ovp/"+id, text = "ок"}
                             };
                        }
                        else
                        {
                            userMessage.info = "Произошла ошибка";
                            userMessage.buttons = new ErrorMassegeBtn[]
                            {
                                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                            };
                        }                        
                    }
                    else
                    {
                        userMessage.info = "Произошла ошибка";
                        userMessage.buttons = new ErrorMassegeBtn[]
                        {
                                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                        };
                    }

                }
                #endregion
            }
            else
            {
                if (!model.StructureItem.Ovp)
                {
                    return Redirect("/admin/orgs/structure/" + id);//если струкутра с таким id уже существует и не является типом OVP
                }
                #region обновление
                if (ModelState.IsValid) {
                    if (_cmsRepository.setOvp(id, back_model.StructureItem,back_model.DepartmentItem)) //, AccountInfo.id, RequestUserInfo.IP
                    {
                        userMessage.info = "Запись обновлена";
                        userMessage.buttons = new ErrorMassegeBtn[]{
                                 new ErrorMassegeBtn { url = "/admin/orgs/ovp/"+id, text = "ок"}
                             };
                    }
                    else { userMessage.info = "Произошла ошибка"; }
                }
                #endregion
            }
            model.ErrorInfo = userMessage;
            return View("Ovp", model);
        }
        #endregion


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-structure-btn")]
        public ActionResult CancelStructure(Guid id)
        {
            try {
                var data = _cmsRepository.getStructure(id);                
                if (data != null)                
                    return Redirect(StartUrl + "/item/" + data.OrgId + Request.Url.Query);
                else
                    return Redirect(StartUrl + "/item/" + Guid.Parse(Request.Params["orgid"]));
            }
            catch {
                return Redirect(StartUrl);
            }            
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-department-btn")]
        public ActionResult CancelDepartment(Guid id)
        {
            try {
                var data = _cmsRepository.getDepartamentItem(id);                
                if (data != null)                                    
                    return Redirect(StartUrl + "/structure/" + data.StructureF + Request.Url.Query);
                else
                    return Redirect(StartUrl + "/structure/" + Guid.Parse(Request.Params["strucid"]));                                
            }
            catch {
                return Redirect(StartUrl);
            }            
        }
        //[HttpPost]
        //public ActionResult()
        // GET: Admin/Orgs/department/{Guid}
        public ActionResult Department(Guid id)
        {
            ViewBag.Title = "Отделение";            
            model.DepartmentItem=_cmsRepository.getDepartamentItem(id);
            if (model.DepartmentItem != null)
            {
                model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(id, ViewBag.ActionName);

                var _peopList = _cmsRepository.getPersonsThisDepartment(id);

                if (_peopList != null)
                {
                    model.PeopleList = new SelectList(_peopList, "Id", "FIO");
                }
                

                model.PeopleLStatus = new SelectList(
                   new List<SelectListItem>
                   {
                        new SelectListItem { Text = "Не выбрано", Value =""},
                        new SelectListItem { Text = "Начальник отделения", Value ="boss"},
                        new SelectListItem { Text = "Старшая медсестра", Value = "sister" },                        
                   }, "Value", "Text"
               );
            }
            else
            {
                var StrucId = Request.Params["strucid"];
                if (StrucId.Length > 0)
                {
                    ViewBag.StrucId = StrucId;
                    model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(Guid.Parse(StrucId), "structure");
                }                
            }
            return View("department", model);
        }                
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-department-btn")]
        public ActionResult DepartmentSave(Guid id, OrgsViewModel back_model)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";
            model.DepartmentItem = _cmsRepository.getDepartamentItem(id);
            if (ModelState.IsValid)
            {
                if (model.DepartmentItem == null)
                {
                    #region создание
                    var StrucId = Request.Form["strucid"];// Request.Params["strucid"];
                    if (StrucId != null)
                    {
                        Guid StrucGuid = Guid.Parse(StrucId);
                        if (_cmsRepository.insDepartament(id, StrucGuid, back_model.DepartmentItem)) //, AccountInfo.id, RequestUserInfo.IP
                        {
                            userMessage.info = "Запись создана";
                            userMessage.buttons = new ErrorMassegeBtn[]{
                                 new ErrorMassegeBtn { url = "/admin/orgs/department/"+id, text = "ок"}
                             };
                        }
                        else { userMessage.info = "Произошла ошибка"; }
                    }
                    else { userMessage.info = "Произошла ошибка - в адресе строки нет идентифкатора структуры"; }
                    #endregion
                }
                else
                {
                    #region обновление
                    if(_cmsRepository.updDepartament(id, back_model.DepartmentItem)) //, AccountInfo.id, RequestUserInfo.IP
                    {
                        userMessage.info = "Запись обновлена";
                        userMessage.buttons = new ErrorMassegeBtn[]{
                                 new ErrorMassegeBtn { url = "/admin/orgs/department/"+id, text = "ок"}
                             };
                    }
                    else { userMessage.info = "Произошла ошибка"; }
                    #endregion
                }
            }
            model.ErrorInfo = userMessage;
            return View("department", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-department-btn")]
        public ActionResult DepartmentDelete(Guid id)
        {
            ErrorMassege userMassage = new ErrorMassege();
            userMassage.title = "Информация";
            Guid IdParentStruct = _cmsRepository.getDepartamentItem(id).StructureF;
            if (_cmsRepository.delDepartament(id)) //, AccountInfo.id, RequestUserInfo.IP
            {
                userMassage.info = "Запись Удалена";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" },
                    new ErrorMassegeBtn { url = "/admin/orgs/structure/"+IdParentStruct, text = "Вернуться в струкутуру"}
                };
            }
            else
            {
                userMassage.info = "Произошла ошибка";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            model.ErrorInfo = userMassage;
            return View("department", model);
        }
        /// <summary>
        /// Добавление телефонного номера отделению
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-new-phone-depart")]
        public ActionResult AddPhone()
        {
            string IdDepartment = Request["DepartmentItem.Id"];
            string PhoneLabel = Request["new_phone_label"];
            string PhoneValue = Request["new_phone_value"];
            _cmsRepository.insDepartmentsPhone(Guid.Parse(IdDepartment), PhoneLabel, PhoneValue); //, AccountInfo.id, RequestUserInfo.IP
            return Redirect(((System.Web.HttpRequestWrapper)Request).RawUrl);
        }
        public ActionResult DelPhoneDepart(int id)
        {
            _cmsRepository.delDepartmentsPhone(id);
            return null;
        }

        
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-new-people-depart")]
        public ActionResult AddPeople()
        {
            string IdDepartment = Request["DepartmentItem.Id"];
            string IdLinkPeopleForOrg = Request["s_people"];
            string PeopleStatus = Request["s_people_status"];
            string PeoplePost = Request["s_people_post"];
            _cmsRepository.insPersonsThisDepartment(Guid.Parse(IdDepartment), Guid.Parse(IdLinkPeopleForOrg), PeopleStatus, PeoplePost);
            return Redirect(((System.Web.HttpRequestWrapper)Request).RawUrl);
        }

        public ActionResult delPeople(string iddep, string idpeople)
        {
            _cmsRepository.delPersonsThisDepartment(Guid.Parse(iddep), Guid.Parse(idpeople));
            return null;
        }
        
        /// <summary>
        /// Для конкретного объекта получаем список организаций
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrgsListModal(Guid objId, ContentType objType)
        {
            var filtr = new OrgFilter()
            {
                Domain = Domain,
                RelId = objId,
                RelType = objType
            };

            var model = new OrgsModalViewModel()
            {
                ObjctId = objId,
                ObjctType = objType,
                OrgsList = _cmsRepository.getOrgsListWhithChekedFor(filtr),
                OrgsTypes = _cmsRepository.getOrgTypesList(new OrgTypeFilter(){ })
            };

            #region for test
            if (model.OrgsTypes != null)
            {
                foreach (var orgtype in model.OrgsTypes)
                {
                    if (orgtype.Sort == 1000)
                    {
                        if (model.OrgsList != null)
                        {
                            var list1 = model.OrgsList.Where(t => t.Types == null);
                        }
                    }

                    else
                    {
                        if (model.OrgsList != null)
                        {
                            var list2 = model.OrgsList.Where(t => t.Types != null && t.Types.Contains(orgtype.Id));
                        }
                    }
                }
            }
            #endregion

            return PartialView("Modal/Orgs", model);
        }

        [HttpPost]
        public ActionResult UpdateLinkToOrg (ContentLinkModel data)
        {
            if(data != null)
            {
                var res = _cmsRepository.updateContentLink(data);
                if (res)
                    return Json("Success");
            }
            
            //return Response.Status = "OK";
            return Json("An Error Has occourred"); //Ne
        }


        #region administrative
        public ActionResult Administrativ(Guid Id)
        {
            ViewBag.Title = "Административный персонал";
            var OrgId = Request.Params["orgid"];
            //информация о персоне
            model.AdministrativItem = _cmsRepository.getAdministrativ(Id);

            #region сотрудники для выпадающего списка
            var _peopList = _cmsRepository.getPersonsThisOrg(model.AdministrativItem.OrgId);
            if (_peopList != null)
            {
                model.PeopleList = new SelectList(_peopList, "Id", "FIO");
            }
            #endregion
            if (OrgId == null)
            {
                model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(Id, ViewBag.ActionName);
            }            
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-adminiatrativ-btn")]
        public ActionResult Administrativ(Guid Id, OrgsViewModel back_model, HttpPostedFileBase upload)
        {
            ErrorMassege userMassege = new ErrorMassege
            {
                title = "Информация"
            };

            ViewBag.Title = "Административный персонал";

            if (ModelState.IsValid)
            {
                string aliasname = Transliteration.Translit(back_model.AdministrativItem.Surname + "-" + back_model.AdministrativItem.Name);

                #region Изображение
                string savePath = Settings.UserFiles+Domain+"/administrativ/" + Id + "/";

                int width = 225; // ширина 
                int height = 225; // высота

                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                    if (!validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        model.ErrorInfo = new ErrorMassege()
                        {
                            title = "Ошибка",
                            info = "Вы не можете загружать файлы данного формата",
                            buttons = new ErrorMassegeBtn[]
                            {
                                new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                            }
                        };
                        return View("Item", model);
                    }                    
                    Photo photo = new Photo
                    {
                        Name = aliasname + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, aliasname, width, height)
                    };

                    back_model.AdministrativItem.Photo = photo;
                }                
                #endregion

                model.AdministrativItem = _cmsRepository.getAdministrativ(Id);
                if (model.AdministrativItem == null)
                {
                    var OrgId = Request.Params["orgid"];
                    back_model.AdministrativItem.OrgId = Guid.Parse(OrgId);
                    _cmsRepository.insAdministrativ(Id, back_model.AdministrativItem);
                    userMassege.info = "Запись создана";
                    userMassege.buttons = new ErrorMassegeBtn[]{
                        new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                        new ErrorMassegeBtn { url = "/admin/orgs/administrativ/"+Id, text = "ок", action = "false" }
                    };
                }
                else
                {
                    _cmsRepository.updAdministrativ(Id, back_model.AdministrativItem);                    
                    userMassege.info = "Запись сохранена";
                    userMassege.buttons = new ErrorMassegeBtn[]{
                        new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                        new ErrorMassegeBtn { url = "/admin/orgs/administrativ/"+Id, text = "ок", action = "false" }
                    };
                }
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";
                userMassege.buttons = new ErrorMassegeBtn[]
                {
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            #region сотрудники для выпадающего списка
            var _peopList = _cmsRepository.getPersonsThisOrg(model.AdministrativItem.OrgId);
            if (_peopList != null)
            {
                model.PeopleList = new SelectList(_peopList, "Id", "FIO");
            }
            #endregion

            model.BreadCrumbOrg = _cmsRepository.getBreadCrumbOrgs(Id, ViewBag.ActionName);
            return View(model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-adminiatrativ-btn")]
        public ActionResult CancelAdministrative(Guid id)
        {
            try
            {
                var data = _cmsRepository.getAdministrativ(id);
                if (data != null)
                    return Redirect(StartUrl + "/item/" + data.OrgId + Request.Url.Query);
                else
                    return Redirect(StartUrl + "/item/" + Guid.Parse(Request.Params["orgid"]));
            }
            catch
            {
                return Redirect(StartUrl);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-adminiatrativ-btn")]
        public ActionResult DeleteAdministrative(Guid id)
        {
            ErrorMassege userMassage = new ErrorMassege();
            userMassage.title = "Информация";
            Guid ParentOrgId= _cmsRepository.getAdministrativ(id).OrgId;
            if (_cmsRepository.delAdministrativ(id)) //, AccountInfo.id, RequestUserInfo.IP
            {
                userMassage.info = "Запись Удалена";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" },
                    new ErrorMassegeBtn { url = "/admin/item/"+ParentOrgId, text = "Вернуться в список"}
                };
            }
            else
            {
                userMassage.info = "Произошла ошибка";
                userMassage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            model.ErrorInfo = userMassage;
            return View("item", model);

        }

        #endregion
    }
}