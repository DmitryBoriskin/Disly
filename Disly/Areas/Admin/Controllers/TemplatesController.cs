using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class TemplatesController : CoreController
    {

        //public ActionResult AdminMenu(string viewName = "Templates/Menu/Default")
        //{
        //    cmsMenuModel[] Menu = _cmsRepository.getCmsMenu(AccountInfo.Id);

        //    return View(viewName, Menu);
        //}

        public ActionResult Filtr(string Title, string Alias, string Icon, string Url, Catalog_list[] Items, string BtnName = "Добавить", string viewName = "Templates/Filtr/Default", bool readOnly = true)
        {
            string Link = Request.Url.PathAndQuery.ToLower();
            string nowValue = Request.QueryString[Alias];


            //for (int i = 0; i < Items.Length; i++)
            //{
            //    Items[i].link = addFiltrParam(Link, Alias.ToLower(), Items[i].value.ToLower());
            //    Items[i].url = Url.ToLower() + Items[i].value.ToLower() + "/";
            //    Items[i].selected = (nowValue == Items[i].value.ToLower()) ? "now" : String.Empty;
            //}

            if (Items != null && Items.Count() > 0)
            {
                foreach (var item in Items)
                {
                    item.link = addFiltrParam(Link, Alias.ToLower(), item.value.ToLower());
                    item.url = Url.ToLower() + item.value.ToLower() + "/";
                    item.selected = (nowValue == item.value.ToLower()) ? "now" : String.Empty;
                }
            }

            Link = addFiltrParam(Link, Alias.ToLower(), "");

            FiltrModel Model = new FiltrModel()
            {
                Title = Title,
                Alias = Alias,
                Icon = Icon,
                BtnName = BtnName,
                Link = Link,
                Url = Url.ToLower(),
                Items = Items,
                AccountGroup = AccountInfo.Group,
                ReadOnly = readOnly
            };

            return View(viewName, Model);
        }

        //!!! Заменено на partial("Shared/Partial/Pager")
        //public ActionResult Pager(Pager Model, string startUrl, string viewName = "Templates/Pager/Default")
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
        //        Select(s => new PagerModel
        //        {
        //            text = (s == "*") ? "..." : s,
        //            url = (s == "*") ? String.Empty : addFiltrParam(startUrl, "page", s),
        //            isChecked = (s == Model.Page.ToString())
        //        }).ToArray();

        //    if (viewModel.Length < 2) viewModel = null;

        //    return View(viewName, viewModel);
        //}
    }
}