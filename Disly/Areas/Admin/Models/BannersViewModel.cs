using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class BannersViewModel : CoreViewModel
    {
        public cmsBannersModel[] List { get; set; }
        public cmsBannersModel Item { get; set; }
    }
}