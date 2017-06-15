using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disly.Areas.Admin.Models;
using System.Web.Mvc;
using cms.dbModel.entity;
using System.Net;

namespace Disly.Areas.Admin.Controllers
{
    public class UsersController : CoreController
    {
        public UserSex[] SexType;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"].ToString().ToLower();
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            #region Заполнение выподающих списков
            SexType = new UserSex[3];
            SexType[0] = new UserSex { Label = "Пол не выбран", Value = null };
            SexType[1] = new UserSex { Label = "Мужской", Value = true };
            SexType[2] = new UserSex { Label = "Женский", Value = false };
            #endregion

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Вывод страницы пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            string UrlGroup = Request.Params["group"];
            
            UsersViewModel model = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,


                getUsersList = _repository.getUserList(UrlGroup, Domain),
                Group = _repository.getUserGroup()
            };
            
            //ограничиваем видимость не-разработчикам и не-админам портала
            string Alias = model.Account.Group;
            if (Alias.ToLower() != "developer" && Alias.ToLower() != "administrator") //приходит null
            {
                model.getUsersList = model.getUsersList.Where(m => (m.F_Group.ToLower() != "developer") && m.F_Group.ToLower() != "administrator").ToArray();
            }
            return View(model);
        }

        /// <summary>
        /// Вывод страницы пользователей
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string searchSurname, string searchName, string searchEmail)
        {
            string UrlGroup = Request.Params["group"];
            UsersViewModel model = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                getUsersList = _repository.getUsersList(UrlGroup, searchSurname, searchName, searchEmail,Domain),
                Group = _repository.getUserGroup()
            };

            //ограничиваем видимость не-разработчикам и не-админам портала
            string Alias = model.Account.Group;
            if (Alias.ToLower() != "developer" && Alias.ToLower() != "administrator")
            {
                model.getUsersList = model.getUsersList.Where(m => (m.F_Group.ToLower() != "developer") || m.F_Group.ToLower() != "administrator").ToArray();
            }

            return View(model);
        }

        public ActionResult Create(Guid id)
        {
            UsersViewModel model = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                User = _repository.getUser(id,Domain),
                Sex = SexType,
                Group = _repository.getUserGroup()
            };
            model.Group = model.Group.Where(w => (w.C_Alias.ToLower() != "developer") && (w.C_Alias.ToLower() != "administrator")).ToArray();

            return View("Item", model);         
        }
        
        public ActionResult Edit(Guid id)
        {
            UsersViewModel model = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                User = _repository.getUser(id,Domain),
                Sex = SexType,
                Group = _repository.getUserGroup()
            };
            model.Group = model.Group.Where(w => (w.C_Alias.ToLower() != "developer") && (w.C_Alias.ToLower() != "administrator")).ToArray();

            return View("Item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn")]
        public ActionResult Create(Guid id, UsersViewModel model)
        {
            try
            {
                if (ModelState.IsValid) {
                    char[] _pass = model.pass.ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    model.User.Id = id;
                    model.User.C_Salt = NewSalt;
                    model.User.C_Hash = NewHash;

                    UserSiteLink usLink = new UserSiteLink()
                    {
                        SiteId = Domain,
                        UserId = id
                    };

                    //проверка на наличие пользователя (по E-mail)                    
                    if (_repository.isEmailFree(model.User.C_EMail))
                    {
                        _repository.createUser(id, model.User);
                        _repository.createUserSiteLink(usLink); //привязка нового юзера к данному сайту
                        _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);

                        ViewBag.Message = "Запись добавлена";
                        ViewBag.backurl = id;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Данный E-mail уже используется.");
                        //TODO: если пользователь существует, нужно предложить связать существующего пользователя с новым сайтом
                        //UsersModel userExist = _repository.getUser(model.User.C_EMail);
                        //ModelState.AddModelError("", "Данный E-mail уже используется. Связать сайт с этим пользователем?");
                    }
                }

                model = new UsersViewModel()
                {
                    Account = AccountInfo,
                    Settings = SettingsInfo,
                    UserResolution = UserResolutionInfo,

                    User = _repository.getUser(id,Domain),
                    Sex = SexType,
                    Group = _repository.getUserGroup()
                };
                model.Group = model.Group.Where(w => (w.C_Alias.ToLower() != "developer") && (w.C_Alias.ToLower() != "administrator")).ToArray();

                return View("Item", model);
            }
            catch (Exception ex)
            {
                ViewBag.Exeption = ex.ToString();
                return RedirectToAction("Error"); // на страницу ошибок
            }
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn")]
        public ActionResult Edit(Guid id, UsersViewModel model)
        {
            try
            {
                if (ModelState.IsValid) {
                    //проверка на наличие пользователя (по E-mail)                    
                    if (_repository.isEmailFree(model.User.C_EMail))
                    {
                        _repository.updateUser(id, model.User);
                        _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);

                        ViewBag.Message = "Запись добавлена";
                        ViewBag.backurl = id;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Данный E-mail уже используется.");
                    }
                }

                model = new UsersViewModel()
                {
                    Account = AccountInfo,
                    Settings = SettingsInfo,
                    UserResolution = UserResolutionInfo,

                    User = _repository.getUser(id,Domain),
                    Sex = SexType,
                    Group = _repository.getUserGroup()
                };
                model.Group = model.Group.Where(w => (w.C_Alias.ToLower() != "developer") && (w.C_Alias.ToLower() != "administrator")).ToArray();

                return View("Item", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
                return RedirectToAction("Error"); // на страницу ошибок
            }
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return RedirectToAction("", (String)RouteData.Values["controller"]);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ViewBag.Message = "Запись удалена";
            ViewBag.backurl = id;
            _repository.deleteUser(id, Domain);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);

            return RedirectToAction("Index");
        }

        //public ActionResult UserResolut(Guid id)
        //{
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        User = _repository.getUser(id,Domain),
        //        ResolutionsList = _repository.getResolutionsPerson(id)
        //    };
        //    return PartialView("Resolut", model);
        //}


        //[HttpGet]
        //public ActionResult GroupEdit() {
        //    string Alias = Request.Params["alias"];
        //    UsersViewModel model = new UsersViewModel()
        //    {                
        //        GroupItem = _repository.getUserGroup(Alias),
        //        ResolutionsTemplatesList=_repository.getResolutions(Alias)
        //    };
        //    return PartialView("Group", model);
        //}
        
        //public ActionResult GroupCreate()
        //{
        //    UsersViewModel model = new UsersViewModel();
        //    return PartialView("Group", model);
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn-group-user")]        
        //public ActionResult GroupCreate(UsersViewModel model)
        //{
        //    Guid PageId = Guid.NewGuid();          
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _repository.createUsersGroup(PageId, model.GroupItem.C_Alias, model.GroupItem.C_GroupName);
        //            _repository.insertLog(PageId, AccountInfo.id, "insert", RequestUserInfo.IP);
        //            ViewBag.SuccesAlert = "Запись добавлена";
        //            ViewBag.ActionName = "groupedit";
        //        }
        //        catch
        //        {
        //            ViewBag.DankerAlert = "Группа с таким названием уже существует";
        //            UsersViewModel newmodel = new UsersViewModel();
        //            return PartialView("Group", newmodel);
        //        }
                
        //    }
        //    model = new UsersViewModel()
        //    {
        //        GroupItem = _repository.getUserGroup(model.GroupItem.C_Alias),
        //        ResolutionsTemplatesList = _repository.getResolutions(model.GroupItem.C_Alias)
        //    };
        //    return PartialView("Group", model);            
        //}


        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn-group-user")]
        //public ActionResult GroupEdit(UsersViewModel model)
        //{
        //    Guid PageId;            
        //    UsersViewModel model_dop = new UsersViewModel()
        //    {
        //        GroupItem = _repository.getUserGroup(model.GroupItem.C_Alias),
        //        ResolutionsTemplatesList = _repository.getResolutions(model.GroupItem.C_Alias)
        //    };
        //    PageId = model_dop.GroupItem.id;
        //    if (ModelState.IsValid)
        //    {
        //        _repository.updateUsersGroup(model.GroupItem.C_Alias, model.GroupItem.C_GroupName);
        //        _repository.insertLog(PageId, AccountInfo.id, "update", RequestUserInfo.IP);
        //        ViewBag.SuccesAlert = "Запись обновлена";                
        //    }
        //    return PartialView("Group", model_dop);
        //}


        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn-group-user")]
        //public ActionResult GroupDelete()
        //{
        //    string Alias = Request.Params["alias"];
        //    Guid PageId;
        //    UsersViewModel model_dop = new UsersViewModel()
        //    {
        //        GroupItem = _repository.getUserGroup(Alias)
        //    };
        //    _repository.deleteUsersGroup(Alias);//удаление группы
        //    _repository.delResolutionsTemplates(Alias);//удаление прав            

        //    PageId = model_dop.GroupItem.id;
        //    _repository.insertLog(PageId, AccountInfo.id, "delete", RequestUserInfo.IP);
        //    return RedirectToAction("Index");

        //}

        //[HttpPost]
        //public ActionResult UserResolutAppoint(Guid id, Guid url, string action, int val) {            
        //    _repository.appointResolutionsUser(id, url, action, val);
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        User = _repository.getUser(id,Domain),
        //        ResolutionsList = _repository.getResolutionsPerson(id)
        //    };
        //    return PartialView("Resolut", model);
        //}
                
        //[HttpPost]        
        //public ActionResult GroupResolut(string user, Guid url, string action, int val) {
        //    string Alias = Request.Params["alias"];
        //    _repository.appointResolutionsTemplates(user, url, action, val);
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        ResolutionsTemplatesList = _repository.getResolutions(Alias)
        //    };
        //    return PartialView("Group", model);
        //}

        public ActionResult ChangePass(Guid id)
        {
            UsersViewModel model = new UsersViewModel()
            {                
                UserResolution = UserResolutionInfo,
                User = _repository.getUser(id,Domain)
            };
            return PartialView("ChangePass", model);
        }

        [HttpPost]        
        [MultiButton(MatchFormKey = "action", MatchFormValue = "password-update")]
        public ActionResult ChangePass(Guid id, UsersViewModel model)
        {

            model = new UsersViewModel()
            {
                User = _repository.getUser(id,Domain),
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ChangePass=model.ChangePass
            };
            string NewPass = model.ChangePass.Password;
            if (ModelState.IsValid)
            {
                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;
                _repository.changePasswordUser(id, NewSalt, NewHash);
                _repository.insertLog(id, AccountInfo.id, "change_pass", RequestUserInfo.IP);
                ViewBag.SuccesAlert = "Пароль изменен";
                #region оповещение на e-mail
                string ErrorText = "";
                string Massege = String.Empty;
                Mailer Letter = new Mailer();
                Letter.Theme = "Изменение пароля";
                Massege = "<p>Уважаемый "+ model.User.C_Surname +" "+ model.User.C_Name+"</p>";
                Massege += "<p>Ваш пароль на сайте <b>"+model.Settings.Title+"</b> был изменен</p>";
                Massege += "<p>Ваш новый пароль:<b>" + NewPass + "</b></p>";
                Massege += "<p>С уважением, администрация портала!</p>";
                Massege += "<hr><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</span>";
                Letter.MailTo = model.User.C_EMail;
                Letter.Text = Massege;
                ErrorText = Letter.SendMail();
                #endregion
            }
           
            return PartialView("ChangePass", model);
        }
    }
}

