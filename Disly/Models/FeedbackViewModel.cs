using cms.dbModel.entity;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class FeedbackViewModel : PageViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public FeedbacksList List { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FeedbackModel Item { get; set; }

        /// <summary>
        /// Навигация
        /// </summary>
        public MaterialsGroup[] Nav { get; set; }
        /// <summary>
        /// Доболнительная информация из библиотеки
        /// </summary>
        public SiteMapModel DopInfo { get; set; }
    }

    public class FeedbackFormViewModel
    {
        /// <summary>
        /// Отправитель
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// Email отправителя
        /// </summary>
        public string SenderEmail { get; set; }
        /// <summary>
        /// Доп контакты отправителя
        /// </summary>
        public string SenderContacts { get; set; }
        /// <summary>
        /// Тема сообщения
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// текст вопроса
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Загруженный файл
        /// </summary>
        public HttpPostedFileBase FileToUpload { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileUploaded { get; set; }
        /// <summary>
        /// Согласен на обработку данных
        /// </summary>
        public bool IsAgree { get; set; }

        ///// <summary>
        ///// Отправлять по Email
        ///// </summary>
        //public bool ByEmail { get; set; }
        ///// <summary>
        ///// Адрес, на который необходимо отправлять ответ
        ///// </summary>
        //public string PostAddress { get; set; }
    }

    public class FeedbackAnswerFormViewModel
    {
        /// <summary>
        /// id сообщения на которое отвечаем
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// проверочный код
        /// </summary>
        public Guid AnswererCode { get; set; }
        /// <summary>
        /// отвечает(кто)
        /// </summary>
        public string Answerer { get; set; }
        /// <summary>
        /// текст ответа
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// Опубликовать на сайте
        /// </summary>
        public bool Publish { get; set; }
        /// <summary>
        /// Отправить ответ по email
        /// </summary>
        public bool ByEmail { get; set; }
    }
}
