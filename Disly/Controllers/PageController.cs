using cms.dbase;
using Disly.Models;
using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class PageController : RootController
    {
        public const String Name = "Error";
        public const String ActionName_Custom = "Custom";
        
        /// <summary>
        /// Сраница по умолчанию
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
            #region Создаем переменные (значения по умолчанию)
            PageViewModel Model = new PageViewModel();
            string _ViewName = (ViewName != String.Empty) ? ViewName : "~/Views/Error/CustomError.cshtml";

            string PageTitle = "страница сайта";
            string PageDesc = "описание страницы";
            string PageKeyw = "ключевые слова";
            #endregion
            
            //cms.dbModel.entity.pagePathModel[] _PagePath = _repository.getPagePath(_path + _alias, _domain);

            //Model = new PageViewModel() {
            //    PagePath = _PagePath,
            //    PageInfo = _repository.getPageInfo(_PagePath[_PagePath.Length-1].Path, _PagePath[_PagePath.Length-1].Alias, _domain),                
            //    PageMenu = _repository.getPageMenu(_domain),
            //    NewsList = _repository.getMaterials(_domain),                
            //    PageMenuChild = _repository.getPageChildElem(_path + _alias, _domain),
            //    PlaceCardList = _repository.getPlaceCards(_domain),
            //    NewsItem = _repository.getMaterialsItem(_path + _alias, _domain),
            //    //Banners = _repository.getBanners(BANNER_SECTIONS, _domain)

            //    //SitesInfo=_repository.getAllSitesInfo()
            //};

            #region Метатеги
            ViewBag.Title = PageTitle;
            ViewBag.Description = PageDesc;
            ViewBag.KeyWords = PageKeyw;
            #endregion

            return View(_ViewName);

            //if (Model.PageInfo != null)
            //{
            //    if (Model.PageInfo.View != String.Empty) ViewName = Model.PageInfo.View;
            //    ViewBag.Alias = Model.PageInfo.Alias;

            //    PageTitle = Model.PageInfo.Title;
            //    PageDesc = Model.PageInfo.Desc;
            //    PageKeyw = Model.PageInfo.Keyw;

            
            

            //    return View(ViewName, Model);
            //}
            //// если сайт есть, а главная еще не создана
            //else if (_path == "/" && _alias == " " && Model.PageInfo == null)
            //{
            //    return RedirectToAction("InDevelopment", "ErrorController");
            //}
            ////если нет страницы
            //else
            //{
            //    return RedirectToAction("Custom", "ErrorController", new { @httpCode = "404" });                
            //}   
        }
    }
}

