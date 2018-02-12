using System;
using System.Web;

public class EventFilter : FilterParams
{
    /// <summary>
    /// Id элемента, привязанного к событию
    /// </summary>
    public Guid? RelId { get; set; }
    /// <summary>
    /// тип элемента, привязанного к событию
    /// </summary>
    public ContentType RelType { get; set; }
}
