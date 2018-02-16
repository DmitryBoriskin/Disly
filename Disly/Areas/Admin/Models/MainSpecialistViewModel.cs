using cms.dbModel.entity.cms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public IEnumerable<EmployeePost> EmployeePostList { get; set; }

        /// <summary>
        /// Список главных специалистов
        /// </summary>
        public GSList List { get; set; }

        /// <summary>
        /// Единичная запись главного специалиста
        /// </summary>
        public GSModel Item { get; set; }

        /// <summary>
        /// Список сотрудников по специализации
        /// </summary>
        public IEnumerable<EmployeeModel> EmployeeList { get; set; }

        /// <summary>
        /// Список всех врачей
        /// </summary>
        public IEnumerable<EmployeeModel> AllDoctors { get; set; }
    }

    public class GSEmployeeViewModel: CoreViewModel
    {
        
        /// <summary>
        /// 
        /// </summary>
        public GSMemberModel Member { get; set; }

        /// <summary>
        /// Список сотрудников по специализации
        /// </summary>
        public IEnumerable<EmployeeModel> EmployeeList { get; set; }

    }
}
