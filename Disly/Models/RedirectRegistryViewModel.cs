using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы редиректа на регистрацию
    /// </summary>
    public class RedirectRegistryViewModel : PageViewModel
    {
        /// <summary>
        /// Список докторов
        /// </summary>
        public Doctor[] Doctors { get; set; }
    }
}
