using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class VoteController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        public string _ip = RequestUserInfo.IP;
        private VoteViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new VoteViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = _repository.getSiteMap("vote")
            };

            model.Breadcrumbs = new List<Breadcrumbs>();
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "Обратная связь",
                Url = "/feedback"
            });
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region Получаем данные из адресной строки
            string UrlPath = "/" + (String)RouteData.Values["path"];
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "Голосование",
                Url = ""
            });

            model.List = _repository.getVote(_ip); //Domain,
            model.Siblings = (model.CurrentPage != null) ? _repository.getSiteMapSiblingElements("/feedback/") : null;

            if(model.Siblings != null && !model.Siblings.Select(p=>p.Id).Contains(model.CurrentPage.Id))
                model.Siblings.Add(model.CurrentPage);

            model.Child = model.Siblings.ToArray();

            #region Создаем переменные (значения по умолчанию)            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Опросы";
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

        [HttpPost]
        public ActionResult GiveVote(Guid id)
        {
            string answerId = Request["r-" + id.ToString()];
            String[] answers = answerId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            _repository.GiveVote(id, answers, _ip);

            return Redirect("/vote");
        }

        public ActionResult Item(Guid id)
        {
            #region Получаем данные из адресной строки
            string UrlPath = "/" + (String)RouteData.Values["path"];
            if (UrlPath.LastIndexOf("/") > 0 && UrlPath.LastIndexOf("/") == UrlPath.Length - 1) UrlPath = UrlPath.Substring(0, UrlPath.Length - 1);

            string _path = UrlPath.Substring(0, UrlPath.LastIndexOf("/") + 1);
            string _alias = UrlPath.Substring(UrlPath.LastIndexOf("/") + 1);
            #endregion

            model.Item = _repository.getVoteItem(id, _ip);

            #region Создаем переменные (значения по умолчанию)            
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Опросы";
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
    }
}

