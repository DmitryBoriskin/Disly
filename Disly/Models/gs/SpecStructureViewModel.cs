using cms.dbModel.entity;

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
        public GSMemberModel[] SpesialitsList { get; set; }
        /// <summary>
        /// Экспертный состав
        /// </summary>
        public GSMemberModel[] ExpertsList { get; set; }
        /// <summary>
        /// Список докторов по специальности
        /// </summary>
        public PeopleList DoctorsList { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public Specialisation[] Specializations { get; set; }

    }
}
