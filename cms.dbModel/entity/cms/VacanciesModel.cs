using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class VacanciesList
    {
        public VacancyModel[] Data;
        public Pager Pager;
    }

    public class VacancyModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Дата размещения
        /// </summary>
        [Display(Name = "Дата")]
        [Required(ErrorMessage = "Поле не должно быть пустым.")]
        public DateTime Date { get; set; }
        /// <summary>
        /// Профессия varchar(100)
        /// </summary>
        [Required(ErrorMessage = "Поле не должно быть пустым.")]
        public string Profession { get; set; }
        /// <summary>
        /// Должность varchar(512)
        /// </summary>
        public string Post { get; set; }
        /// <summary>
        /// Образование varchar(2048)
        /// </summary>
        public string Сonditions { get; set; }
        /// <summary>
        /// Опыт/Стаж varchar(2048)
        /// </summary>
        public string Experience { get; set; }
        /// <summary>
        /// Зарплата varchar(100)
        /// </summary>
        public string Salary { get; set; }
        /// <summary>
        /// Описание varchar(2048)
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// Замещение
        /// </summary>
        public bool Temporarily { get; set; }
        /// <summary>
        /// Неактивное
        /// </summary>
        public bool Disabled { get; set; }
    }
}