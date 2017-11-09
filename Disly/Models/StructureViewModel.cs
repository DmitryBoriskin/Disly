using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
