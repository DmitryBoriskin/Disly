using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class LPUController : RootController
    {
        private LPUViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new LPUViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            string PageTitle = "ЛПУ";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
        }

        // GET: LPU
        public ActionResult Index()
        {
            model.OrgTypes = _repository.getOrgTypes();
            return View(model);
        }

        // GET: /lpu/list/{id}
        public ActionResult List(Guid id)
        {
            // хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "ЛПУ",
                Url = "/lpu"
            });

            // список организаций
            model.OrgList = _repository.getOrgModels(id);

            // название типа организаций
            ViewBag.TypeTitle = _repository.getOrgTypeName(id);
            return View(model);
        }
    }
}