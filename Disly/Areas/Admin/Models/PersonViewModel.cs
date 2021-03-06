﻿using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель для представления врачей
    /// </summary>
    public class PersonViewModel : CoreViewModel
    {
        /// <summary>
        /// Сотрудник
        /// </summary>
        public EmployeeModel Item { get; set; }

        /// <summary>
        /// Список сотрудников
        /// </summary>
        public UsersList List { get; set; }

        /// <summary>
        /// Должности
        /// </summary>
        public IEnumerable<Specialisation> EmployeePosts { get; set; }
    }
}
