using cms.dbModel.entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class WorksheetViewModel : PageViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public AnketasList List { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AnketaModel Item { get; set; }

        /// <summary>
        /// Доболнительная информация из библиотеки
        /// </summary>
        public SiteMapModel DopInfo { get; set; }
    }
}
