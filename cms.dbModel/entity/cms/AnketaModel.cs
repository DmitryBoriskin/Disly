using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Список анкет с пэйджером
    /// </summary>
    public class AnketasList
    {
        /// <summary>
        /// Список
        /// </summary>
        public AnketaModel[] Data;

        /// <summary>
        /// Пэйджер
        /// </summary>
        public Pager Pager;
    }

    /// <summary>
    /// Анкета
    /// </summary>
    public class AnketaModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Счетчик
        /// </summary>
        public int Count { get; set; }

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
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Дата начала события
        /// </summary>
        [Display(Name = "Дата начала анкетирования")]
        [Required(ErrorMessage = "Поле «Дата» не должно быть пустым.")]
        public DateTime DateBegin { get; set; }
        
        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        public bool Disabled { get; set; }

        ///// <summary>
        ///// Изображение
        ///// </summary>
        //public Photo PreviewImage { get; set; }

        /// <summary>
        /// ссылка на организацию/событие/персону по умолчанию
        /// </summary>
        public Guid ContentLink { get; set; }

        /// <summary>
        /// Тип (организация/событие/персона) по умолчанию
        /// </summary>
        public string ContentLinkType { get; set; }

        /// <summary>
        /// Связь с другими объектами/сущностями
        /// </summary>
        public ObjectLinks Links { get; set; }
    }

    ///// <summary>
    ///// Модель, описывающая события для привязки к новостям/событиям/организации/персоне и т.д
    ///// </summary>
    //public class EventsShortModel
    //{
    //    /// <summary>
    //    /// Идентификатор
    //    /// </summary>
    //    public Guid Id { get; set; }
        
    //    /// <summary>
    //    /// Дата начала события
    //    /// </summary>
    //    public DateTime DateBegin { get; set; }
        
    //    /// <summary>
    //    /// Название
    //    /// </summary>
    //    public string Title { get; set; }

    //    /// <summary>
    //    /// Название
    //    /// </summary>
    //    public string Text { get; set; }

    //    /// <summary>
    //    /// Наличие связей с объектом
    //    /// </summary>
    //    public bool Checked { get; set; }
    //}
}