using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class cmsBannersModel
    {
        public Guid Id { get; set; }
        public string SiteId { get; set; }
        [Required]
        [Display(Name = "Название баннера")]
        public string Title { get; set; }
        public string Photo { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Тип")]
        public string Type { get; set; }
        public string TypeText { get; set; }
        [Required]
        [Display(Name = "Секция")]
        public string Section { get; set; }
        public string SectionText { get; set; }
        public int Permit { get; set; }
        public bool Target { get; set; }
        public int Visits { get; set; }
        public int Clicks { get; set; }
        public bool Disabled { get; set; }
    }

    public class BannersModel
    {
        public cmsBannersModel[] BannersList { get; set; }
    }
}
