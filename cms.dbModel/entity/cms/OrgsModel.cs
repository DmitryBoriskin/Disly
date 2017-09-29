using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Организация
    /// </summary>
    public class OrgsModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле «Название организации» не должно быть пустым.")]
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public bool Disabled { get; set; }
        public StructureModel[] Structure { get; set; }
    }

    /// <summary>
    /// Структурное подразделение
    /// </summary>
    public class StructureModel
    {
        [Required]
        public Guid Id { get; set; }
        public Guid OrgId { get; set; }
        [Required(ErrorMessage = "Поле «Название структуры» не должно быть пустым.")]
        public string Title { get; set; }
        public string Adress { get; set; }
        public double? GeopointX { get; set; }
        public double? GeopointY { get; set; }
        public string Phone { get; set; }
        public string PhoneReception { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// как до нас добраться(маршрут)
        /// </summary>
        public string Routes { get; set; }
        /// <summary>
        /// График работы
        /// </summary>
        public string Schedule { get; set; }
        public string DirecorPost { get; set; }
        public Guid? DirectorF { get; set; }
        /// <summary>
        /// Отделения
        /// </summary>
        public Departments[] Departments { get; set; }
    }
    /// <summary>
    /// Отделение
    /// </summary>
    public class Departments
    {
        [Required]
        public Guid Id { get; set; }
        public Guid StructureF { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string DirecorPost { get; set; }
        public Guid DirectorF { get; set; }
        public DepartmentsPhone[] Phones { get; set; }
        public People[] Peoples { get; set; }
    }

    public class DepartmentsPhone
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class People
    {
        [Required]
        public Guid Id { get; set; }
        public string FIO { get; set; }        
    }
    

    public class BreadCrumb
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }

}

