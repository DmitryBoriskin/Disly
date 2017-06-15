using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class PlaceCardController : CoreController
    {        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }
                
        public ActionResult Index()
        {
            PlaceCardViewModel model = new PlaceCardViewModel()
            {
                List = _repository.getCmsPlaceCards(Domain),

                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult Create(Guid id)
        {
            PlaceCardViewModel model = new PlaceCardViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            
            DateTime DateStart = DateTime.Now;

            ViewBag.DateStart = DateStart.ToString("yyyy-MM-dd");
            ViewBag.DateEnd = DateStart.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.TimeStart = ViewBag.TimeEnd = DateStart.ToString("HH:mm");


            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn")]
        public ActionResult Create(Guid id,PlaceCardViewModel model)
        {
            ViewBag.BtnName = "Создать";

            model.Item.Year = DateTime.Now.ToString("yyyy");
            model.Item.Month = DateTime.Now.ToString("MM");
            model.Item.Day = DateTime.Now.ToString("dd");
            model.Item.DateStart = model.Item.DateStart + model.Item.TimeStart;
            model.Item.DateEnd = model.Item.DateEnd + model.Item.TimeEnd;
            model.Item.Site = Domain;

            if (ModelState.IsValid)
            {
                if (model.Item.Alias == null) { model.Item.Alias = Transliteration.Translit(model.Item.Title.ToString()); }
                _repository.createPlaceCard(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);

                ViewBag.Message = "Запись добавлена";
                ViewBag.backurl = id;
            }

            ViewBag.DateStart = model.Item.DateStart.ToString("yyyy-MM-dd");
            ViewBag.TimeStart = model.Item.DateStart.ToString("HH:mm");
            ViewBag.DateEnd = model.Item.DateEnd.ToString("yyyy-MM-dd");
            ViewBag.TimeEnd = model.Item.DateEnd.ToString("HH:mm");

            model = new PlaceCardViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo,

                Item = _repository.getPlaceCard(Domain, id)
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
            PlaceCardViewModel model = new PlaceCardViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo,

                Item = _repository.getPlaceCard(Domain, id)
            };
            if (model.Item != null)
            {
                ViewBag.DateStart = Convert.ToDateTime(model.Item.DateStart).ToString("yyyy-MM-dd");
                ViewBag.TimeStart = Convert.ToDateTime(model.Item.DateStart).ToString("HH:mm");
                ViewBag.DateEnd = Convert.ToDateTime(model.Item.DateEnd).ToString("yyyy-MM-dd");
                ViewBag.TimeEnd = Convert.ToDateTime(model.Item.DateEnd).ToString("HH:mm");
            }
            else {
                DateTime DateStart = DateTime.Now;

                ViewBag.DateStart = DateStart.ToString("yyyy-MM-dd");
                ViewBag.DateEnd = DateStart.AddDays(1).ToString("yyyy-MM-dd");
                ViewBag.TimeStart = ViewBag.TimeEnd = DateStart.ToString("HH:mm");
            }


            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn")]
        public ActionResult Edit(Guid id, PlaceCardViewModel model)
        {
            ViewBag.BtnName = "Сохранить";

            model.Item.Site = Domain;
            model.Item.DateStart = model.Item.DateStart + model.Item.TimeStart;
            model.Item.DateEnd = model.Item.DateEnd + model.Item.TimeEnd;

            if (ModelState.IsValid)
            {
                _repository.updatePlaceCard(id, model.Item);

                _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);
                ViewBag.Message = "Запись сохранена";
                ViewBag.backurl = id;
            }

            ViewBag.DateStart = Convert.ToDateTime(model.Item.DateStart).ToString("yyyy-MM-dd");
            ViewBag.TimeStart = Convert.ToDateTime(model.Item.DateStart).ToString("HH:mm");
            ViewBag.DateEnd = Convert.ToDateTime(model.Item.DateEnd).ToString("yyyy-MM-dd");
            ViewBag.TimeEnd = Convert.ToDateTime(model.Item.DateEnd).ToString("HH:mm");

            model = new PlaceCardViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo,

                Item = _repository.getPlaceCard(Domain, id)
            };
            return View("Item", model);
        }
           
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _repository.deletePlaceCard(Domain, id);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);

            ViewBag.Message = "Запись удалена";
            ViewBag.backurl = id;
            PlaceCardViewModel model = new PlaceCardViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo
            };
            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            return RedirectToAction("", "PlaceCard");
        }
    }
}