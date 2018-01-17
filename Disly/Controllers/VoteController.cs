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

            currentPage = _repository.getSiteMap("Vote");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new VoteViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = currentPage
            };

            #region Создаем переменные (значения по умолчанию)
            string PageTitle = model.CurrentPage.Title;
            string PageDesc = model.CurrentPage.Desc;
            string PageKeyw = model.CurrentPage.Keyw;
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

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
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "Голосование",
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
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.Item = _repository.getVoteItem(id, _ip);

            return View(_ViewName, model);
        }
    }
}
