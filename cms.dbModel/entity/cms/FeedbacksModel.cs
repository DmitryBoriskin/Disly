using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class FeedbacksList
    {
        public FeedbackModel[] Data;
        public Pager Pager;
    }

    public class FeedbackModel
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// varchar(256)
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// varchar(2048)
        /// </summary>
        [Required(ErrorMessage = "Поле не должно быть пустым.")]
        public string Text { get; set; }
        /// <summary>
        /// Дата
        /// </summary>
        [Display(Name = "Дата")]
        [Required(ErrorMessage = "Поле не должно быть пустым.")]
        public DateTime Date { get; set; }
        /// <summary>
        /// почта отправителя varchar(50)
        /// </summary>
        public string SenderEmail { get; set; }
        /// <summary>
        /// имя отправителя varchar(256)
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// ответ varchar(4096)
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// имя, ответившего на сообщение varchar(256)
        /// </summary>
        public string Answerer { get; set; }
        /// <summary>
        /// Новое
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// Неактивное
        /// </summary>
        public bool Disabled { get; set; }
    }
}