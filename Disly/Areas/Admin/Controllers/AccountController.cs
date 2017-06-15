using cms.dbase;
using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Disly.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        protected bool _IsAuthenticated = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected AccountRepository _accountRepository;
        protected cmsRepository _cmsRepository;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _accountRepository = new AccountRepository("cmsdbConnection");
            _cmsRepository = new cmsRepository("cmsdbConnection");
            //SettingsModel Settings = _cmsRepository.getCmsSettings();

            #region Метатеги
            ViewBag.Title = "Авторизация";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";

            ViewBag.SiteName = "Имя сайта";//Settings.Title;
            ViewBag.SiteURL = "site";//Settings.Url;
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult LogIn()
        {
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            else return View();
        }

        /// <summary>
        /// Авторизация в CMS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogIn(LogInModel model)
        {
            try
            {                
                // Ошибки в форме
                if (!ModelState.IsValid) return View(model);

                string _login = model.Login;
                string _pass = model.Pass;
                bool _remember = model.RememberMe;

                AccountModel AccountInfo = _accountRepository.getCmsAccount(_login);

                string Salt = string.Empty;
                string Hash = string.Empty;

                if (AccountInfo != null)
                {
                    Salt = AccountInfo.Salt;
                    Hash = AccountInfo.Hash;
                }
                
                Cripto password = new Cripto(Salt, Hash);
                if (password.Verify(_pass.ToCharArray()))
                {
                    FormsAuthentication.SetAuthCookie(AccountInfo.id.ToString(), _remember);
                    _accountRepository.insertLog(AccountInfo.id, AccountInfo.id, "login", RequestUserInfo.IP);

                    return RedirectToAction("Index", "Main");
                }
                else
                {
                    ModelState.AddModelError("", "Пара логин и пароль не подходят. Попробуйте ещё раз");
                }

                return View();
            }
            catch (HttpAntiForgeryException ex)
            {
                return View();
            }
        }
        
        /// <summary>
        /// Форма "Напомнить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult RestorePass()
        {
            ViewBag.Title = "Напомнить пароль";
            ViewBag.SiteAdress = Request.Url.Authority.Substring(0, Request.Url.Authority.IndexOf(":"));

            if (_IsAuthenticated) return RedirectToAction("", "Main");
            else return View();
        }

        /// <summary>
        /// Форма "Напомнить пароль"
        /// </summary>
        /// <param name="model"></param>
        /// <returns>отправляем письмо пользователю с новым паролем</returns>
        [HttpPost]
        public ActionResult RestorePass(RestoreModel model)
        {
            ViewBag.Title = "Напомнить пароль";
            ViewBag.SiteAdress = Request.Url.Authority.Substring(0, Request.Url.Authority.IndexOf(":"));

            try
            {
                string _login = model.Email;
                AccountModel AccountInfo = _accountRepository.getCmsAccount(_login);

                // Ошибки в форме
                if (!ModelState.IsValid)
                {
                    // пустое поле
                    if (_login == null || _login == "")
                    {
                        ModelState.AddModelError("", "Поле \"E-Mail\" не заполнено. Для восстановления пароля введите адрес почты.");
                    }
                    return View(model);
                }

                // существует ли адрес
                if (AccountInfo != null)
                {
                    string newPass = Membership.GeneratePassword(8, 0);

                    Cripto pass = new Cripto(newPass.ToCharArray());
                    string NewSalt = pass.Salt;
                    string NewHash = pass.Hash;
                    _accountRepository.changePasswordUser(AccountInfo.id, NewSalt, NewHash, RequestUserInfo.IP);

                    #region оповещение на e-mail
                    //string ErrorText = "";
                    //string Massege = String.Empty;
                    //Mailer Letter = new Mailer();
                    //Letter.Theme = "Изменение пароля";
                    //Massege = "<p>Уважаемый " + AccountInfo.Surname + " " + AccountInfo.Name + "</p>";
                    //Massege += "<p>Ваш пароль на сайте был изменен</p>";
                    //Massege += "<p>Ваш новый пароль:<b>" + newPass + "</b></p>";
                    //Massege += "<p>С уважением, администрация портала!</p>";
                    //Massege += "<hr><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</span>";
                    //Letter.MailTo = AccountInfo.Mail;
                    //Letter.Text = Massege;
                    //ErrorText = Letter.SendMail();
                    #endregion

                    return RedirectToAction("ConfirmRestorePass", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Адрес почты заполнен неверно. Попробуйте ещё раз");
                }
                return View();

            }
            catch (HttpAntiForgeryException ex)
            {
                return View();
            }
        }

        /// <summary>
        /// Форма "Изменить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmRestorePass()
        {
            ViewBag.Title = "Напомнить пароль";
            ViewBag.SiteAdress = Request.Url.Authority.Substring(0, Request.Url.Authority.IndexOf(":"));

            return View();
        }

        /// <summary>
        /// Форма "Изменить пароль"
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToLoginRestorePass()
        {
            ViewBag.Title = "Напомнить пароль";
            ViewBag.SiteAdress = Request.Url.Authority.Substring(0, Request.Url.Authority.IndexOf(":"));

            return RedirectToAction("LogIn", "Account");
        }

        /// <summary>
        /// Закрываем сеанс работы с CMS
        /// </summary>
        /// <returns></returns>
        public ActionResult logOff()
        {
            AccountModel AccountInfo = _accountRepository.getCmsAccount(new Guid(User.Identity.Name));
            _accountRepository.insertLog(AccountInfo.id, AccountInfo.id, "log_off", RequestUserInfo.IP);
            FormsAuthentication.SignOut();

            return RedirectToAction("LogIn", "Account");
        }

    }
}