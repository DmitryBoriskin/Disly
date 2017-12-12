using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
 {
     public class RedirectFromOldController : CoreController
     {
    
         protected override void OnActionExecuting(ActionExecutingContext filterContext)
         {
             base.OnActionExecuting(filterContext);
 
             ViewBag.ControllerName = (String) RouteData.Values["controller"];
             ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();
 
             ViewBag.HttpKeys = Request.QueryString.AllKeys;
             ViewBag.Query = Request.QueryString;
 
             #region Метатеги
             ViewBag.Title = UserResolutionInfo.Title;
             ViewBag.Description = "";
             ViewBag.KeyWords = "";
             #endregion
         }
 
 
         /// <summary>
         /// GET: Список событий
         /// </summary>
         /// <param name="category"></param>
         /// <param name="type"></param>
         /// <returns></returns>
         public ActionResult Index()
         {
            var url = "url";
            var page = "news";
            switch(page)
            {
                case "page":
                    break;
                case "news":
                    break;
                case "doctorsnew":
                    break;
                case "doctors":
                    break;
                case "hospitals":
                    //"http://www.med.cap.ru/Hospitals.aspx?mtype=10&type=0"
                    break;
                case "services":
                    //Services.aspx? mtyperun = 20
                    break;
                case "vacancies":
                    break;
                case "anons":
                    break;

            }
            //RedirectToAction("", "");

            return Redirect(url);
        }

    }
 }