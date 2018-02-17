using cms.dbModel.entity;

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
        public PeopleList DoctorsList { get; set; }

        /// <summary>
        /// Единичная запись доктора
        /// </summary>
        public PeopleModel DoctorsItem { get; set; }

        /// <summary>
        /// Список структур
        /// </summary>
        public StructureModel[] DepartmentsSelectList { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public Specialisation[] PeoplePosts { get; set; }

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
