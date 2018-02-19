using System;
using System.Web;

public class SpecialisationFilter : FilterParams
{
    /// <summary>
    /// Фильтрация по человеку
    /// </summary>
    public Guid? PeopleId { get; set; }
    /// <summary>
    /// Фильтрация по специализациям
    /// </summary>
    public int[] Specializations { get; set; }

    /// <summary>
    /// Фильтрация по организациям
    /// </summary>
    public Guid[] Orgs { get; set; }

}
