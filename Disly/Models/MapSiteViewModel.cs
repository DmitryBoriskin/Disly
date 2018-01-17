using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для внешней части карты сайта
    /// </summary>
    public class SiteMapViewModel : PageViewModel
    {
        /// <summary>
        /// Список элементов карты сайта
        /// </summary>
        public SiteMapModel[] List { get; set; }
    }
}
