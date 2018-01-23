using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая новости в представлении
    /// </summary>
    public class MaterialsViewModel : CoreViewModel
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public MaterialsList List { get; set; }

        /// <summary>
        /// Конкретная запись новости
        /// </summary>
        public MaterialsModel Item { get; set; }


        /// <summary>
        /// Список организаций, разбитых на группы
        /// </summary>
        public List<OrgType> OrgsByType { get; set; }

        /// <summary>
        /// rss канал
        /// </summary>
        public RssItem[] RssObject { get; set; }        
        public RssChannel RssChannel { get; set; }
        public RssChannel[] RssChannelList { get; set; }
    }
}