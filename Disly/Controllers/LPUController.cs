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
        public ActionResult Index(string t, Guid? id)
        {
            model.Nav = new MaterialsGroup[]
            {
                new MaterialsGroup{Title="Все"},
                new MaterialsGroup{Title="По типу медицинского учреждения", Alias="typelist"},
                new MaterialsGroup{Title="По ведомственной принадлежности", Alias="affiliation"},
            };

            model.Breadcrumbs.Add(new Breadcrumbs
            {
                Title = "ЛПУ",
                Url = "/lpu"
            });

            model.Type = t;
            switch (t)
            {
                case "typelist":
                    model.Breadcrumbs.Add(new Breadcrumbs
                    {
                        Title = "По типу медицинского учреждения",
                        Url = "/lpu?t=typelist"
                    });

                    if (!id.Equals(null))
                    {
                        // список организаций
                        model.OrgList = _repository.getOrgModels(id);

                        // название типа организаций
                        ViewBag.TypeTitle = _repository.getOrgTypeName((Guid)id);

                        model.Breadcrumbs.Add(new Breadcrumbs
                        {
                            Title = ViewBag.TypeTitle,
                            Url = ""
                        });
                    }
                    else
                    {
                        model.OrgTypes = _repository.getOrgTypes();
                    }

                    break;
                case "affiliation":
                    model.Breadcrumbs.Add(new Breadcrumbs
                    {
                        Title = "По ведомственной принадлежности",
                        Url = "/lpu?t=affiliation"
                    });

                    if (id != null)
                    {
                        model.OrgList = _repository.getOrgModels(null)
                            .Where(w => w.Affiliation.Equals(id)).ToArray();

                        ViewBag.TypeTitle = _repository.getAffiliationDepartment((Guid)id);

                        model.Breadcrumbs.Add(new Breadcrumbs
                        {
                            Title = ViewBag.TypeTitle,
                            Url = ""
                        });
                    }
                    else
                    {
                        model.DepartmentAffiliations = _repository.getDepartmentAffiliations();
                    }

                    break;
                default:
                    // хлебные крошки

                    // список организаций
                    model.OrgList = _repository.getOrgModels(id);

                    // название типа организаций
                    if (!id.Equals(null))
                    {
                        ViewBag.TypeTitle = _repository.getOrgTypeName((Guid)id);
                    }

                    break;
            }

            return View(model);
        }
    }
}