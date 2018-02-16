using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель, докторов портала
    /// </summary>
    public class PortalDoctorsViewModel : PageViewModel
    {
        /// <summary>
        /// Постраничный список докторов
        /// </summary>
        public PeopleList DoctorsList { get; set; }

        /// <summary>
        /// Единичная запись доктора
        /// </summary>
        public PeopleModel DoctorsItem { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public Specialisation[] PeoplePosts { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }
    }
}
