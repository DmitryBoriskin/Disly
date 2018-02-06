using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class FiltrModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Название кнопки 
        /// </summary>
        public string BtnName { get; set; }
        /// <summary>
        /// Ссылка с фильтром
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Сссылка на все
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Список групп
        /// </summary>
        public Catalog_list[] Items { get; set; }
        /// <summary>
        /// Только разработчик может добавить новую группу
        /// </summary>
        public string AccountGroup { get; set; }
        /// <summary>
        /// Флаг, отображающий является ли фильтр редактируемым
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}