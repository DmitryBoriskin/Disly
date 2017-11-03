using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для внешнего представления сайта
    /// </summary>
    public class PageViewModel
    {
        /// <summary>
        /// Информация по ошибками
        /// </summary>
        public ErrorViewModel ErrorInfo { get; set; }

        /// <summary>
        /// Информация по сайту
        /// </summary>
        public SitesModel SitesInfo { get; set; }

        /// <summary>
        /// Группы меню
        /// </summary>
        public SiteMapModel[] SiteMapArray { get; set; }

        /// <summary>
        /// Баннеры
        /// </summary>
        public BannersModel[] BannerArray { get; set; }
    }

    /// <summary>
    /// Модель для внешнего представления ошибок
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// http-code
        /// </summary>
        public Int32? HttpCode { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Обратная ссылка
        /// </summary>
        public String BackUrl { get; set; }
    }
}