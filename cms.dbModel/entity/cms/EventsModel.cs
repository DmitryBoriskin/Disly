using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Список событий с пэйджером
    /// </summary>
    public class EventsList
    {
        /// <summary>
        /// Список событий
        /// </summary>
        public EventsModel[] Data;

        /// <summary>
        /// Пэйджер
        /// </summary>
        public Pager Pager;
    }

    /// <summary>
    /// Событие
    /// </summary>
    public class EventsModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Целочисленный идентификатор
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Title { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Место
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Создатель события
        /// </summary>
        public string EventMaker { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название ссылки
        /// </summary>
        public string UrlName { get; set; }

        /// <summary>
        /// Дата начала события
        /// </summary>
        [Display(Name = "Дата начала события")]
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public DateTime DateBegin { get; set; }
        
        /// <summary>
        /// Дата окончания события
        /// </summary>
        public DateTime? DateEnd { get; set; }
        
        /// <summary>
        /// Ежегодное событие
        /// </summary>
        public bool Annually { get; set; }
        
        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Ключевые слова
        /// </summary>
        public string KeyW { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Ссылка на редактирование сайта. будет заполнено если у события есть сайт
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Изображение
        /// </summary>
        public Photo PreviewImage { get; set; }

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
        /// Связь с другими объектами/сущностями
        /// </summary>
        public ObjectLinks Links { get; set; }
    }

    /// <summary>
    /// Модель, описывающая события для привязки к новостям/событиям/организации/персоне и т.д
    /// </summary>
    public class EventsShortModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Дата начала события
        /// </summary>
        public DateTime DateBegin { get; set; }
        
        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Наличие связей с объектом
        /// </summary>
        public bool Checked { get; set; }
    }
}