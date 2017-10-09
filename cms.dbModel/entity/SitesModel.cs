using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class SitesList
    {
        public SitesModel[] Data;
        public Pager Pager;
    }

    public class SitesModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Поле «Доменное имя» не должно быть пустым.")]
        [RegularExpression(@"^[^-]([a-zA-Z0-9-]+)$", ErrorMessage = "Поле «Доменное имя» может содержать только буквы латинского алфавита и символ - (дефис). Доменное имя не может начинаться с дефиса.")]
        public string Alias { get; set; }
        public string Type { get; set; }
        public Guid ContentId { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Site { get; set; }
        public string Worktime { get; set; }
        public string Logo { get; set; }
        public string Scripts { get; set; }
        public Domain[] DomainList { get; set; }
    }

    public class Domain
    {
        public Guid id { get; set;}
        public string DomainName { get; set; }
    }
}