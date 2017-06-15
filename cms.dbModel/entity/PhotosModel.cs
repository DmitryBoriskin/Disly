using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class PhotosModel
    {
        public Guid Id { get; set; }
        public Guid Album_Id { get; set; }
        //[Required(ErrorMessage = "Поле «Наименование фотоальбома» не должно быть пустым.")]
        public string Title { get; set; }
        //[Required(ErrorMessage = "Поле «Дата публикации» не должно быть пустым.")]
        public DateTime Date { get; set; }
        public string Preview { get; set; }
        //[Required(ErrorMessage = "Поле «Фотография» не должно быть пустым.")]
        public string Photo { get; set; }
    }
}
