namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель справочника
    /// Используется для построения фильтров, категорий 
    /// и наполнения полей с выпадающими списками
    /// </summary>
    public class Catalog_list
    {
        /// <summary>
        /// Заголовок записи
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Значение записи (ключ для связи)
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// Иконка (иллюстрация)
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// Ссылка, для применения фильтра
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// Адрес формы для редактирования данной записи
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string selected { get; set; }
    }
}
