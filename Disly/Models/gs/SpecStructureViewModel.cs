using cms.dbModel.entity;
using cms.dbModel.entity.cms;
using System.Collections.Generic;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы специалисты
    /// </summary>
    public class SpecStructureViewModel : PageViewModel
    {
        /// <summary>
        /// тип информации, показываемой на странице 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Общая информация о модели
        /// </summary>
        public GSModel MainSpec { get; set; }
        /// <summary>
        /// Список докторов главных специалистов
        /// </summary>
        public People[] SpesialitsList { get; set; }
        /// <summary>
        /// Экспертный состав
        /// </summary>
        public People[] ExpertsList { get; set; }
        /// <summary>
        /// Список докторов по специальности
        /// </summary>
        public People[] DoctorsList { get; set; }


    }
}
