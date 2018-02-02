using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class FeedbackController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private FeedbackViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new FeedbackViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                CurrentPage = currentPage
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Обратная связь";
            ViewBag.Description = "Обратная связь с сайтом";
            ViewBag.KeyWords = "Часто задаваемые вопросы, отзывы, задать вопрос";
            #endregion

        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Appeallist");
        }

        /// <summary>
        /// Список обращений
        /// </summary>
        /// <returns></returns>
        public ActionResult Appeallist(string sendStatus = "new", string message = "")
        {
            #region currentPage
            currentPage = _repository.getSiteMap(_path, _alias);
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
                model.Child = (currentPage.ParentId.HasValue) ? _repository.getSiteMapChild(currentPage.ParentId.Value) : null;
            }
            #endregion

            

            var filter = getFilter();
            filter.Disabled = false;
            filter.Type = FeedbackType.appeal.ToString();             //Только вопросы
            model.List = _repository.getFeedbacksList(filter);

            #region sort filters ViewBag
            ViewBag.FilterSearchText = filter.SearchText;
            ViewBag.FilterDate = filter.Date;
            ViewBag.FilterDateEnd = filter.DateEnd;
            #endregion

            ViewBag.FbType = FeedbackType.appeal;
            ViewBag.FormStatus = sendStatus;
            ViewBag.IsAgree = false;
            ViewBag.Anonymous = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            return View(model);
        }

        /// <summary>
        /// Список отзывов
        /// </summary>
        /// <returns></returns>
        public ActionResult Reviewlist(string sendStatus = "new")
        {
            #region currentPage
            currentPage = _repository.getSiteMap(_path, _alias);
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
                model.Child = (currentPage.ParentId.HasValue) ? _repository.getSiteMapChild(currentPage.ParentId.Value) : null;
            }
            #endregion

            var filter = getFilter();
            filter.Disabled = false;
            //Только отзывы
            filter.Type = FeedbackType.review.ToString();
            model.List = _repository.getFeedbacksList(filter);


            #region sort filters ViewBag
            ViewBag.FilterSearchText = filter.SearchText;
            ViewBag.FilterDate = filter.Date;
            ViewBag.FilterDateEnd = filter.DateEnd;
            #endregion

            ViewBag.FbType = FeedbackType.review;
            ViewBag.FormStatus = sendStatus;
            ViewBag.IsAgree = false;
            ViewBag.Anonymous = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            return View(model);
        }

        /// <summary>
        /// Форма отправки обращения
        /// </summary>
        /// <returns></returns>
        //public ActionResult Form()
        //{
        //    model.Child = (model.CurrentPage != null && model.CurrentPage.ParentId.HasValue)
        //           ? _repository.getSiteMapChild(model.CurrentPage.ParentId.Value) : null;

        //    #region Создаем переменные (значения по умолчанию)
        //    string _ViewName = (ViewName != String.Empty) ? "Form" : "~/Views/Error/CustomError.cshtml";

        //    string PageTitle = "Форма обратной связи";
        //    string PageDesc = "описание страницы";
        //    string PageKeyw = "ключевые слова";
        //    #endregion

        //    #region Метатеги
        //    ViewBag.Title = PageTitle;
        //    ViewBag.Description = PageDesc;
        //    ViewBag.KeyWords = PageKeyw;
        //    #endregion

        //    ViewBag.IsAgree = false;
        //    ViewBag.Anonymous = false;
        //    ViewBag.captchaKey = Settings.CaptchaKey;

        //    if (!string.IsNullOrEmpty(getFilter().Type))
        //    {
        //        var fbType = FeedbackType.appeal;
        //        var res = Enum.TryParse(getFilter().Type, out fbType);

        //        if(res)
        //            ViewBag.FbType = fbType;
        //    }

        //    return View(_ViewName, model);
        //}

        /// <summary>
        /// Отправка обращения
        /// </summary>
        /// <param name="bindData"></param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "send-btn")]
        public ActionResult SendForm(FeedbackFormViewModel bindData)
        {
            var newId = Guid.NewGuid();
            string PrivateKey = Settings.SecretKey;
            string EncodedResponse = Request["g-Recaptcha-Response"];
            bool IsCaptchaValid = (ReCaptchaClass.Validate(PrivateKey, EncodedResponse) == "True" ? true : false);

            var formStatus = "new";
            
            if (ModelState.IsValid && IsCaptchaValid)
            {
                var AnswererCode = Guid.NewGuid();
                var newMessage = new FeedbackModel()
                {
                    Id = newId,
                    IsNew = true,
                    Disabled = true,
                    Date = DateTime.Now,
                    SenderName = bindData.SenderName,
                    SenderEmail = bindData.SenderEmail,
                    SenderContacts = bindData.SenderContacts,
                    Title = !string.IsNullOrEmpty(bindData.Theme)
                            ? bindData.Theme :
                            (bindData.Text.Length > 126) ? bindData.Text.Substring(0, 126) + " ..." : bindData.Text,
                    Text = bindData.Text,
                    Anonymous = bindData.Anonymous,
                    AnswererCode = AnswererCode,
                    FbType = bindData.FbType
                };
                //var res = _repository.insertFeedbackItem(newMessage);

                var res = false;
                var fileLink = "";
                if (res)
                {
                    var savedFileName = "";
                    if (bindData.FileToUpload != null)
                    {
                        string savePath = Settings.UserFiles + Domain + Settings.FeedbacksDir + newId.ToString() + "/";
                        if (!Directory.Exists(savePath))
                            Directory.CreateDirectory(Server.MapPath(savePath));

                        savedFileName = Path.Combine(Server.MapPath(savePath), Path.GetFileName(bindData.FileToUpload.FileName));
                        bindData.FileToUpload.SaveAs(savedFileName);
                        fileLink = "<a href=\"" + Settings.BaseURL + savePath + Path.GetFileName(bindData.FileToUpload.FileName) + "\">" + bindData.FileToUpload.FileName + "</a>";
                    }

                    #region отправка сообщение на e-mail.ru
                    // domen_ru/feedback/answerform?id=db4609c2-c5dc-4f4a-9d6a-542192ec7cb3&code=b6614768-fa5f-4a27-bd09-d07a3e6db1a9
                    var domainUrl = _repository.getSiteDefaultDomain(Domain);
                    if (string.IsNullOrEmpty(domainUrl))
                        domainUrl = Settings.BaseURL;

                    var answerLink = string.Format("http://{0}/feedback/answerform?id={1}&code={2}", domainUrl, newId, AnswererCode);

                    var msgText = string.Format(
                        "<div style=\"background-color:#fcf8e3;padding:15px;border:1px solid #eee; border-radius:4px;\">"
                        + "<p>Уважаемый администратор сайта {0}!<br />"
                        + "У вас 1 новое сообщение, отправленное через форму обратной связи сайта:</p>"

                        + "<p style=\"padding-bottom:30px;\">"
                        + "<b>Заявитель:</b> {1}<br />"
                        + "<b>E-mail:</b>{2}<br />"
                        + "<b>Дополнительные контакты:</b> {3}<br />"
                        + "</p>"
                        + "<p style=\"padding-bottom:30px;\">"
                        + "<b>Тема обращения:</b><br />{4}<br /><br />"
                        + "<b>Текст обращения:</b><br />{5}"
                        + "</p>"
                        + "<p>"
                        + "Ответить на обращение можно по ссылке: <a href=\"{6}\">Ответить на обращение</a>"
                        + "</p>"
                        + "<p>"
                        + "Прикрепленый файл: {7}"
                        + "</p>"
                        + "</div>"

                        + "<hr />"
                        + "<div style=\"font-size:12px;line-height:16px;color:#9c9c9c;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;\">"
                        + "Сообщение было создано почтовым роботом. Пожалуйста, не отвечайте на него."
                        + "</div>",
                        Domain,
                        bindData.SenderName,
                        bindData.SenderEmail,
                        bindData.SenderContacts,
                        bindData.Theme,
                        bindData.Text,
                        answerLink,
                        fileLink
                        );

                    Mailer letter = new Mailer();
                    letter.isSsl = true;
                    letter.Theme = "Сайт " + Domain + ": Обратная связь";
                    letter.Text = msgText;
                    letter.Attachments = savedFileName;
                    letter.MailTo = Settings.mailTo;

                    var admins = _repository.getSiteAdmins();
                    if (admins != null)
                    {
                        letter.MailTo = string.Join(";", admins.Select(s => s.EMail.ToString()));
                    }

                    var errorText = letter.SendMail();
                    #endregion

                    formStatus = "send";

                    if (bindData.FbType == FeedbackType.review)
                        return RedirectToAction("Reviewlist", new { sendStatus = formStatus });

                    return RedirectToAction("Appeallist", new { sendStatus = formStatus });
                }
                else
                    formStatus = "error";
            }
            else
                formStatus = "captcha";

          //В случае ошибок в форме
            var _alias = "appeallist";
            if (bindData.FbType != FeedbackType.appeal)
                _alias = "reviewlist";

            #region currentPage
            currentPage = _repository.getSiteMap(_path, _alias);
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
                model.Child = (currentPage.ParentId.HasValue) ? _repository.getSiteMapChild(currentPage.ParentId.Value) : null;
            }
            #endregion

            var filter = getFilter();
            filter.Disabled = false;
            //Только отзывы
            filter.Type = bindData.FbType.ToString();
            model.List = _repository.getFeedbacksList(filter);

            #region sort filters ViewBag
            ViewBag.FilterSearchText = filter.SearchText;
            ViewBag.FilterDate = filter.Date;
            ViewBag.FilterDateEnd = filter.DateEnd;
            #endregion

            #region postback data
            ViewBag.SenderName = bindData.SenderName;
            ViewBag.SenderEmail = bindData.SenderEmail;
            ViewBag.SenderContacts = bindData.SenderContacts;
            ViewBag.FbType = bindData.FbType;
            ViewBag.Theme = bindData.Theme;
            ViewBag.Text = bindData.Text;
            #endregion

            ViewBag.FormStatus = formStatus;
            ViewBag.IsAgree = false;
            ViewBag.Anonymous = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            return View(model);
        }

        /// <summary>
        /// Форма для отправки ответа на обращение
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult AnswerForm(Guid id, Guid code)
        {
            string _ViewName = (ViewName != String.Empty) ? "AnswerForm" : "~/Views/Error/CustomError.cshtml";

            var feedbackItem = _repository.getFeedbackItem(id);
            if (feedbackItem == null || (feedbackItem != null && feedbackItem.AnswererCode.HasValue && feedbackItem.AnswererCode.Value != code))
            {
                var errorModel = new ErrorViewModel()
                {
                    Title = "Ошибка ",
                    HttpCode = 404,
                    Message = "Страница не найдена, убедитесь в правильности набранного адреса.",
                    BackUrl = "/feedbacks/"
                };
                return View("~/Views/Error/Index.cshtml", errorModel);
            }

            ViewBag.ByEmail = false;
            ViewBag.Publish = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            model.Item = feedbackItem;
            return View(_ViewName, model);
        }

        /// <summary>
        /// Отправка ответа на обращение (также емайл пользователю и администратору). меняем код доступа
        /// </summary>
        /// <param name="bindData"></param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "answer-btn")]
        public ActionResult AnswerForm(FeedbackAnswerFormViewModel bindData)
        {
            var errorText = "";
            ViewBag.ByEmail = true;
            ViewBag.captchaKey = Settings.CaptchaKey;

            var feedbackItem = _repository.getFeedbackItem(bindData.Id);
            if (feedbackItem == null || (feedbackItem != null && feedbackItem.AnswererCode.HasValue && feedbackItem.AnswererCode.Value != bindData.AnswererCode))
                throw new Exception("Попытка отправить ответ на сообщение с неверным кодом!");

            string PrivateKey = Settings.SecretKey;
            string EncodedResponse = Request["g-Recaptcha-Response"];
            bool IsCaptchaValid = (ReCaptchaClass.Validate(PrivateKey, EncodedResponse) == "True" ? true : false);

            if (ModelState.IsValid && IsCaptchaValid)
            {
                var newAnswererCode = Guid.NewGuid();
                feedbackItem.Answer = bindData.Answer;
                feedbackItem.Answerer = bindData.Answerer;
                feedbackItem.Disabled = !bindData.Publish;
                feedbackItem.AnswererCode = newAnswererCode;

                var res = _repository.updateFeedbackItem(feedbackItem);
                if (res)
                {
                    var page = "appeallist";
                    if (feedbackItem.FbType == FeedbackType.review)
                        page = "reviewlist";

                    var domainUrl = _repository.getSiteDefaultDomain(Domain);
                    if (string.IsNullOrEmpty(domainUrl))
                        domainUrl = Settings.BaseURL;

                    // domen_ru/feedback/reviewlist#feedback_b1d60415-d600-44bf-82f3-27d56f371fb1
                    var feedbackLink = string.Format("http://{0}/feedback/{1}#feedback_{2}", domainUrl, page, bindData.Id);

                    //Если стоит галочка отправить ответ по емайл? тому кто задал вопрос
                    if (bindData.ByEmail)
                    {
                        #region отправка сообщения на e-mail.ru
                        var msgText1 = string.Format(
                        "<div>"
                        + "<p style=\"padding-bottom:30px;\">Уважаемый {0}!</p>"
                        + "Ваше обращение:"

                        + "<p style=\"background-color:#fcf8e3;padding:15px;border:1px solid #eee; border-radius:4px; padding-bottom:30px;\">"
                        + "<b>Дата:</b><br />{1}<br /><br />"
                        + "<b>Тема обращения:</b><br />{2}<br /><br />"
                        + "<b>Текст обращения:</b><br />{3}"
                        + "</p>"

                        + "<p style=\"padding-bottom:30px;\">"
                        + "рассмотрено, <b>отвечает:</b><br />{4}<br /><br />"
                        + "<b>Ответ на обращение:</b><br />{5}"
                        + "</p>"

                         + "<p style=\"padding-bottom:30px;\">"
                        + "Также ответ опубликован на сайте. <a href=\"{6}\">Перейти</a>"
                        + "</p>"
                        + "</div>"

                        + "<hr />"
                        + "<div style=\"font-size:12px;line-height:16px;color:#9c9c9c;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;\">"
                        + "Сообщение было создано почтовым роботом. Пожалуйста, не отвечайте на него."
                        + "</div>",
                        feedbackItem.SenderName,
                        feedbackItem.Date,
                        feedbackItem.Title,
                        feedbackItem.Text,
                        feedbackItem.Answerer,
                        feedbackItem.Answer,
                        feedbackLink
                        );

                        Mailer letter1 = new Mailer();
                        letter1.isSsl = true;
                        letter1.Theme = "Сайт " + Domain + ": Обратная связь";
                        letter1.Text = msgText1;
                        letter1.MailTo = feedbackItem.SenderEmail;

                        errorText = letter1.SendMail();
                        #endregion
                    }

                    #region отправка сообщения о редактировании, с новым кодом доступа
                    // domen_ru/feedback/answerform?id=db4609c2-c5dc-4f4a-9d6a-542192ec7cb3&code=b6614768-fa5f-4a27-bd09-d07a3e6db1a9
                    var answerEditLink = string.Format("http://{0}/feedback/answerform?id={1}&code={2}", domainUrl, bindData.Id, newAnswererCode);

                    var msgText2 = string.Format(
                        "<div style=\"background-color:#fcf8e3;padding:15px;border:1px solid #eee; border-radius:4px;\">"
                        + "<p>Уважаемый администратор сайта {0}!<br /><br />"
                        + "На сообщение (обратная связь):<br />"
                        + "<a href=\"{1}\">{2}</a><br /><br />"
                        + "специалистом был дан ответ.<br /> Чтобы отредактировать ответ на обращение, перейдите по ссылке: "
                         + "<a href=\"{3}\">Редактировать</a>"
                        + "</p>"

                        + "<hr />"
                        + "<div style=\"font-size:12px;line-height:16px;color:#9c9c9c;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;\">"
                        + "Сообщение было создано почтовым роботом. Пожалуйста, не отвечайте на него."
                        + "</div>",
                        Domain,
                        feedbackLink,
                        feedbackLink,
                        answerEditLink
                        );

                    Mailer letter2 = new Mailer();
                    letter2.isSsl = true;
                    letter2.Theme = "Сайт " + Domain + ": Обратная связь";
                    letter2.Text = msgText2;
                    letter2.MailTo = Settings.mailTo;

                    var admins = _repository.getSiteAdmins();
                    if (admins != null)
                    {
                        letter2.MailTo = string.Join(";", admins.Select(s => s.EMail.ToString()));
                    }

                    errorText += "<br />" + letter2.SendMail();
                    #endregion

                    ViewBag.FormStatus = "send";
                }
            }
            else
            {
                ViewBag.FormStatus = "captcha";
                ViewBag.Answer = bindData.Answer;
                ViewBag.Answerer = bindData.Answerer;
            }

            ViewBag.ByEmail = true;
            ViewBag.IsAgree = false;

            model.Item = feedbackItem;

            if (feedbackItem.FbType == FeedbackType.review)
                return RedirectToAction("Reviewlist");

            return RedirectToAction("Appeallist");
            //return View(_ViewName, model);
        }
    }
}
