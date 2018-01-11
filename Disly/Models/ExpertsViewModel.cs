using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы специалисты
    /// </summary>
    public class ExpertsViewModel : PageViewModel
    {
        /// <summary>
        /// Общая информация о специальности из раздела главных специалистов
        /// </summary>
        public string SpecializationInfo { get; set; }

        /// <summary>
        /// Список докторов по специальности
        /// </summary>
        public People[] DoctorsList { get; set; }

        /// <summary>
        /// Список докторов главных специалистов
        /// </summary>
        public People[] SpesialitsList { get; set; }

        /// <summary>
        /// Экспертный состав
        /// </summary>
        public People[] ExpertsList { get; set; }

        /// <summary>
        /// тип информации показываемой на странице 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Навигация
        /// </summary>
        public List<MaterialsGroup> Nav { get; set; }

        /// <summary>
        /// Информация о странице (доп. инфо) из карты сайта
        /// </summary>
        public SiteMapModel Info { get; set; }
    }
}
