using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disly.Areas.Admin.Models;
using System.Web.Mvc;
using cms.dbModel.entity;

namespace Disly.Areas.Admin.Controllers
{
    public class SettingsController : CoreController
    {
        /// <summary>
        /// Страница настроек
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            SettingsViewModel model = new SettingsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                siteSettings = _repository.getCmsSettings()
            };

            return View(model);
        }

        /// <summary>
        /// Страница настроек
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(SettingsViewModel Object)
        {
            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            try {
                if (ModelState.IsValid) {
                    _repository.updateCmsSettings(Object.siteSettings);
                    _repository.insertLog(Object.siteSettings.Guid, AccountInfo.id, "update", RequestUserInfo.IP);
                    
                    #region оповещение на e-mail
                    string ErrorText = "";
                    string Massege = String.Empty;
                    Mailer Letter = new Mailer();
                    Letter.Theme = "Обновилась информация в настройках сайта";
                    Massege = "<p>Уважаемый пользователь</p>";
                    Massege += "<p>Произошло изменение настроек вашего сайта</p>";                    
                    Letter.MailTo = "aleksandr-mikhailov@it-serv.ru";
                    Letter.Text = Massege;
                    ErrorText = Letter.SendMail();
                    #endregion
                }

                SettingsViewModel model = new SettingsViewModel()
                {
                    Account = AccountInfo,
                    Settings = SettingsInfo,
                    UserResolution = UserResolutionInfo,

                    siteSettings = _repository.getCmsSettings()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Exeption = ex.ToString();
                return RedirectToAction("Error"); // на страницу ошибок
            }

        }

    }
}
