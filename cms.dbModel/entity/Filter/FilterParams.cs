using System;
using System.Web;

public class FilterParams
{
    public string Domain { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
    public bool? Disabled { get; set; }
    public string Type { get; set; }
    public string Categoty { get; set; }
    public string Group { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DateEnd { get; set; }
    public string SearchText { get; set; }
    public string Lang { get; set; }
}
