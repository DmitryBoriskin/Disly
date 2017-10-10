using cms.dbModel.entity;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Models
{
    public class SitesViewModel : CoreViewModel
    {
        public SitesList List { get; set; }
        public SitesModel Item { get; set; }

        public SelectList TypeList { get; set; }
        public SelectList OrgsList { get; set; }
        public SelectList EventsList { get; set; }
        public SelectList PeopleList { get; set; }
    }
}
