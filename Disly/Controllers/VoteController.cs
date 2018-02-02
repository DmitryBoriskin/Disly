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
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
        };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion

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
            #region currentPage
            currentPage = _repository.getSiteMap("Vote");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = ViewBag.Title,
                Url = ""
            });

            model.List = _repository.getVote(_ip); //Domain,
            model.Siblings = (model.CurrentPage != null) ? _repository.getSiteMapSiblingElements("/feedback/") : null;

            if (model.Siblings != null && !model.Siblings.Select(p => p.Id).Contains(model.CurrentPage.Id))
                model.Siblings.Add(model.CurrentPage);

            model.Child = model.Siblings.ToArray();

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
            #region currentPage
            currentPage = _repository.getSiteMap("Vote");
            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.Item = _repository.getVoteItem(id, _ip);

            return View(_ViewName, model);
        }
    }
}
