using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class SitesViewModel : CoreViewModel
    {
        public SitesList List { get; set; }
        public SitesModel Item { get; set; }
    }
}
