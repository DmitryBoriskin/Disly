using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class StructureController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        private StructureViewModel model;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new StructureViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                Breadcrumbs = breadcrumb,
                BannerArray = bannerArray,
                CurrentPage = currentPage
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Список струкутур
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region currentPage
            currentPage = _repository.getSiteMap("Structure");
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

            model.Structures = _repository.getStructureList(); //Domain
            //если в списке только одна структура — редиректим на него
            if (model.Structures != null && model.Structures.Count() > 0 && model.Structures.Count()==1)
                return Redirect(ControllerName + "/" + model.Structures[0].Num);


            return View(_ViewName, model);
        }
        /// <summary>
        /// отдельная структура+списиок отделений
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public ActionResult Item(int num)
        {
            if (num == 0)
                return Redirect(ControllerName + "/");

            #region currentPage
            currentPage = _repository.getSiteMap("Structure");
            if (currentPage == null)
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

            model.StructureItem = _repository.getStructureItem(num); //Domain, 
            if (model.StructureItem != null)
            {
                if (model.Breadcrumbs != null)
                {
                    model.Breadcrumbs.Add(new Breadcrumbs()
                    {
                        Title = model.StructureItem.Title
                    });
                }
                if (model.StructureItem.Ovp)
                {
                    model.DepartmentItem = _repository.getOvpDepartaments(model.StructureItem.Id);
                }
                else
                {
                    model.DepartmentList = _repository.getDepartmentsList(model.StructureItem.Id);
                }
            }

            return View(_ViewName, model);
        }
        /// <summary>
        /// отдельное отделелние
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Department(int num, Guid id)
        {
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            model.StructureItem = _repository.getStructureItem(num); //Domain,
            if (model.StructureItem != null)
            {
                if (model.Breadcrumbs != null)
                {
                    model.Breadcrumbs.Add(new Breadcrumbs()
                    {
                        Title = model.StructureItem.Title,
                        Url = "/" + ControllerName + "/" + num
                    });
                }
            }
            model.DepartmentItem = _repository.getDepartmentsItem(id);
            if (model.Breadcrumbs != null)
            {
                model.Breadcrumbs.Add(new Breadcrumbs()
                {
                    Title = model.DepartmentItem.Title
                });

            }

            ViewBag.Title = (model.DepartmentItem != null) ? model.DepartmentItem.Title : "Отделение";

            return View(_ViewName, model);
        }

    }
}
