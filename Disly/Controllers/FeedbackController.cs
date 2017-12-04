using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
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
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var filter = getFilter();
            filter.Disabled = false;

            model.List = _repository.getFeedbacksList(filter);

            model.Child = (model.CurrentPage != null) ? _repository.getSiteMapChild(model.CurrentPage.Id) : null;

            ViewBag.Filter = filter;
            ViewBag.NewsSearchArea = filter.SearchText;
            ViewBag.NewsSearchDateStart = filter.Date;
            ViewBag.NewsSearchDateFin = filter.DateEnd;

            #region Создаем переменные (значения по умолчанию)
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Обратная связь";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion
            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
            return View(_ViewName, model);
        }

        public ActionResult Form()
        {
            model.Child = (model.CurrentPage != null && model.CurrentPage.ParentId.HasValue)
                   ? _repository.getSiteMapChild(model.CurrentPage.ParentId.Value) : null;

            #region Создаем переменные (значения по умолчанию)
            string _ViewName = (ViewName != String.Empty) ? "Form" : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Форма обратной связи";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            ViewBag.ByEmail = true;
            ViewBag.IsAgree = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            return View(_ViewName, model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "send-btn")]
        public ActionResult Form(FeedbackFormViewModel bindData)
        {
            model.Child = (model.CurrentPage != null) ? _repository.getSiteMapChild(model.CurrentPage.Id) : null;

            #region Создаем переменные (значения по умолчанию)
            string _ViewName = (ViewName != String.Empty) ? "Form" : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Форма обратной связи";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            ViewBag.ByEmail = true;
            ViewBag.captchaKey = Settings.CaptchaKey;

            var newId = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                var newMessage = new FeedbackModel()
                {
                    Id = newId,
                    IsNew = true,
                    Disabled = true,
                    Date = DateTime.Now,
                    SenderName = bindData.SenderName,
                    SenderEmail = bindData.SenderEmail,
                    SenderContacts = bindData.SenderContacts,
                    Title = bindData.Theme,
                    Text = bindData.Text,
                    AnswererCode = Guid.NewGuid()
                };
                var res = _repository.insertFeedbackItem(newMessage);
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

                        //Letter.Attachments = savedFileName;
                        //model.File = savePath + upload.FileName;
                    }

                    var feedback = _repository.getFeedbackItem(newId);
                    var code = (feedback!= null && feedback.AnswererCode != null)? feedback.AnswererCode: Guid.NewGuid();

                    #region отправка сообщение на e-mail.ru

                    // domen_ru/feedback/answerform?id=db4609c2-c5dc-4f4a-9d6a-542192ec7cb3&code=b6614768-fa5f-4a27-bd09-d07a3e6db1a9
                    var answerLink = string.Format("www.{0}/feedback/answerform?id={1}&code={2}", Domain, newId, code);

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
                        + "Ответить на сообщение можно по ссылке: <a href=\"{6}\">Ответить на обращение</a>"
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
                        answerLink
                        );

                    Mailer letter = new Mailer();
                    letter.isSsl = true;
                    letter.Theme = "Сайт " + Domain + ": Обратная связь";
                    letter.Text = msgText;
                    letter.Attachments = savedFileName;
                    letter.MailTo = "internet@it-serv.ru"; // "s-kuzmina@it-serv.ru";
                    //if (organization == null)
                    //{
                    //    Letter.MailTo = Model.SitesItemInfo.C_Email;
                    //}
                    //else
                    //{
                    //    Letter.MailTo = organization.Email;
                    //}

                    var errorText = letter.SendMail();
                    #endregion
                    ViewBag.FormStatus = "send";
                }
            }
            else
            {
                ViewBag.FormStatus = "captcha";
                ViewBag.SenderName = bindData.SenderName;
            }
            ViewBag.ByEmail = true;
            ViewBag.IsAgree = false;

            return View(_ViewName, model);
        }

        /// <summary>
        /// Форма для отправки сообщения
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult AnswerForm(Guid id, Guid code)
        {
            #region Создаем переменные (значения по умолчанию)
            string _ViewName = (ViewName != String.Empty) ? "AnswerForm" : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Форма ответа на обращение";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            ViewBag.ByEmail = false;
            ViewBag.Publish = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            var feedbackItem = _repository.getFeedbackItem(id);
            if(feedbackItem == null || (feedbackItem!= null && feedbackItem.AnswererCode.HasValue && feedbackItem.AnswererCode.Value != code))
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

            model.Item = feedbackItem;
            return View(_ViewName, model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "answer-btn")]
        public ActionResult AnswerForm(FeedbackAnswerFormViewModel bindData)
        {
            #region Создаем переменные (значения по умолчанию)
            string _ViewName = (ViewName != String.Empty) ? "AnswerForm" : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Форма ответа на обращение";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            ViewBag.ByEmail = true;
            ViewBag.captchaKey = Settings.CaptchaKey;

            if (ModelState.IsValid)
            {
                var feedbackItem = _repository.getFeedbackItem(bindData.Id);
                if (feedbackItem == null || (feedbackItem != null && feedbackItem.AnswererCode.HasValue && feedbackItem.AnswererCode.Value != bindData.AnswererCode))
                    throw new Exception("Попытка отправить ответ на сообщение с неверным кодом!");


                feedbackItem.Answer = bindData.Answer;
                feedbackItem.Answerer = bindData.Answerer;
                feedbackItem.Disabled = !bindData.Publish;

                var res = _repository.updateFeedbackItem(feedbackItem);
                if (res)
                {
                    if (bindData.ByEmail)
                    {
                        #region отправка сообщение на e-mail.ru
                        var answerLink = string.Format("www.{0}/feedback/#{1}", Domain, bindData.Id);
                        var msgText = string.Format(
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
                        answerLink
                        );

                        Mailer letter = new Mailer();
                        letter.isSsl = true;
                        letter.Theme = "Сайт " + Domain + ": Обратная связь";
                        letter.Text = msgText;
                        letter.MailTo = feedbackItem.SenderEmail;

                        var errorText = letter.SendMail();
                        #endregion
                    }

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

            return View(_ViewName, model);
        }
    }
}
