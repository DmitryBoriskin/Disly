using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disly.Areas.Admin.Models
{
    public class SiteModulesViewModel : CoreViewModel
    {
        public SiteModulesModel[] SiteModulesList { get; set; }
        public SiteModulesModel SiteModule { get; set; }
    }
}
