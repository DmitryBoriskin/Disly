using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Организация
    /// </summary>
    public class OrgsModel
    {        
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Короткое название
        /// </summary>
        public string ShortTitle { get; set; }

        /// <summary>
        /// Логотип
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Контактная информация
        /// </summary>
        public string Contacts { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Координата-x
        /// </summary>
        public double? GeopointX { get; set; }

        /// <summary>
        /// Координата-y
        /// </summary>
        public double? GeopointY { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }        

        /// <summary>
        /// Телефон приёмного отделения
        /// </summary>
        public string PhoneReception { get; set; }

        /// <summary>
        /// Факс
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Директорская должность
        /// </summary>
        public string DirecorPost { get; set; }

        /// <summary>
        /// Директор
        /// </summary>
        public Guid? DirectorF { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Структурные подразделения, входящие в организацию
        /// </summary>
        public StructureModel[] Structure { get; set; }

        /// <summary>
        /// Домен
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Идентификатор в ФРМП
        /// </summary>
        public string Frmp { get; set; }

        public Guid[] Types { get; set; }
    }

    /// <summary>
    /// Структурное подразделение
    /// </summary>
    public class StructureModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        public Guid OrgId { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Поле «Название структуры» не должно быть пустым.")]
        public string Title { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Adress { get; set; }

        /// <summary>
        /// Координата-x
        /// </summary>
        public double? GeopointX { get; set; }

        /// <summary>
        /// Координата-y
        /// </summary>
        public double? GeopointY { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Телефон приёмной
        /// </summary>
        public string PhoneReception { get; set; }

        /// <summary>
        /// Факс
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// как до нас добраться(маршрут)
        /// </summary>
        public string Routes { get; set; }
        
        /// <summary>
        /// График работы
        /// </summary>
        public string Schedule { get; set; }

        /// <summary>
        /// Должность директора
        /// </summary>
        public string DirecorPost { get; set; }

        /// <summary>
        /// Директор
        /// </summary>
        public Guid? DirectorF { get; set; }

        /// <summary>
        /// true- если это (ФАП/ОВП)
        /// </summary>
        public bool Ovp { get; set; }
        
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
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Структура
        /// </summary>
        public Guid StructureF { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Директорская должность
        /// </summary>
        public string DirecorPost { get; set; }

        /// <summary>
        /// Директор
        /// </summary>
        public Guid DirectorF { get; set; }

        /// <summary>
        /// Телефоны
        /// </summary>
        public DepartmentsPhone[] Phones { get; set; }

        /// <summary>
        /// Сотрудники
        /// </summary>
        public People[] Peoples { get; set; }
    }

    /// <summary>
    /// Телефон подразделения
    /// </summary>
    public class DepartmentsPhone
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Сотрудники
    /// </summary>
    public class People
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
        /// Привязка к организации
        /// </summary>
        public Guid IdLinkOrg { get; set; }
    }
    
    /// <summary>
    /// Хлебные крошки
    /// </summary>
    public class BreadCrumb
    {
        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Модель, описывающая координаты
    /// </summary>
    public class CoordModel
    {
        /// <summary>
        /// X
        /// </summary>
        public double? GeopointX { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        public double? GeopointY { get; set; }
    }

    /// <summary>
    /// Модель, описывающая типы организаций
    /// </summary>
    public class OrgType
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Флаг
        /// </summary>
        public bool Check { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public OrgsModelSmall[] Orgs { get; set; }
    }

    /// <summary>
    /// Модель, описывающая малым описанием организацию
    /// </summary>
    public class OrgsModelSmall
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Флаг 
        /// </summary>
        public bool Check { get; set; }
    }

    /// <summary>
    /// Модель, описывающая совокупность новости и привязанных к ней организаций
    /// </summary>
    public class MaterialOrgType
    {
        /// <summary>
        /// Типы организаций
        /// </summary>
        public OrgType[] OrgTypes { get; set; }

        /// <summary>
        /// Новость
        /// </summary>
        public MaterialsModel Material { get; set; }
    }
}

