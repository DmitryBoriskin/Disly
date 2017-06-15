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
    public class SiteMapController : CoreController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            #region Filter
            FilterViewModel filter = new FilterViewModel()
            {
                Sections = _repository.getSectionsGroup(ViewBag.ControllerName)
            };
            ViewBag.Filter = filter; 
            #endregion

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: SiteMap
        public ActionResult Index()
        {
            SiteMapViewModel model = new SiteMapViewModel()
            {
                List = _repository.getCmsSiteMap(Domain, "/"),              
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };

            return View(model);
        }
        
        public ActionResult OpenFolder(string path)
        {
            SiteMapViewModel model = new SiteMapViewModel()
            {
                List = _repository.getCmsSiteMap(Domain, path)
            };
            return PartialView(model);
        }
        
        [HttpGet]        
        public ActionResult Create(Guid id)
        {
            ViewBag.GetPath = Request.Params["path"];
            SiteMapViewModel model = new SiteMapViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                MapView = _repository.getCmsPageViewsSelec(),
                MapType = _repository.getSiteMapType()
                //MapView= _repository.getCmsPageViewsSelec()
                //Item = _repository.getCmsSiteMap(id)
            };
            return View("Item", model);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn")]
        public ActionResult Create(Guid id, SiteMapViewModel model, HttpPostedFileBase upload)
        {
            string path = string.Empty;
            try
            {
                path = Request.Params["path"].Replace("//", "/");
            }
            catch { }
            model.Item.Site = Domain;

            if (ModelState.IsValid)
            {
                #region Изображение
                if (upload != null)
                {
                    #region добавление изображения
                    string PathFile = ConfigurationManager.AppSettings["Root"] + Domain + ConfigurationManager.AppSettings["SiteMap"] + model.Item.Alias + "/";
                    if (upload != null && upload.ContentLength > 0)
                        try
                        {
                            model.Item.Logo = Files.SaveImageResize(upload, PathFile, 350, 233);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                        }
                    else
                    {
                        ViewBag.Message = "Вы не выбрали файл.";
                    }
                    #endregion
                }
                #endregion


                if (model.Item.Alias == null) { model.Item.Alias = Transliteration.Translit(model.Item.Title.ToString()); }
                _repository.insCmsSiteMap(id, path, model.Item);
                _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);
                ViewBag.Message = "Запись добавлена";
                ViewBag.backurl = id;
            }
            else { }
            model = new SiteMapViewModel()
            {
                MapType = _repository.getSiteMapType(),
                MapView = _repository.getCmsPageViewsSelec(),
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            return View("Item", model);                  
        }

        // GET: SiteMap/edit/id
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            SiteMapViewModel model = new SiteMapViewModel()
            {
                Item = _repository.getCmsSiteMap(Domain, id),
                MapType = _repository.getSiteMapType(),
                MapView = _repository.getCmsPageViewsSelec(),
                Account = AccountInfo,
                Settings = SettingsInfo,       
                UserResolution = UserResolutionInfo
            };            
            if (model.Item != null){
                ViewBag.Photo = model.Item.Logo;
                ViewBag.PhotoName = Path.GetFileName(model.Item.Logo);
                try
                {
                    ViewBag.PhotoSize = Files.FileAnliz.Size(model.Item.Logo);
                }
                catch
                {
                    ViewBag.PhotoSize = "";
                }
            }
            else{ Response.Redirect("/Error/"); }

            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn")]
        public ActionResult Edit(Guid id, SiteMapViewModel model, HttpPostedFileBase upload)
        {
            ViewBag.BtnName = "Сохранить";
            model.Item.Site = Domain;

            if (ModelState.IsValid)
            {
                #region Изображение
                SiteMapViewModel ActualModel = new SiteMapViewModel()
                {
                    Item = _repository.getCmsSiteMap(Domain, id)
                };

                if (upload != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(ActualModel.Item.Logo)))
                    {
                        System.IO.File.Delete(Server.MapPath(ActualModel.Item.Logo));
                    }
                    #region обновление изображения
                    string Path = ConfigurationManager.AppSettings["Root"] + Domain + ConfigurationManager.AppSettings["SiteMap"] + ActualModel.Item.Alias + "/";
                    if (upload != null && upload.ContentLength > 0)
                        try
                        {
                            model.Item.Logo = Files.SaveImageResize(upload, Path, 350, 233);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                        }
                    else
                    {
                        ViewBag.Message = "Вы не выбрали файл.";
                    }
                    #endregion
                }
                else
                {
                    //добавление нового изображения не происходит                    
                    model.Item.Logo = ActualModel.Item.Logo;
                }
                #endregion
                
                _repository.setCmsSiteMap(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);
                ViewBag.Message = "Запись сохранена";
                ViewBag.backurl = id;
            }
            model = new SiteMapViewModel()
            {
                MapType = _repository.getSiteMapType(),
                MapView = _repository.getCmsPageViewsSelec(),
                Item = _repository.getCmsSiteMap(Domain, id),
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo

            };
            return View("Item", model);

        }
                
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _repository.delCmsSiteMap(Domain, id);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);
            return RedirectToAction("", (String)RouteData.Values["controller"]);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            return RedirectToAction("", "sitemap");
        }

    }
}