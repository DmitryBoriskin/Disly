using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class FilterViewModel : CoreViewModel
    {
        public SectionGroupModel[] Sections { get; set; }
        public SectionGroupModel Section { get; set; }
        
        public SectionGroupItemsModel[] SectionItems { get; set; }
        public SectionGroupItemsModel SectionItem { get; set; }
    }
}
