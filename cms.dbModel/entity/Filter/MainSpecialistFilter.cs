using System;
using System.Web;

public class GSFilter : FilterParams {
    /// <summary>
    /// Id элемента, привязанного к гс 
    /// </summary>
    public Guid? RelId { get; set; }

    /// <summary>
    /// тип элемента, привязанного к гс
    /// </summary>
    public ContentType RelType { get; set; }

}