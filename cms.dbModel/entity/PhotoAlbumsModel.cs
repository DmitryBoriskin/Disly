using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class PhotoAlbumsModel
    {
        public Guid Id { get; set; }
        public string SiteId { get; set; }
        [Required(ErrorMessage = "Поле «Наименование фотоальбома» не должно быть пустым.")]
        public string Title { get; set; }
        public string Desc { get; set; }
        [Required(ErrorMessage = "Поле «Дата публикации» не должно быть пустым.")]
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string Preview { get; set; }
    }
}
