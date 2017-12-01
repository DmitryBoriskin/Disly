using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
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

        public ActionResult FeedbackForm()
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
            ViewBag.ByEmail = true;
            ViewBag.IsAgree = false;
            #endregion

            return View(_ViewName, model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "send-btn")]
        public ActionResult FeedbackSend(FeedbackFormViewModel bindData)
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
            ViewBag.ByEmail = true;
            #endregion

            if (ModelState.IsValid)
            {
                var newMessage = new FeedbackModel()
                {
                    Id = Guid.NewGuid(),
                    IsNew = true,
                    Disabled = true,
                    Date = DateTime.Now,
                    SenderName = bindData.SenderName,
                    SenderEmail = bindData.SenderEmail,
                    Title = bindData.Theme,
                    Text = bindData.Text
                };
                var res = _repository.insertFeedbackItem(newMessage);
                if (res)
                    ViewBag.FormStatus = "send";
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

    }
}
