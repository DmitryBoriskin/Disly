using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class SiteMapViewModel:CoreViewModel
    {
        public cmsSiteMapModel[] List { get; set; }
        public cmsSiteMapModel Item { get; set; }
        public cmsSiteMapTypeModel[] MapType { get; set; }
        public cmsSiteMapViewsModel[] MapView { get; set; }        
    }
}