using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Models
{
    /// <summary>
    /// Модель для представления медицинских услуг
    /// </summary>
    public class MedicalServicesViewModel : PageViewModel
    {
        /// <summary>
        /// тип информации, показываемой на странице 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Медицинские услуги
        /// </summary>
        public MedServiceModel[] MedicalServices { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public OrgsModel[] OrgList { get; set; }
    }
}
