using cms.dbase;
using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace Disly.Controllers
{
    public class ServiceController : Controller
    {

        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected FrontRepository _repository { get; private set; }

        //!! Заменено на Shared/Part/Pager
        //public ActionResult Pager(Pager Model, string startUrl, string viewName = "Services/Pager")
        //{
        //    ViewBag.PagerSize = string.IsNullOrEmpty(Request.QueryString["size"]) ? Model.Size.ToString() : Request.QueryString["size"];
        //    string qwer = String.Empty;

        //    int PagerLinkSize = 2;

        //    int FPage = (Model.Page - PagerLinkSize < 1) ? 1 : Model.Page - PagerLinkSize;
        //    int LPage = (Model.Page + PagerLinkSize > Model.PageCount) ? Model.PageCount : Model.Page + PagerLinkSize;

        //    if (String.IsNullOrEmpty(startUrl)) startUrl = Request.Url.Query;

        //    if (FPage > 1)
        //    {
        //        qwer = qwer + "1,";
        //    }
        //    if (FPage > 2)
        //    {
        //        qwer = qwer + "*,";
        //    }
        //    for (int i = FPage; i < LPage + 1; i++)
        //    {
        //        qwer = (@i < Model.PageCount) ? qwer + @i + "," : qwer + @i;
        //    }
        //    if (LPage < Model.PageCount - 1)
        //    {
        //        qwer = qwer + "*,";
        //    }
        //    if (Model.PageCount > LPage)
        //    {
        //        qwer = qwer + @Model.PageCount;
        //    }


        //    var viewModel = qwer.Split(',').
        //        Where(w => w != String.Empty).
        //        Select(s => new PagerFront
        //        {
        //            text = (s == "*") ? "..." : s,
        //            url = (s == "*") ? String.Empty : addFiltrParam(startUrl, "page", s),
        //            isChecked = (s == Model.Page.ToString())
        //        }).ToArray();

        //    if (viewModel.Length < 2) viewModel = null;

        //    return View(viewName, viewModel);
        //}


        public string addFiltrParam(string query, string name, string val)
        {
            //string search_Param = @"\b" + name + @"=[\w]*[\b]*&?";
            string search_Param = @"\b" + name + @"=(.*?)(&|$)";
            string normal_Query = @"&$";

            Regex delParam = new Regex(search_Param, RegexOptions.CultureInvariant);
            Regex normalQuery = new Regex(normal_Query);
            query = delParam.Replace(query, String.Empty);
            query = normalQuery.Replace(query, String.Empty);

            if (val != String.Empty)
            {
                if (query.IndexOf("?") > -1) query += "&" + name + "=" + val;
                else query += "?" + name + "=" + val;
            }

            query = query.Replace("?&", "?").Replace("&&", "&");

            return query;
        }


        public ActionResult Photolist(Guid id)
        {
            var domainUrl = "";

            if (System.Web.HttpContext.Current != null)
            {
                var context = System.Web.HttpContext.Current;

                if (context.Request != null && context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Host))
                    domainUrl = context.Request.Url.Host.ToLower().Replace("www.", "");
            }

            _repository = new FrontRepository("cmsdbConnection", domainUrl);

            PhotoModel[] model = _repository.getPhotoList(id);
            return View("/Views/Service/Photo.cshtml", model);
        }

    }
}