using cms.dbase.Repository;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace Disly.Controllers
{
    public class SpecContactsController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private SpecContactsViewModel model;
        private OnlineRegistryRepository repoRegistry = new OnlineRegistryRepository("registryConnection");

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SpecContactsViewModel()
            {
                SitesInfo = siteModel,
                MainMenu= mainMenu,
                BannerArrayLayout = bannerArrayLayout,
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Струтура";
            ViewBag.Description = "Струтура сайта главного специалиста";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string tab)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("SpecContacts");
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

            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            var page = model.CurrentPage.FrontSection;

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = ""
            });

            //Получаем модель "главный специалист" (в нее может входить несколько врачей)
            var filter = getFilter();
            filter.Domain = null;
            var pfilter = FilterParams.Extend<PeopleFilter>(filter);
            var gs = _repository.getGSItem(model.SitesInfo.ContentId);
            if (gs != null)
            {
                model.GS = gs;

                //Получение списков врачей, относящихся к гс по типам
                model.SpesialitsList = _repository.getGSMembers(gs.Id, GSMemberType.SPEC);
                if (model.SpesialitsList != null && model.SpesialitsList.Count() > 0)
                {
                    foreach (var item in model.SpesialitsList)
                    {
                        if (item.People != null)
                        {
                            var fltr = new OrgFilter()
                            {
                                PeopleId = item.People.Id
                            };
                            item.Orgs = _repository.getGsMemberContacts(item.Id);
                        }
                    }
                }
            }

            return View(_ViewName, model);
        }
    }
}

