using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class LPUController : RootController
    {
        private LPUViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            currentPage = _repository.getSiteMap("LPU");

            if (currentPage == null)
                throw new Exception("model.CurrentPage == null");

            model = new LPUViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
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
        }

        // GET: LPU
        public ActionResult Index(string tab, Guid? id)
        {
            model.Type = tab;
            var page = model.CurrentPage.FrontSection;

            //Хлебные крошки
            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = model.CurrentPage.Title,
                Url = string.Format("/{0}/", page) // ""
            });

            //Табы на странице
            model.Nav = new List<PageTabsViewModel>();
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Все" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "По типу медицинского учреждения", Alias = "typelist" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "По ведомственной принадлежности", Alias = "affiliation" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Медицинские услуги", Alias = "services" });

            //Обработка активных табов
            if (model.Nav != null && model.Nav.Where(s => s.Alias == tab).Any())
            {
                var navItem = model.Nav.Where(s => s.Alias == tab).Single();
                navItem.Active = true;

                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = navItem.Title,
                    Url = !string.IsNullOrEmpty(navItem.Alias)? "?tab=" + navItem.Alias : ""
                });
            }

            switch (tab)
            {
                case "typelist":
                    if (id.HasValue)
                    {
                        // список организаций
                        model.OrgList = _repository.getOrgModels(id.Value);

                        // название типа организаций
                        ViewBag.TypeTitle = _repository.getOrgTypeName(id.Value);

                        model.Breadcrumbs.Add(new Breadcrumbs
                        {
                            Title = ViewBag.TypeTitle,
                            Url = ""
                        });
                    }
                    else
                        model.OrgTypes = _repository.getOrgTypes();

                    break;
                case "affiliation":
                    if (id.HasValue)
                    {
                        model.OrgList = _repository.getOrgModels(null)
                            .Where(w => w.Affiliation.Equals(id)).ToArray();

                        ViewBag.TypeTitle = _repository.getAffiliationDepartment(id.Value);

                        model.Breadcrumbs.Add(new Breadcrumbs
                        {
                            Title = ViewBag.TypeTitle,
                            Url = ""
                        });
                    }
                    else
                        model.DepartmentAffiliations = _repository.getDepartmentAffiliations();

                    break;
                case "services":
                    if (id.HasValue)
                    {
                        ViewBag.TypeTitle = _repository.getMedicalServiceTitle(id.Value);

                        model.Breadcrumbs.Add(new Breadcrumbs
                        {
                            Title = ViewBag.TypeTitle,
                            Url = ""
                        });
                        model.OrgList = _repository.getOrgPortalModels(id.Value);
                    }
                    else
                        model.MedicalServices = _repository.getMedicalServices(null);

                    break;
                default:
                    if (id.HasValue)
                    {
                        // список организаций
                        model.OrgList = _repository.getOrgModels(id.Value);

                        ViewBag.TypeTitle = _repository.getOrgTypeName(id.Value);
                    }
                    else
                        model.OrgList = _repository.getOrgModels(null);

                    break;
            }

            return View(model);
        }
    }
}