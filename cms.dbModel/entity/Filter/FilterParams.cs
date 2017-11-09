using System;
using System.Web;

public class FilterParams
{
    public string Domain { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
    public bool? Disabled { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string Group { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DateEnd { get; set; }
    public string SearchText { get; set; }
    public string Lang { get; set; }

    public static T Extend<T>(FilterParams f)
        where T: FilterParams, new()
    {
        return new T()
        {
            Domain = f.Domain,
            Page = f.Page,
            Size = f.Size,
            Disabled = f.Disabled,
            Type = f.Type,
            Category = f.Category,
            Group = f.Group,
            Date = f.Date,
            DateEnd = f.DateEnd,
            SearchText = f.SearchText,
            Lang = f.Lang
        };
    }

}
