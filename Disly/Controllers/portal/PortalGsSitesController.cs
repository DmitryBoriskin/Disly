using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PortalGsSitesController : RootController
    {
        private SpecialistsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SpecialistsViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>(),
                CurrentPage = currentPage
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: GeneralSpecialists
        public ActionResult Index()
        {
            #region currentPage
            currentPage = _repository.getSiteMap("PortalGsSites");
            if (currentPage == null)
                //throw new Exception("model.CurrentPage == null");
                return RedirectToRoute("Error", new { httpCode = 404 });

            if (currentPage != null)
            {
                ViewBag.Title = currentPage.Title;
                ViewBag.Description = currentPage.Desc;
                ViewBag.KeyWords = currentPage.Keyw;

                model.CurrentPage = currentPage;
            }
            #endregion

            //if ((model.SitesInfo == null) || (model.SitesInfo != null && model.SitesInfo.Type != ContentLinkType.ORG.ToString().ToLower()))
            //    return RedirectToRoute("Error", new { httpCode = 405 });

            string _ViewName = (ViewName != string.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = ViewBag.Title,
                Url = ""
            });

            //Список объектов "Главный специалист"
            var filter = getFilter();
            filter.Domain = null;
            model.List = _repository.getGSList(filter);

            if(model.List != null && model.List.Count() > 0)
            {
                foreach(var item in model.List)
                {
                    if (!string.IsNullOrEmpty(item.Domain))
                    {
                        var siteInfo = _repository.getSiteInfo(item.Domain);

                        item.SiteImgUrl = (siteInfo != null && siteInfo.Logo != null) ? siteInfo.Logo.Url : "";
                    }
                }
            }
            return View(model);
        }
    }
}