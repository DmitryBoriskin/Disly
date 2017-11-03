using System;
using System.ComponentModel.DataAnnotations;

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
    /// Группа новости
    /// </summary>
    public class MaterialGroup
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// Модель, описывающая список новостей
    /// </summary>
    public class MaterialsList
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public MaterialsModel[] Data { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// Модель, описывающая новости пресс-центра
    /// </summary>
    public class MaterialsModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Поле «Заголовок» не должно быть пустым.")]
        public string Title { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Изображнение
        /// </summary>
        public Photo PreviewImage { get; set; }

        /// <summary>
        /// Ссылка 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название ссылки
        /// </summary>
        public string UrlName { get; set; }

        /// <summary>
        /// Текст новости
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        [Required]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Ключевые слова
        /// </summary>
        public string Keyw { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Флаг важности
        /// </summary>
        [Required]
        public bool Important { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        [Required]
        public bool Disabled { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        //public SelectListItem[] GroupsId { get; set; }
        public Guid[] GroupsId { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public MaterialGroup[] Groups { get; set; }

        /// <summary>
        /// ссылка на организацию/событие/персону по умолчанию
        /// </summary>
        public Guid ContentLink { get; set; }

        /// <summary>
        /// Тип (организация/событие/персона) по умолчанию
        /// </summary>
        public string ContentLinkType { get; set; }

        /// <summary>
        /// Событие
        /// </summary>
        public Guid? Event { get; set; }
    }

    /// <summary>
    /// Модель, описывающая группы новостей
    /// </summary>
    public class MaterialsGroup
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }
    }

    /// <summary>
    /// Модель, описывающая события для привязки к новостям
    /// </summary>
    public class MaterialsEvents
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
    }
}
