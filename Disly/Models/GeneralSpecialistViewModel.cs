using cms.dbModel.entity;
using cms.dbModel.entity.cms;

namespace Disly.Models
{
    /// <summary>
    /// Модель для представления главных специалистов во внешней части
    /// </summary>
    public class GeneralSpecialistViewModel : PageViewModel
    {
        /// <summary>
        /// Список главных специалистов
        /// </summary>
        public MainSpecialistModel[] List { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public SiteMapModel DopInfo { get; set; }
    }
}
