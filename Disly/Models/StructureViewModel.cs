using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class StructureViewModel : PageViewModel
    {
        public StructureModel[] Structures{get;set;}
        public StructureModel StructureItem { get; set; }

        public Departments[] DepartmentList { get; set; }
        public Departments DepartmentItem { get; set; }
    }
}
