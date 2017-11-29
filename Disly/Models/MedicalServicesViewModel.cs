using cms.dbModel.entity;

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
    }
}
