using System;

namespace cms.dbModel.entity
{
    /// <summary>
    ///  сайт организации, события или персоны
    /// </summary>
    public class SiteContentType
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? Id;

        /// <summary>
        /// тип
        /// </summary>
        public string CType;
    }

    /// <summary>
    /// Хлебные крошки
    /// </summary>
    public class Breadcrumbs
    {
        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Модель справочника
    /// Используется для построения фильтров, категорий 
    /// и наполнения полей с выпадающими списками
    /// </summary>
    public class Catalog_list
    {
        /// <summary>
        /// Заголовок группы
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Значение 
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Иконка
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Ссылка, для применения фильтра
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Ссылка на редактирование группы
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Выбрано
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// Доступно для выбора пользователем
        /// </summary>
        public bool Available { get; set; }
    }
}
