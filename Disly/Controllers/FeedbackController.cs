using cms.dbase;
using cms.dbModel.entity;
using Disly.Areas.Admin.Service;
using Disly.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
                BannerArrayLayout = bannerArrayLayout,
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
        public ActionResult Appeallist(string status = null)
        {
            #region currentPage
            currentPage = _repository.getSiteMap(_path, _alias);
            if (currentPage == null)
                //throw new Exception("model.CurrentPage == null");
                return RedirectToRoute("Error", new { httpCode = 404 });

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
            ViewBag.FormStatus = status;
            ViewBag.IsAgree = false;
            ViewBag.Anonymous = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            return View(model);
        }

        /// <summary>
        /// Список отзывов
        /// </summary>
        /// <returns></returns>
        public ActionResult Reviewlist(string status = null)
        {
            #region currentPage
            currentPage = _repository.getSiteMap(_path, _alias);
            if (currentPage == null)
                //throw new Exception("model.CurrentPage == null");
                return RedirectToRoute("Error", new { httpCode = 404 });

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
            ViewBag.FormStatus = status;
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
            Guid newId = Guid.NewGuid();

            string PrivateKey = Settings.SecretKey;
            string EncodedResponse = Request["g-Recaptcha-Response"];
            bool IsCaptchaValid = (ReCaptchaClass.Validate(PrivateKey, EncodedResponse) == "True" ? true : false);

            var formStatus = "";
            var errorMsg = new StringBuilder();

            var fileLink = "";

            if (ModelState.IsValid && IsCaptchaValid)
            {
                var AnswererCode = Guid.NewGuid();

                #region map bind data to FeedbackModel
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
                #endregion

                var fileValid = true;
                var savedFileName = "";
                if (bindData.FileToUpload != null && bindData.FileToUpload.ContentLength > 0)
                {
                    string savePath = Settings.UserFiles + Domain + Settings.FeedbacksDir + newId.ToString() + "/";
                    if (bindData.FileToUpload.ContentLength < 10485760)
                    {
                        //загружаем файлы
                        string[] exts = { "jpg", "jpeg", "png", "gif", "txt", "doc", "docx", "rtf", "xls", "xlsx", "xlm", "pdf", "zip", "7z", "rar" };

                        var fileName = Path.GetFileName(bindData.FileToUpload.FileName);
                        string _name = fileName.Substring(0, fileName.LastIndexOf("."));
                        string _exp = fileName.Substring(fileName.LastIndexOf(".") + 1);

                        fileName = Transliteration.Translit(_name) + "." + _exp;

                        if (!exts.Contains(_exp))
                        {
                            fileValid = false;
                            errorMsg.AppendFormat("К обращению нельзя прикреплять файлы формата {0}<br />", _exp);
                            frontLogger.Info(new Exception("Попытка прикрепить файл формата ." + _exp), "FeedbackController for site " + Domain + ":");
                        }
                        else
                        {
                            if (!Directory.Exists(savePath))
                                Directory.CreateDirectory(Server.MapPath(savePath));

                            savedFileName = Path.Combine(Server.MapPath(savePath), Path.GetFileName(bindData.FileToUpload.FileName));
                            bindData.FileToUpload.SaveAs(savedFileName);
                            fileLink = "<a href=\"" + Settings.BaseURL + savePath + Path.GetFileName(bindData.FileToUpload.FileName) + "\">" + bindData.FileToUpload.FileName + "</a>";
                        }
                    }
                    else
                    {
                        fileValid = false;
                        errorMsg.Append("Размер файла превышает допустимые 10Мб<br />");
                        frontLogger.Info(new Exception("Попытка прикрепить файл зазмером ." + bindData.FileToUpload.ContentLength), "FeedbackController for site " + Domain + ":");
                    }
                }
                if (fileValid)
                {
                    //Сохраняем в базу и отправка на почту
                    var insertRes = _repository.insertFeedbackItem(newMessage);
                    if (insertRes)
                    {
                        #region отправка сообщение на e-mail.ru

                        // domen_ru/feedback/answerform?id=db4609c2-c5dc-4f4a-9d6a-542192ec7cb3&code=b6614768-fa5f-4a27-bd09-d07a3e6db1a9
                        var domainUrl = _repository.getSiteDefaultDomain(Domain);
                        if (string.IsNullOrEmpty(domainUrl))
                            domainUrl = Settings.BaseURL;

                        var answerLink = string.Format("http://{0}/feedback/answerform?id={1}&code={2}", domainUrl, newId, AnswererCode);

                        var msg = new StringBuilder();
                        msg.Append("<div style=\"background-color:#fcf8e3;padding:15px;border:1px solid #eee; border-radius:4px;\">");
                        msg.AppendFormat("<p>Уважаемый администратор сайта {0}!<br />", Domain);
                        msg.Append("У вас 1 новое сообщение, отправленное через форму обратной связи сайта:</p>");

                        msg.Append("<p style=\"padding-bottom:30px;\">");
                        msg.AppendFormat("<b>Заявитель:</b> {0}<br />", bindData.SenderName);
                        msg.AppendFormat("<b>E-mail:</b>{0}<br />", bindData.SenderEmail);
                        msg.AppendFormat("<b>Дополнительные контакты:</b> {0}<br />", bindData.SenderContacts);
                        msg.Append("</p>");
                        msg.Append("<p style=\"padding-bottom:30px;\">");
                        msg.AppendFormat("<b>Тема обращения:</b><br />{0}<br /><br />", bindData.Theme);
                        msg.AppendFormat("<b>Текст обращения:</b><br />{0}", bindData.Text);
                        msg.Append("</p>");
                        msg.AppendFormat("<p>Ответить на обращение можно по ссылке: <a href=\"{0}\">Ответить на обращение</a></p>", answerLink);
                        if (string.IsNullOrEmpty(fileLink))
                            msg.AppendFormat("<p>Прикрепленый файл: {0}</p>", fileLink);
                        msg.Append("</div>");

                        //Подпись
                        var caption = new StringBuilder();
                        caption.Append("<hr />");
                        caption.Append("<div style=\"font-size:12px;line-height:16px;color:#9c9c9c;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;\">");
                        caption.Append("Сообщение было создано почтовым роботом. Пожалуйста, не отвечайте на него.");
                        caption.Append("</div>");

                        msg.Append(caption);

                        Mailer letter = new Mailer();
                        letter.isSsl = true;
                        letter.Domain = Domain;
                        letter.Theme = "Сайт " + Domain + ": Обратная связь";
                        letter.Text = msg.ToString();
                        letter.Attachments = savedFileName;
                        letter.MailTo = Settings.mailTo;
                       

                        var admins = _repository.getSiteAdmins();
                        if (admins != null)
                        {
                            letter.MailTo = string.Join(";", admins.Select(s => s.EMail.ToString()));
                        }

                        //letter.MailTo = "s-kuzmina@asoft21.ru";

                        //Логируем в SendMail, даже если админу не отправилось - в базу записалось
                        var sendingRes = letter.SendMail();

                        #endregion

                        if (bindData.FbType == FeedbackType.review)
                            return RedirectToAction("Reviewlist", new { status = "send" });

                        return RedirectToAction("Appeallist", new { status = "send" });
                    }
                    else
                    {
                        formStatus = "error";
                        errorMsg.Append("Не удалось отправить сообщение<br />");
                    }
                }
            }
            else
            {
                formStatus = "error";

                if (!IsCaptchaValid)
                    errorMsg.Append("Ошибка ввода captcha");
                else
                    errorMsg.Append("Заполните все обязательные поля");
            }

            //В случае ошибок в форме
            var _alias = "appeallist";
            if (bindData.FbType != FeedbackType.appeal)
                _alias = "reviewlist";

            #region currentPage
            currentPage = _repository.getSiteMap(_path, _alias);
            if (currentPage == null)
                return RedirectToRoute("Error", new { httpCode = 404 });

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

            ViewBag.IsAgree = false;
            ViewBag.Anonymous = false;
            ViewBag.captchaKey = Settings.CaptchaKey;

            ViewBag.FormStatus = formStatus;
            ViewBag.FormMsg = errorMsg.ToString();

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
            var errorMsg = new StringBuilder();

            //Подпись
            var caption = new StringBuilder();
            caption.Append("<hr />");
            caption.Append("<div style=\"font-size:12px;line-height:16px;color:#9c9c9c;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;\">");
            caption.Append("Сообщение было создано почтовым роботом. Пожалуйста, не отвечайте на него.");
            caption.Append("</div>");

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

                var updateRes = _repository.updateFeedbackItem(feedbackItem);
                if (updateRes)
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

                        var msg1 = new StringBuilder();
                        msg1.Append("<div>");
                        msg1.AppendFormat("<p style=\"padding-bottom:30px;\">Уважаемый {0}!</p>", feedbackItem.SenderName);
                        msg1.Append("Ваше обращение:");
                        msg1.Append("<p style=\"background-color:#fcf8e3;padding:15px;border:1px solid #eee; border-radius:4px; padding-bottom:30px;\">");
                        msg1.AppendFormat("<b>Дата:</b><br />{0}<br /><br />", feedbackItem.Date);
                        msg1.AppendFormat("<b>Тема обращения:</b><br />{0}<br /><br />", feedbackItem.Title);
                        msg1.AppendFormat("<b>Текст обращения:</b><br />{0}", feedbackItem.Text);
                        msg1.Append("</p>");

                        msg1.Append("<p style=\"padding-bottom:30px;\">");
                        msg1.AppendFormat("рассмотрено, <b>отвечает:</b><br />{0}<br /><br />", feedbackItem.Answerer);
                        msg1.AppendFormat("<b>Ответ на обращение:</b><br />{0}", feedbackItem.Answer);
                        msg1.Append("</p>");

                        msg1.Append("<p style=\"padding-bottom:30px;\">");
                        msg1.AppendFormat("Также ответ опубликован на сайте. <a href=\"{0}\">Перейти</a>", feedbackLink);
                        msg1.Append("</p>");
                        msg1.Append("</div>");

                        msg1.Append(caption);

                        Mailer letter = new Mailer();
                        letter.isSsl = true;
                        letter.Domain = Domain;
                        letter.Theme = "Сайт " + Domain + ": Обратная связь";
                        letter.Text = msg1.ToString();
                        letter.MailTo = feedbackItem.SenderEmail;

                        //Логируем в SendMail
                        var res1 = letter.SendMail();

                        #endregion
                    }

                    #region отправка сообщения о редактировании, с новым кодом доступа
                    // domen_ru/feedback/answerform?id=db4609c2-c5dc-4f4a-9d6a-542192ec7cb3&code=b6614768-fa5f-4a27-bd09-d07a3e6db1a9
                    var answerEditLink = string.Format("http://{0}/feedback/answerform?id={1}&code={2}", domainUrl, bindData.Id, newAnswererCode);

                    var msg2 = new StringBuilder();
                    msg2.Append("<div style=\"background-color:#fcf8e3;padding:15px;border:1px solid #eee; border-radius:4px;\">");
                    msg2.AppendFormat("<p>Уважаемый администратор сайта {0}!<br /><br />", Domain);
                    msg2.Append("На сообщение (обратная связь):<br />");
                    msg2.AppendFormat("<a href=\"{0}\">{1}</a><br /><br />", feedbackLink, feedbackLink);
                    msg2.Append("специалистом был дан ответ.<br /> Чтобы отредактировать ответ на обращение, перейдите по ссылке: ");
                    msg2.AppendFormat("<a href=\"{0}\">Редактировать</a>", answerEditLink);
                    msg2.Append("</p>");

                    msg2.Append(caption);

                    Mailer letter2 = new Mailer();
                    letter2.isSsl = true;
                    letter2.Domain = Domain;
                    letter2.Theme = "Сайт " + Domain + ": Обратная связь";
                    letter2.Text = msg2.ToString();
                    letter2.MailTo = Settings.mailTo;

                    var admins = _repository.getSiteAdmins();
                    if (admins != null)
                    {
                        letter2.MailTo = string.Join(";", admins.Select(s => s.EMail.ToString()));
                    }
                    //letter.MailTo = "s-kuzmina@asoft21.ru";

                    //Логируем в SendMail
                    var res2 = letter2.SendMail();

                    #endregion

                    if (feedbackItem.FbType == FeedbackType.review)
                        return RedirectToAction("Reviewlist");

                    return RedirectToAction("Appeallist");
                }
                else
                {
                    errorMsg.Append("Не удалось отправить сообщение<br />");
                }
            }
            else
            {
                if (!IsCaptchaValid)
                    errorMsg.Append("Ошибка ввода captcha<br />");
                else
                    errorMsg.Append("Заполните все обязательные поля<br />");
            }

            model.Item = feedbackItem;

            ViewBag.Answer = bindData.Answer;
            ViewBag.Answerer = bindData.Answerer;
            ViewBag.ByEmail = true;
            ViewBag.IsAgree = false;

            ViewBag.FormStatus = "error";
            ViewBag.FormMsg = errorMsg.ToString();

            return View(model);
        }
    }
}
