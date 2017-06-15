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
    public class SiteSettingsController : CoreController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Страница по умолчанию (Список)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            SitesViewModel model = new SitesViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                Site = _repository.getSite(Domain)
            };

            return View(model);
        }

        /// <summary>
        /// Поиск в списке
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(SitesViewModel model)
        {
            Guid id = _repository.getSite(Domain).Id;

            if (ModelState.IsValid)
            {
                _repository.updateSite(id, model.Site);
                _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);

                ViewBag.Message = "Запись обновлена";
                ViewBag.backurl = id;
            }

            model = new SitesViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                Site = _repository.getSite(Domain)
            };

            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return RedirectToAction("", (String)RouteData.Values["controller"]);
        }
    }
}