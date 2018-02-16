using cms.dbModel.entity;
using cms.dbModel.entity.cms;

namespace Disly.Models
{
    /// <summary>
    /// Модель докторов для типовой страницы 
    /// </summary>
    public class DoctorsViewModel : PageViewModel
    {
        /// <summary>
        /// Список докторов
        /// </summary>
        public DoctorList DoctorsList { get; set; }

        /// <summary>
        /// Единичная запись доктора
        /// </summary>
        public People DoctorsItem { get; set; }

        /// <summary>
        /// Список структур
        /// </summary>
        public StructureModel[] DepartmentsSelectList { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public EmployeePost[] PeoplePosts { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// Врачи для регистрации
        /// </summary>
        public Doctor[] DoctorsRegistry { get; set; }
    }
}
