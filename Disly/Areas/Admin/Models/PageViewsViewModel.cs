using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class PageViewsViewModel : CoreViewModel
    {
        public cmsPageViewsModel[] List { get; set; }
        public cmsPageViewsModel Item { get; set; }
        public string ErrorInfo { get; set; }
    }
}