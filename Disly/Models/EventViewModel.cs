using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для представления события во внешней части
    /// </summary>
    public class EventViewModel : PageViewModel
    {
        /// <summary>
        /// Список событий
        /// </summary>
        public EventsList List { get; set; }

        /// <summary>
        /// Единичная запись события
        /// </summary>
        public EventsModel Item { get; set; }
    }
}
