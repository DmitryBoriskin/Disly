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
    public class AllUsersController : CoreController
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
            string Alias = Request.Params["group"];
            UsersViewModel model = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                

                getUsersList = _repository.getAllUserList(Alias),
                Group = _repository.getUserGroup()
            };
            return View(model);
        }

        /// <summary>
        /// Вывод страницы пользователей
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string searchSurname, string searchName, string searchEmail, UsersViewModel searchList)
        {
            string GroupName = Request.Params["group"];
            searchList = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                getUsersList = _repository.getUsersList(GroupName, searchSurname, searchName, searchEmail,Domain),
                Group = _repository.getUserGroup()

            };

            return View(searchList);
        }

        public ActionResult Create(Guid id)
        {
            UsersViewModel model = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                User = _repository.getUser(id),
                Sex = SexType,
                Group = _repository.getUserGroup()
            };

            return View("Item", model);         
        }
        
        public ActionResult Edit(Guid id)
        {
            UsersViewModel model = new UsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                User = _repository.getUser(id),
                Sex = SexType,
                Group = _repository.getUserGroup()
            };

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

                    _repository.createUser(id, model.User);
                    _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);

                    ViewBag.Message = "Запись добавлена";
                    ViewBag.backurl = id;
                }

                model = new UsersViewModel()
                {
                    Account = AccountInfo,
                    Settings = SettingsInfo,
                    UserResolution = UserResolutionInfo,

                    User = _repository.getUser(id),
                    Sex = SexType,
                    Group = _repository.getUserGroup()
                };

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
                    _repository.updateUser(id, model.User);
                    _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);

                    ViewBag.Message = "Запись обновлена";
                    ViewBag.backurl = id;
                }

                model = new UsersViewModel()
                {
                    Account = AccountInfo,
                    Settings = SettingsInfo,
                    UserResolution = UserResolutionInfo,

                    User = _repository.getUser(id),
                    Sex = SexType,
                    Group = _repository.getUserGroup()
                };

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
            _repository.deleteUser(id);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SiteLinks(Guid id)
        {
            var List = _repository.getUserSiteLinks(id);
            var WholeList = _repository.getSiteList(null);
            //var WholeList = (_repository.getSiteList(null)).Except(List).ToArray();
            SitesModel[] AllList = new SitesModel[WholeList.Length];
            int i = 0;
            //TODO!!!!!!!!!!!!

            foreach (SitesModel site in WholeList)
            {
                if (List !=null && !List.Contains(site))
                {
                    AllList[i] = site;
                    i++;
                }

            }

            //for (int j = 0; i < List.Length; i++)
            //{
            //    foreach (SitesModel site in WholeList)
            //    {
            //        if (site.Id != List[i].Id)
            //        {
            //            AllList[j] = site;
            //            j++;
            //        }
            //    }
            //}

            AllList = AllList.Where(w => w != null).ToArray();

            SitesViewModel model = new SitesViewModel()
            {
                SiteList = List,
                AllSiteList = WholeList,
                Site = null,
                User = _repository.getUser(id)
            };

            return PartialView("SiteLinks", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn-group")]
        public ActionResult SiteLinks(SitesViewModel input)
        {
                UserSiteLink model = new UserSiteLink()
                {
                    SiteId = input.Site.C_Domain,
                    UserId = input.User.Id
                };
                _repository.createUserSiteLink(model);
            _repository.insertLog(input.User.Id, AccountInfo.id, "insert sitelink", RequestUserInfo.IP);

            ViewBag.SuccesAlert = "Связь успешно добавлена.";

            return RedirectToAction("SiteLinks/" + input.User.Id);
        }

        [HttpPost]
        public string DeleteSiteLinks(string userId, string siteId)
        {
            Guid UserId = Guid.Parse(userId);
            try
            {
                _repository.deleteUserSiteLink(UserId, siteId);
                _repository.insertLog(UserId, AccountInfo.id, "delete sitelink", RequestUserInfo.IP);

                return "";
            }
            catch { return "Не удалось удалить связь с сайтом."; }
        }
    }
}

