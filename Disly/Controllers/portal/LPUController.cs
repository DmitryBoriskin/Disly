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

            model = new LPUViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArrayLayout = bannerArrayLayout,
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion

        }

        // GET: LPU
        public ActionResult Index(string tab, Guid? id)
        {
            #region currentPage
            currentPage = _repository.getSiteMap("LPU");
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
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Медицинские услуги", Alias = "services" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "По типу медицинского учреждения", Alias = "typelist" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "По ведомственной принадлежности", Alias = "affiliation" });
            model.Nav.Add(new PageTabsViewModel { Page = page, Title = "Все" });


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
                    {
                        model.OrgTypes = _repository.getOrgTypes();

                        int count_1_column = Convert.ToInt32(model.OrgTypes.Length / 2);
                        ViewBag.Column1 = model.OrgTypes.Skip(0).Take(count_1_column);
                        ViewBag.Column2 = model.OrgTypes.Skip(count_1_column);
                    }

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
                    {
                        model.DepartmentAffiliations = _repository.getDepartmentAffiliations();

                        int count_1_column = Convert.ToInt32(model.DepartmentAffiliations.Length / 2);
                        ViewBag.Column1 = model.DepartmentAffiliations.Skip(0).Take(count_1_column);
                        ViewBag.Column2 = model.DepartmentAffiliations.Skip(count_1_column);
                    }

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
                    {
                        model.MedicalServices = _repository.getMedicalServices(null);

                        int count_1_column = Convert.ToInt32(model.MedicalServices.Length / 2);
                        ViewBag.Column1 = model.MedicalServices.Skip(0).Take(count_1_column);
                        ViewBag.Column2 = model.MedicalServices.Skip(count_1_column);

                    }                      

                    break;
                default:
                    if (id.HasValue)
                    {
                        // список организаций
                        model.OrgList = _repository.getOrgModels(id.Value);
                        ViewBag.TypeTitle = _repository.getOrgTypeName(id.Value);
                    }
                    else
                    {
                        model.OrgList = _repository.getOrgsModel(tab);
                    }
                    break;
            }

            return View(model);
        }
    }
}