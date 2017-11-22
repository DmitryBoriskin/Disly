using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая опросы
    /// </summary>
    public class VoteViewModel : CoreViewModel
    {
        /// <summary>
        /// Список опрсов
        /// </summary>
        public VoteList List { get; set; }

        /// <summary>
        /// Конкретный опросы
        /// </summary>
        public VoteModel Item { get; set; }
        
    }
}