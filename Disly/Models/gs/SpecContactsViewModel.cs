using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы специалисты
    /// </summary>
    public class SpecContactsViewModel : PageViewModel
    {
        /// <summary>
        /// Общая информация о модели
        /// </summary>
        public GSModel GS { get; set; }
        /// <summary>
        /// Список докторов главных специалистов
        /// </summary>
        public GSMemberModel[] SpesialitsList { get; set; }
    }
}
