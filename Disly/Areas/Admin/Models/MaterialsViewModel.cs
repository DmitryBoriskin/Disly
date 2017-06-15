using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class MaterialsViewModel : CoreViewModel
    {
        public cmsMaterialsModel[] List { get; set; }
        public cmsMaterialsModel Item { get; set; }
        public string ErrorInfo { get; set; }
        public string SearchLine { get; set; }
        public Nullable<DateTime> Begin { get; set; }
        public Nullable<DateTime> End { get; set; }
    }
}