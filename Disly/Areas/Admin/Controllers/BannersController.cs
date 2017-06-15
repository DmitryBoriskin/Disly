using Disly.Areas.Admin.Models;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class BannersController : CoreController
    {
        string savePath = "/Userfiles/Banners/";

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

        // GET: Banners
        public ActionResult Index()
        {
            string TypeBanners = Request.Params["type"];
            string SectionBanners = Request.Params["section"];
            BannersViewModel model = new BannersViewModel() {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                List = _repository.getcmsBanners(SectionBanners,TypeBanners,Domain)
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult Create(Guid id)
        {
            BannersViewModel model = new BannersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                Item = _repository.getBanner(id,Domain)
            };
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
            return View("Item", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn")]
        public ActionResult Create(Guid id, BannersViewModel model, HttpPostedFileBase upload)
        {
            ViewBag.BtnName = "Создать";

            if (ModelState.IsValid)
            {                
                model.Item.SiteId = Domain;

                if (upload != null) {
                    model.Item.Photo = savePath + upload.FileName;
                    model.Item.Photo = Files.SaveImageResize(upload, ConfigurationManager.AppSettings["ImgBtn"], 300, 200);                    
                }
                _repository.insertBanners(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);

                ViewBag.Message = "Запись добавлена";
                ViewBag.backurl = id;
            }
            model = new BannersViewModel() 
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            return View("Item", model);
        }



        public ActionResult Edit(Guid id)
        {
            BannersViewModel model = new BannersViewModel() 
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                Item=_repository.getBanner(id,Domain)
            };
            if (model.Item == null) Response.Redirect("/Admin/Banners");         
            ViewBag.Date = Convert.ToDateTime(model.Item.Date).ToString("yyyy-MM-dd");
            ViewBag.Photo = model.Item.Photo;
            ViewBag.PhotoName = Path.GetFileName(model.Item.Photo);
            try
            {
                ViewBag.PhotoSize = Files.FileAnliz.Size(model.Item.Photo);
            }
            catch {
                ViewBag.PhotoSize = "";
            }            
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn")]
        public ActionResult Edit(Guid id, BannersViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                BannersViewModel modelImg = new BannersViewModel()
                {
                    Item = _repository.getBanner(id, Domain)
                };

                if (upload != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(modelImg.Item.Photo)))
                    {
                        System.IO.File.Delete(Server.MapPath(modelImg.Item.Photo));                        
                    }
                    #region обновление изображения
                    if (upload != null && upload.ContentLength > 0)
                        try
                        {
                            model.Item.Photo = Files.SaveImageResize(upload, ConfigurationManager.AppSettings["ImgBtn"], 300, 200);

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
                else {
                    //добавление нового изображения не происходит                    
                    model.Item.Photo = modelImg.Item.Photo;
                }

                _repository.updBanners(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);
                ViewBag.Message = "Запись сохранена";
                ViewBag.backurl = id;
            }
            model = new BannersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                Item=_repository.getBanner(id,Domain)
            };
            return View("Item", model);

        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                    return enc;
            return null;
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ViewBag.Message = "Запись удалена";
            ViewBag.backurl = id;
            BannersViewModel model = new BannersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            _repository.delBanners(id,Domain);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);
            return View("Item", model);
        }





        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            return RedirectToAction("", "banners");
        }

    }
}