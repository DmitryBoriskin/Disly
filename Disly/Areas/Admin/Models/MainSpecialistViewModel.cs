using cms.dbModel.entity.cms;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель для представления главных специалистов
    /// </summary>
    public class MainSpecialistViewModel : CoreViewModel
    {
        /// <summary>
        /// Список должностей
        /// </summary>
        public IEnumerable<EmployeePostModel> EmployeePostList { get; set; }

        /// <summary>
        /// Список главных специалистов
        /// </summary>
        public MainSpecialistList List { get; set; }

        /// <summary>
        /// Единичная запись главного специалиста
        /// </summary>
        public MainSpecialistModel Item { get; set; }

        /// <summary>
        /// Список сотрудников по специализации
        /// </summary>
        public IEnumerable<EmployeeModel> EmployeeList { get; set; }
    }
}
