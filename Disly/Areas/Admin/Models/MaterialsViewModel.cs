using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class MaterialsViewModel : CoreViewModel
    {
        public MaterialsList List { get; set; }
        public MaterialsModel Item { get; set; }

        public Catalog_list[] GroupList { get; set; }
    }
}