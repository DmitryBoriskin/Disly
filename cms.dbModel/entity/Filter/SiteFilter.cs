using System;
using System.Web;

public class SiteFilter : FilterParams
{
    /// <summary>
    /// Id элемента, привязанного к сайту
    /// </summary>
    public Guid? RelId { get; set; }

    /// <summary>
    /// тип элемента, привязанного к сайту
    /// </summary>
    public ContentType RelType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? UserId { get; set; }



}
