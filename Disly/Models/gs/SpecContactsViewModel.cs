using cms.dbModel.entity;
using cms.dbModel.entity.cms;
using System.Collections.Generic;

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
        public GSModel MainSpec { get; set; }
        /// <summary>
        /// Список докторов главных специалистов
        /// </summary>
        public People[] SpesialitsList { get; set; }
    }
}
