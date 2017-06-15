using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class PlaceCardModel
    {
        public Guid id { get; set; }

        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        [Display(Name = "Название")]
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Site { get; set; }
        public string Keyw { get; set; }
        public string Desc { get; set; }

        [Required(ErrorMessage = "Поле «Дата начала события» не должно быть пустым.")]        
        [Display(Name = "Дата начала события")]
        public DateTime DateStart { get; set; }
        public TimeSpan TimeStart { get; set; }

        [Required(ErrorMessage = "Поле «Дата окончания события» не должно быть пустым.")]
        [Display(Name = "Дата окончания события")]
        public DateTime DateEnd { get; set; }
        public TimeSpan TimeEnd { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Text { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
        public string Place { get; set; }        
        public string Url { get; set; }
        public string UrlName { get; set; }

        [Required]
        public bool Disabled { get; set; }
    }
}
