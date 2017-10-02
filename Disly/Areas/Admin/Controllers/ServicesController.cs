using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class ServicesController : CoreController
    {
        private string domain;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            string _domain = HttpContext.Request.Url.Host.ToString().ToLower().Replace("www.", "").Replace("new.", "");
            try { domain = _cmsRepository.getSiteId(_domain); }
            catch
            {
                if (_domain != ConfigurationManager.AppSettings["BaseURL"]) filterContext.Result = Redirect("/Error/");
                else domain = String.Empty;
            }
        }

        /// <summary>
        /// Проссмотр журнала
        /// </summary>
        /// <param name="id">ID записи, для которой получаем журнал</param>
        /// <returns></returns>
        public ActionResult Log(Guid id)
        {
            cmsLogModel[] model = _cmsRepository.getCmsPageLog(id);

            return PartialView("Log", model);
        }

        #region Смена пароля
        /// <summary>
        /// Смена пароля
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChangePass(Guid id)
        {
            PortalUsersViewModel model = new PortalUsersViewModel()
            {
                UserResolution = UserResolutionInfo,
                Item = _cmsRepository.getUser(id)
            };
            return PartialView("ChangePass", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "password-update")]
        public ActionResult ChangePass(Guid id, PortalUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                string NewPass = model.Password.Password;

                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;
                _cmsRepository.changePassword(id, NewSalt, NewHash, AccountInfo.id, RequestUserInfo.IP);
                ViewBag.SuccesAlert = "Пароль изменен";
                #region оповещение на e-mail
                //string ErrorText = "";
                //string Massege = String.Empty;
                //Mailer Letter = new Mailer();
                //Letter.Theme = "Изменение пароля";


                //bool sex = true;
                //if (model.User.B_Sex.HasValue) { if (!(bool)model.User.B_Sex) sex = false; }
                //Massege = (sex) ? "<p>Уважаемый " : "<p>Уважаемая ";
                //Massege += model.User.C_Surname + " " + model.User.C_Name + "</p>";
                //Massege += "<p>Ваш пароль на сайте <b>" + model.Settings.Title + "</b> был изменен</p>";
                //Massege += "<p>Ваш новый пароль:<b>" + NewPass + "</b></p>";
                //Massege += "<p>С уважением, администрация портала!</p>";
                //Massege += "<hr><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</span>";
                //Letter.MailTo = model.User.C_EMail;
                //Letter.Text = Massege;
                //ErrorText = Letter.SendMail();
                #endregion
            }

            model = new PortalUsersViewModel()
            {
                UserResolution = UserResolutionInfo,
                Item = _cmsRepository.getUser(id)
                
            };

            return PartialView("ChangePass", model);
        }
        #endregion

        #region Настройки доступа пользователя
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult UserResolutions(Guid id)
        //{
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        User = _repository.getUser(id),
        //        ResolutionsList = _repository.getResolutionsPerson(id)
        //    };
        //    return PartialView("UserResolutions", model);
        //}

        //[HttpPost]
        //public ActionResult UserResolutions(Guid id, Guid url, string action, int val)
        //{   
        //    bool Result = false;

        //    _repository.appointResolutionsUser(id, url, action, val);

        //    //UsersViewModel model = new UsersViewModel()
        //    //{
        //    //    User = _repository.getUser(id),
        //    //    ResolutionsList = _repository.getResolutionsPerson(id)
        //    //};
        //    //return PartialView("UserResolutions", model);
        //    return Content(Result.ToString());
        //}
        #endregion

        #region Группы пользователей
        public ActionResult UsersGroup(string id)
        {
            UsersGroupModel model = _cmsRepository.getUsersGroup(id);

            return PartialView("UsersGroup", model);
        }

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
        //public ActionResult GroupResolut(string user, Guid url, string action, int val)
        //{
        //    string Alias = Request.Params["alias"];
        //    _repository.appointResolutionsTemplates(user, url, action, val);
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        ResolutionsTemplatesList = _repository.getResolutions(Alias)
        //    };
        //    return PartialView("Group", model);
        //}
        #endregion

        #region Карта сайта
        [HttpGet]
        public ActionResult MenuGroup(string id)
        {
            SiteMapMenu model = new SiteMapMenu();

            return PartialView("SiteMapMenuGroup", model);
        }
        #endregion

        /// <summary>
        /// Изменение позиции в списке
        /// </summary>
        /// <param name="group">Раздел, в котором производятся изменения</param>
        /// <param name="id">ID записи, у которой меняется позиция</param>
        /// <param name="permit">Номер новой позиции</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePermit(string group, string menuSort, Guid id, int permit)
        {
            bool Result = false;

            switch (group.ToLower())
            {
                case "cmsmenu":
                    Result = _cmsRepository.permit_cmsMenu(id, permit, AccountInfo.id, RequestUserInfo.IP);
                    break;
                case "sitemap":
                    Result = _cmsRepository.permit_SiteMap(id, permit, domain, menuSort);
                    break;
            }

            return Content(Result.ToString());
        }
        
        [HttpPost]
        public ActionResult GetFile()
        {
            Response.ContentType = "application/json";

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                //file.SaveAs(Server.MapPath("/Content/temp/" + file.FileName));

                using (Image image = Image.FromStream(file.InputStream))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                        return Content("{ \"location\": \"" + base64String + "\" }");
                    }
                }

                //return Content("{ \"location\": \"/Content/temp/" + file.FileName + "\" }");
            }

            return Content("");
        }
    }
}