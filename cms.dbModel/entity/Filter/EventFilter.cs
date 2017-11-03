using System;
using System.Web;

public class EventFilter : FilterParams
{
    public Guid? MaterialId { get; set; }
    public Guid? EventId { get; set; }
}
