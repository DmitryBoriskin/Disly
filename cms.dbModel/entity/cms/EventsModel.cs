using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class EventsList
    {
        public EventModel[] Data;
        public Pager Pager;
    }

    public class EventModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// ???
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// varchar(512)
        /// </summary>
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Title { get; set; }
        /// <summary>
        /// varchar(512)
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        ///  varchar(1024)
        /// </summary>
        public string Preview { get; set; }
        /// <summary>
        /// varchar(512)
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// varchar(512)
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// varchar(1024)
        /// </summary>
        public string EventMaker { get; set; }
        /// <summary>
        /// varchar(1024)
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// varchar(512)
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
        /// varchar(1024)
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// varchar(512)
        /// </summary>
        public string KeyW { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// Ссылка на редактирование сайта. будет заполнено если у события есть сайт
        /// </summary>
        public string SiteId { get; set; }        
    }
}