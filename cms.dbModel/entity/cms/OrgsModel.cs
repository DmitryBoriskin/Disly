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
        public StructureModel[] Structure { get; set; }
    }
    


    /// <summary>
    /// Структура организации
    /// </summary>
    public class StructureModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле «Название структуры» не должно быть пустым.")]
        public string Title { get; set; }
        public string Adress { get; set; }
        public double? GeopontX { get; set; }
        public double? GeopontY { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// как до нас добраться(маршрут)
        /// </summary>
        public string Routes { get; set; }
        public string DirecorPost { get; set; }
        public Guid DirectorF { get; set; }
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
    }
}
