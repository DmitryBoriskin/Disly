using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class DoctorsViewModel : PageViewModel
    {
        public People[] DoctorsList { get; set; }
        public People DoctorsItem { get; set; }
    }
}
