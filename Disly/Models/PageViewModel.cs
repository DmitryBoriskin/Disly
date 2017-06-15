using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cms.dbModel.entity;

namespace Disly.Models
{
    public class PageViewModel
    {
        public ErrorViewModel ErrorInfo { get; set; }
        public SitesModel[] SitesInfo { get; set; }
    }

    public class ErrorViewModel
    {
        public Int32? HttpCode { get; set; }
        public String Title { get; set; }
        public String Message { get; set; }
        public String BackUrl { get; set; }
    }
}