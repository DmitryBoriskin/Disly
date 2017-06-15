using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class cmsMaterialsModel
    {
        public Guid id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Title { get; set; }        
        [Display(Name = "Алиас")]
        public string Site { get; set; }
        public string Alias { get; set; }
        public string Org { get; set; }
        public string Keyw { get; set; }
        public string Desc { get; set; }
        [Required]        
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Text { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
        public string Type { get; set; }        
        public string Category { get; set; }
        public bool Disabled { get; set; }
        public bool Deleted { get; set; }
        public string Url { get; set; }
        public string UrlName { get; set; }
    }
}
