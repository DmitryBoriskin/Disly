using System;
using cms.dbModel.entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class cmsPageViewsModel
    {
        public Guid id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Ссылка на представление")]
        public string Url { get; set; }
        public bool ReadOnly { get; set; }
    }
}
