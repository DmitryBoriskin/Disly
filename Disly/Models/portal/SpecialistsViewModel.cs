using cms.dbModel.entity;
using cms.dbModel.entity.cms;

namespace Disly.Models
{
    /// <summary>
    /// Модель для представления главных специалистов во внешней части
    /// </summary>
    public class SpecialistsViewModel : PageViewModel
    {
        /// <summary>
        /// Список главных специалистов
        /// </summary>
        public MainSpecialistModel[] List { get; set; }

        /// <summary>
        /// Список врачей из главных специалистов
        /// </summary>
        public People[] Members { get; set; }

    }
}
