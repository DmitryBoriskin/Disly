using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class MaterialsViewModel : CoreViewModel
    {
        public MaterialsList List { get; set; }
        public MaterialsModel Item { get; set; }

        public Catalog_list[] GroupList { get; set; }
    }
}