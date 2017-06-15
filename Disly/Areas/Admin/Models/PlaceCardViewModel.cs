using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class PlaceCardViewModel : CoreViewModel
    {
        public PlaceCardModel[] List { get; set; }
        public PlaceCardModel Item { get; set; }
        public string ErrorInfo { get; set; }
        //public string SearchLine { get; set; }
        //public Nullable<DateTime> Begin { get; set; }
        //public Nullable<DateTime> End { get; set; }
    }
}