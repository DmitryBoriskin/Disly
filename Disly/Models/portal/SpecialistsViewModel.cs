using cms.dbModel.entity;

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
        public GSModel[] List { get; set; }

        /// <summary>
        /// Список врачей из главных специалистов
        /// </summary>
        public PeopleModel[] Members { get; set; }

    }
}
