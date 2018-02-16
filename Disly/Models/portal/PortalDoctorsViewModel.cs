using cms.dbModel.entity;
using cms.dbModel.entity.cms;

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
        public DoctorList DoctorsList { get; set; }

        /// <summary>
        /// Единичная запись доктора
        /// </summary>
        public People DoctorsItem { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public EmployeePost[] PeoplePosts { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }
    }
}
