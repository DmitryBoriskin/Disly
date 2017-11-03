using System;
using System.Web;

public class OrgFilter: FilterParams
{
    /// <summary>
    /// Должен передаваться либо MaterialId либо EventId
    /// </summary>
    public Guid? MaterialId { get; set; }
    /// <summary>
    /// Должен передаваться либо MaterialId либо EventId
    /// </summary>
    public Guid? EventId { get; set; }
}
