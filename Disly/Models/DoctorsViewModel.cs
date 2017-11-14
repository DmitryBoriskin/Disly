using cms.dbModel.entity;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class DoctorsViewModel : PageViewModel
    {
        public People[] DoctorsList { get; set; }
        public People DoctorsItem { get; set; }

        public StructureModel[] DepartmentsSelectList { get; set; }
    }
}
