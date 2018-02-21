using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Постраничный список врачей (сущность человек)
    /// </summary>
    public class PeopleList
    {
        /// <summary>
        /// Список докторов
        /// </summary>
        public PeopleModel[] Doctors { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// врач (сущность человек)
    /// </summary>
    public class PeopleModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        /// СНИЛС
        /// </summary>
        public string SNILS { get; set; }

        /// <summary>
        /// Привязка к организации
        /// </summary>
        public Guid IdLinkOrg { get; set; }

        /// <summary>
        /// Привязка к гс
        /// </summary>
        public Guid IdLinkGS { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Инфа в формате xml
        /// </summary>
        public string[] XmlInfo { get; set; }

        /// <summary>
        /// Десериализованная инфа по сотруднику
        /// </summary>
        public Employee EmployeeInfo { get; set; }

        /// <summary>
        /// Фотография
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Список должностей сотрудников
        /// </summary>
        public IEnumerable<Specialisation> Posts { get; set; }

        /// <summary>
        /// Есть ли ссылка на регистрацию
        /// </summary>
        public bool IsRedirectUrl { get; set; }

        /// <summary>
        /// Идентификатор структуры
        /// </summary>
        public int? StructureId { get; set; }

        /// <summary>
        /// Идентификатор департамента
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Название департамента
        /// </summary>
        public string DepartmentTitle { get; set; }

        /// <summary>
        /// главный специалист
        /// </summary>
        public GSModel GS { get; set; }
    }
}


