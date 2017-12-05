using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{

    public enum FeedbackType
    {
        /// <summary>
        /// не определено
        /// </summary>
        undefined = 0,
        /// <summary>
        /// Вопрос по обратной связи
        /// </summary>
        appeal = 1,
        /// <summary>
        /// Отзыв
        /// </summary>
        review = 2,
        /// <summary>
        /// Часто задаваемые вопросы
        /// </summary>
        faq = 3
    }

    /// <summary>
    /// Список отзывов с пейджером
    /// </summary>
    public class FeedbacksList
    {
        /// <summary>
        /// Список событий
        /// </summary>
        public FeedbackModel[] Data;

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager;
    }

    /// <summary>
    /// Отзыв
    /// </summary>
    public class FeedbackModel
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
        /// Текст
        /// </summary>
        [Required(ErrorMessage = "Поле не должно быть пустым.")]
        public string Text { get; set; }

        /// <summary>
        /// Тип сообщения из обратной связи
        /// </summary>
        public FeedbackType Fbtype { get; set; }

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
        /// контакты отправителя nvarchar(1024)
        /// </summary>
        public string SenderContacts { get; set; }

        /// <summary>
        /// Отправить анонимно
        /// </summary>
        public bool Anonymous { get; set; }

        /// <summary>
        /// ответ varchar(4096)
        /// </summary>
        public string Answer { get; set; }
        
        /// <summary>
        /// имя, ответившего на сообщение varchar(256)
        /// </summary>
        public string Answerer { get; set; }

        /// <summary>
        /// Guid, код для ответа через форму на сайте
        /// </summary>
        public Guid? AnswererCode { get; set; }

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