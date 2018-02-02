using cms.dbModel.entity;
using System.ComponentModel;

namespace Disly.Areas.Admin.Models
{
    public abstract class CoreViewModel
    {
        /// <summary>
        /// Название контроллера
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// Название актина
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// Текущий домен
        /// </summary>
        public string DomainName { get; set; }
        /// <summary>
        /// Настройки сайта
        /// </summary>
        public SettingsModel Settings { get; set; }
        /// <summary>
        /// Подключенный пользователь
        /// </summary>
        public AccountModel Account { get; set; }
        /// <summary>
        /// Права пользователя
        /// </summary>
        public ResolutionsModel UserResolution { get; set; }
        /// <summary>
        /// Меню админки
        /// </summary>
        public cmsMenuModel[] Menu { get; set; }
        /// <summary>
        /// Логи, последние н записей
        /// </summary>
        public cmsLogModel Log { get; set; }
        /// <summary>
        /// Ошибки
        /// </summary>
        public ErrorMessage ErrorInfo { get; set; }
    }

    // Ошибки
    public class ErrorMessage
    {
        public string title { get; set; }
        public string info { get; set; }
        public ErrorMassegeBtn[] buttons { get; set; }
    }
    public class ErrorMassegeBtn
    {
        public string url { get; set; }
        public string text { get; set; }
        [DefaultValue("default")]
        public string style { get; set; }
        public string action { get; set; }
    }

    // Pager (постраничный навигатор)
    public class PagerModel
    {
        public string text { get; set; }
        public string url { get; set; }
        public bool isChecked { get; set; }
    }


}
///// <summary>
///// Пейджер
///// </summary>
//public class PagerModel1
//{
//    /// <summary>
//    /// Кол-во эл-тов
//    /// </summary>
//    public int ItemsCount { get; set; }

//    /// <summary>
//    /// Кол-во эл-тов на странице
//    /// </summary>
//    public int PageSize { get; set; }

//    /// <summary>
//    /// Текущая страница
//    /// </summary>
//    public int Page { get; set; }

//    /// <summary>
//    /// Кол-во страниц
//    /// </summary>
//    public int PageCount
//    {
//        get
//        {
//            return ItemsCount / PageSize + (ItemsCount % PageSize == 0 ? 0 : 1);
//        }
//    }
//}
    
