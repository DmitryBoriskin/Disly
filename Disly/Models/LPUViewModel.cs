using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Лечебно-профилактическое учреждение
    /// </summary>
    public class LPUViewModel : PageViewModel
    {
        /// <summary>
        /// Типы организаций
        /// </summary>
        public OrgType[] OrgTypes { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public OrgFrontModel[] OrgList { get; set; }

        /// <summary>
        /// Список ведомственных принадлежностей
        /// </summary>
        public DepartmentAffiliationModel[] DepartmentAffiliations { get; set; }

        /// <summary>
        /// Навигация
        /// </summary>
        public MaterialsGroup[] Nav { get; set; }

        /// <summary>
        /// тип информации показываемой на странице 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Медицинские услуги
        /// </summary>
        public MedicalService[] MedicalServices { get; set; }
    }
}
