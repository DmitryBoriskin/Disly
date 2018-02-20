using System;
using System.Web;

public class OrgFilter: FilterParams
{
    /// <summary>
    /// Id элемента, привязанного к организации 
    /// </summary>
    public Guid? RelId { get; set; }

    /// <summary>
    /// тип элемента, привязанного к организации 
    /// </summary>
    public ContentType RelType { get; set; }

    /// <summary>
    /// Исключаем
    /// </summary>
    public Guid? Except { get; set; }

    /// <summary>
    /// Фильтр по человеку
    /// </summary>
    public Guid? PeopleId { get; set; }
}
