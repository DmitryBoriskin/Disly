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
    public class MaterialsController : CoreController
    {        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

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

        // GET: Materials
        public ActionResult Index(string category, string type)
        {
            MaterialsViewModel model = new MaterialsViewModel()
            {
                List = _repository.getSearchCmsMaterial(Domain, null, null, null, category, type),
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            return View(model);
        }
        /// <summary>
        /// Поиск в списке
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string SearchLine, DateTime? Begin, DateTime? End, MaterialsViewModel searchList, string category, string type)
        {
            searchList = new MaterialsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                List = _repository.getSearchCmsMaterial(Domain, SearchLine, Begin, End, category, type)
            };

            return View(searchList);
        }

        [HttpGet]
        public ActionResult Create(Guid id)
        {
            MaterialsViewModel model = new MaterialsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd"); // Convert.ToDateTime(Model.Item.Date).ToString("yyyy-MM-dd")
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn")]
        public ActionResult Create(Guid id,MaterialsViewModel model, HttpPostedFileBase upload)
        {
            ViewBag.BtnName = "Создать";

            model.Item.Site = Domain;
            model.Item.Year = model.Item.Date.ToString("yyyy");
            model.Item.Month = model.Item.Date.ToString("MM");
            model.Item.Day = model.Item.Date.ToString("dd");

            //int org = RequestUrl.HostName().ToString().IndexOf(".", 0);
            //model.Item.Org = RequestUrl.HostName().Substring(0, org);            

            if (ModelState.IsValid)
            {
                #region Изображение
                if (upload != null)
                {                    
                    #region добавление изображения
                    string Path = ConfigurationManager.AppSettings["Root"] + Domain + ConfigurationManager.AppSettings["News"] + model.Item.Year + "/" + model.Item.Month + "/";
                    if (upload != null && upload.ContentLength > 0)
                        try
                        {
                            model.Item.Photo = Files.SaveImageResize(upload, Path, 253, 168);
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
                _repository.insCmsMaterials(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);

                ViewBag.Message = "Запись добавлена";
                ViewBag.backurl = id;
            }

            model = new MaterialsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };

            return View("Item", model);
        }               

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialsViewModel model = new MaterialsViewModel()
            {
                Item = _repository.getCmsMaterial(Domain, id),                
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            if (model.Item != null)
            {
                ViewBag.Date = Convert.ToDateTime(model.Item.Date).ToString("yyyy-MM-dd");
                ViewBag.Photo = model.Item.Photo;
                ViewBag.PhotoName = Path.GetFileName(model.Item.Photo);
                try
                {
                    ViewBag.PhotoSize = Files.FileAnliz.Size(model.Item.Photo);
                }
                catch
                {
                    ViewBag.PhotoSize = "";
                }
            }
            else { Response.Redirect("/Error/"); }


            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn")]
        public ActionResult Edit(Guid id, MaterialsViewModel model, HttpPostedFileBase upload)
        {
            ViewBag.BtnName = "Сохранить";

            model.Item.Site = Domain;
            if (ModelState.IsValid)
            {

                #region Изображение
                MaterialsViewModel ActualModel = new MaterialsViewModel()
                {
                    Item = _repository.getCmsMaterial(Domain,id)
                };

                if (upload != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(ActualModel.Item.Photo)))
                    {
                        System.IO.File.Delete(Server.MapPath(ActualModel.Item.Photo));
                    }
                    #region обновление изображения
                    string Path = ConfigurationManager.AppSettings["Root"] + Domain + ConfigurationManager.AppSettings["News"]+ ActualModel.Item.Year+"/" + ActualModel.Item.Month+"/";
                    if (upload != null && upload.ContentLength > 0)
                        try
                        {
                            model.Item.Photo = Files.SaveImageResize(upload, Path, 253, 168);                            
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
                    model.Item.Photo = ActualModel.Item.Photo;
                }
                #endregion


                _repository.setCmsMaterials(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);
                ViewBag.Message = "Запись сохранена";
                ViewBag.backurl = id;  
                
                
                                  
            }            
            model = new MaterialsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                Item = _repository.getCmsMaterial(Domain, id)
            };
            return View("Item", model);
        }
           
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ViewBag.Message = "Запись удалена";
            ViewBag.backurl = id;
            MaterialsViewModel model = new MaterialsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            _repository.delCmsMaterials(Domain, id);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);
            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            return RedirectToAction("", "materials");
        }
    }
}