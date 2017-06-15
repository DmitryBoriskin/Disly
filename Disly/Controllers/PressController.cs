using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PressController : RootController
    {
        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Category = (RouteData.Values["category"] != null) ? RouteData.Values["category"] : String.Empty;

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "новости";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion
            
            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName, Model);
        }

        public ActionResult Item(int date, int? day)
        {

            ViewBag.Date = date.ToString();
            ViewBag.Day = day.ToString();
            ViewBag.Alias = (RouteData.Values["alias"] != null) ? RouteData.Values["alias"] : String.Empty;

            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "новости";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion


            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName, Model);
        }
    }
}

