using System;
using System.Web;

public class SiteFilter : FilterParams
{
    /// <summary>
    /// Должен передаваться либо MaterialId либо EventId
    /// </summary>
    public Guid? UserId { get; set; }

}
