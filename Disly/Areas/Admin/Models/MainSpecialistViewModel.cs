using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель для представления главных специалистов
    /// </summary>
    public class GSViewModel : CoreViewModel
    {
        

        /// <summary>
        /// Список главных специалистов
        /// </summary>
        public GSList List { get; set; }

        /// <summary>
        /// Единичная запись главного специалиста
        /// </summary>
        public GSModel Item { get; set; }

        /// <summary>
        /// Список должностей/специализации
        /// </summary>
        public Specialisation[] Spesializations { get; set; }

        /// <summary>
        /// Список сотрудников по специализации
        /// </summary>
        public EmployeeModel[] EmployeeList { get; set; }

        /// <summary>
        /// Список всех врачей
        /// </summary>
        public EmployeeModel[] AllDoctors { get; set; }
    }


    public class GSMemberViewModel: CoreViewModel
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
