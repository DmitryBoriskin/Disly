using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class cmsSiteMapModel
    {
        public Guid id { get; set; }
        public string Org { get; set; }
        public Guid View { get; set; }
        public string Type { get; set; }
        public int Permit { get; set; }
        public string Site { get; set; }
        public string Menu { get; set; }
        public string Path { get; set; }
        //[Required]
        [Display(Name = "Алиас")]
        public string Alias { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Title { get; set; }
        public string Logo { get; set; }
        public string File { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public string Desc { get; set; }
        public string Keyw { get; set; }
        public bool Disabled { get; set; }
    }
}

