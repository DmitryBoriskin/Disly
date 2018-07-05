using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
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
        /// Год
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Месяц
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// День
        /// </summary>
        public int Day { get; set; }

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
        /// Прикрепленные документы
        /// </summary>
        public DocumentsModel[] Documents { get; set; }

        /// <summary>
        /// Флаг важности
        /// </summary>
        [Required]
        public bool Important { get; set; }

        /// <summary>
        /// Кол-во просмотров
        /// </summary>
        public int CountSee { get; set; }

        /// <summary>
        /// Флаг запрещённости отображения во внешней части
        /// </summary>
        [Required]
        public bool Disabled { get; set; }

        /// <summary>
        /// ссылка на организацию/событие/персону по умолчанию
        /// </summary>
        public Guid ContentLink { get; set; }

        /// <summary>
        /// Тип (организация/событие/персона) по умолчанию
        /// </summary>
        public string ContentLinkType { get; set; }

        /// <summary>
        /// Отключено для редактирования администратором портала, даже тем кто создал
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Признак импортированности материала из рсс ленты
        /// </summary>
        public bool ImportRss { get; set; }
        /// <summary>
        /// Сми о медицине(мира/России/Чувашии)
        /// </summary>
        public string SmiType { get; set; }
  
        //-------------------------------------------------------------
        /// <summary>
        /// Группа
        /// </summary>
        //public SelectListItem[] GroupsId { get; set; }
        public Guid[] GroupsId { get; set; }
        /// <summary>
        /// Группы
        /// </summary>
        public MaterialsGroup[] Groups { get; set; }

        /// <summary>
        /// Связь с другими объектами/сущностями
        /// </summary>
        public ObjectLinks Links { get; set; }

        /// <summary>
        /// Название группы (для модуля внешней части)
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Флаг привязки к главному порталу
        /// </summary>
        public bool IsAttacheToMainPortal { get; set; }
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
        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }
    }

    /// <summary>
    /// Модель, описывающая новость для модуля главной страницы
    /// </summary>
    public class MaterialFrontModule
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string GroupName { get; set; }
        public string SmiType { get; set; }

        /// <summary>
        /// Алиас группы
        /// </summary>
        public string GroupAlias { get; set; }

        /// <summary>
        /// Картинка к новости
        /// </summary>
        public string Photo { get; set; }
    }





    public class RssChannel
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public string RssLink { get; set; }
    }

    public class RssImportModel
    {
        public string title { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public string language { get; set; }
        public string copyright { get; set; }
        public DateTime lastBuildDate { get; set; }
        public string yandex_logo { get; set; }
        public List<RssItem> items { get; set; }
    }
    public class RssItem
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public DateTime pubDate { get; set; }
        public string yandex_full_text { get; set; }
        public string description { get; set; }
        public string yandex_genre { get; set; }
        public string category { get; set; }
        public string enclosure { get; set; }
        public bool New { get; set; }
        public string RssGuid { get; set; }
    }
}
