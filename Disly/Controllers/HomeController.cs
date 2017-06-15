using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class HomeController : RootController
    {
        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "Главная страница";
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

