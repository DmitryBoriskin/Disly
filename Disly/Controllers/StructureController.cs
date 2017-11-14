using cms.dbase;
using cms.dbModel.entity;
using Disly.Models;
using System;
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
                Breadcrumbs=breadcrumb,
                BannerArray = bannerArray
            };
        }

        /// <summary>
        /// Список струкутур
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

            model.Structures = _repository.getStructures(Domain);
            //если в списке только одна структура — редиректим на него
            if (model.Structures.Length ==1)
            {
                return Redirect(ControllerName + "/" + model.Structures[0].Num);
            }

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "Структура";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion                        

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName,model);
        }
        /// <summary>
        /// отдельная структура+списиок отделений
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public ActionResult Item(int num)
        {
            model.StructureItem = _repository.getStructureItem(Domain, num);
            if (model.StructureItem != null)
            {
                model.Breadcrumbs.Add(new Breadcrumbs()
                {
                    Title = model.StructureItem.Title
                });
                model.DepartmentList = _repository.getDepartmentsList(model.StructureItem.Id);
            }
            
            
            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = "страница сайта";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion                        
            #region Метатеги
            ViewBag.Title = model.StructureItem.Title;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion
            return View(_ViewName, model);
        }
        /// <summary>
        /// отдельное отделелние
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Department(int num, Guid id)
        {
            model.StructureItem = _repository.getStructureItem(Domain, num);
            if (model.StructureItem != null)
            {
                model.Breadcrumbs.Add(new Breadcrumbs()
                {
                    Title = model.StructureItem.Title,
                    Url = "/"+ControllerName + "/" + num
                });
            }

            model.DepartmentItem = _repository.getDepartmentsItem(id);
            if (model.DepartmentItem != null)
            {
                model.Breadcrumbs.Add(new Breadcrumbs()
                {
                    Title = model.DepartmentItem.Title
                });

            }


            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";
            string PageTitle = (model.DepartmentItem!=null)?model.DepartmentItem.Title:"Отдление";
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

