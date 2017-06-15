using Disly.Areas.Admin.Models;
using System;
using System.Reflection;
using System.Net;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class PageViewsController : CoreController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            #region Метатеги
            ViewBag.Title = "Шаблоны страниц";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: PageViews
        public ActionResult Index()
        {
            PageViewsViewModel model = new PageViewsViewModel()
            {
                List = _repository.getCmsPageViews(),
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };            
            return View(model);            
        }

        public ActionResult Create(Guid Id)
        {
            PageViewsViewModel model = new PageViewsViewModel()
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
            PageViewsViewModel model = new PageViewsViewModel()
            {
                Item = _repository.getCmsPageViews(id),
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            return View("Item", model);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn")]
        public ActionResult Edit(Guid id, PageViewsViewModel model)
        {
            ViewBag.BtnName = "Сохранить";
            if (ModelState.IsValid)
            {
                _repository.setCmsPageViews(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);

                ViewBag.Message = "Запись сохранена";
                ViewBag.backurl = id;
            }

            model = new PageViewsViewModel()
            {
                Item = _repository.getCmsPageViews(id),
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
            };

            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn")]
        public ActionResult Create(Guid id, PageViewsViewModel model)
        {
            ViewBag.BtnName = "Создать";
            if (ModelState.IsValid)
            {
                _repository.insCmsPageViews(id, model.Item);
                _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);

                ViewBag.Message = "Запись добавлена";
                ViewBag.backurl = id;
            }

            model = new PageViewsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
            };

            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ViewBag.Message = "Запись удалена";
            ViewBag.backurl = id;
            PageViewsViewModel model = new PageViewsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
            };
            _repository.delCmsPageViews(id);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);
            return View("Item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            return RedirectToAction("", "pageviews");
        }
    }
}