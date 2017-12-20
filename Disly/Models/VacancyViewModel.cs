using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для вакнций
    /// </summary>
    public class VacancyViewModel : PageViewModel
    {
        public VacanciesList List { get; set; }
        public VacancyModel Item { get; set; }        
    }
}
