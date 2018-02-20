using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class StatisticViewModel : CoreViewModel
    {
        public List<StatisticMaterial> StatMaterialsOrg{ get; set; }
        public List<StatisticMaterial> StatMaterialsGs { get; set; }
        public List<StatisticMaterial> StatMaterialsEvent { get; set; }
        public List<StatisticFeedBack> StatFeedBack { get; set; }
    }
}