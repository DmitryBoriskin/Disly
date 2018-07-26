using Disly.Areas.Admin.Models;
using System;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class MainController : CoreController
    {
        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //_cmsRepository.NormalizeDepartamnt(); //робот нормализующий поля n_sort в департаментах
            MainViewModel model = new MainViewModel()
            {
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
            };
            if (AccountInfo != null)
            {
                model.Menu = _cmsRepository.getCmsMenu(AccountInfo.Id);
                model.AccountLog = _cmsRepository.getCmsUserLog(AccountInfo.Id);
            }

            _cmsRepository.permit_OrgsAdminstrativ(Guid.Parse("c8d42fea-dced-4108-9ae8-fd91f184810a"), Guid.Parse("cc7c029a-a19e-11e1-8e66-b803051cdedb"), 6);
            #region Метатеги
            ViewBag.Title = "Главная";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
    }
}