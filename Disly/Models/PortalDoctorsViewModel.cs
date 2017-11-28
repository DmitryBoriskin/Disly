﻿using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель, докторов портала
    /// </summary>
    public class PortalDoctorsViewModel : PageViewModel
    {
        /// <summary>
        /// Постраничный список докторов
        /// </summary>
        public DoctorList DoctorList { get; set; }

        /// <summary>
        /// Единичная запись доктора
        /// </summary>
        public People Doctor { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public PeoplePost[] PeoplePosts { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }
    }
}
