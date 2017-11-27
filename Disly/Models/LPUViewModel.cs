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
    }
}
