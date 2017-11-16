using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая новости в представлении
    /// </summary>
    public class SiteSectionViewModel : CoreViewModel
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public SiteSectionList List { get; set; }
        public SiteSectionModel Item { get; set; }
    }
}