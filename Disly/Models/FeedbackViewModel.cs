using cms.dbModel.entity;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class FeedbackViewModel : PageViewModel
    {
        public FeedbacksList List { get; set; }
        public FeedbackModel Item { get; set; }

        /// <summary>
        /// Навигация
        /// </summary>
        public MaterialsGroup[] Nav { get; set; }
        /// <summary>
        /// Доболнительная информация из библиотеки
        /// </summary>
        public SiteMapModel DopInfo { get; set; }
    }
}
