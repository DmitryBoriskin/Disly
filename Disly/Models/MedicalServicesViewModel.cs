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
        /// Медицинские услуги
        /// </summary>
        public MedicalService[] MedicalServices { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public OrgFrontModel[] OrgList { get; set; }

        /// <summary>
        /// тип информации показываемой на странице 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Навигация
        /// </summary>
        public List<MaterialsGroup> Nav { get; set; }

        /// <summary>
        /// Информация
        /// </summary>
        public SiteMapModel Info { get; set; }
    }
}
