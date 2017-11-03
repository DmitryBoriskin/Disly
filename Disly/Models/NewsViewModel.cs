using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы новостей
    /// </summary>
    public class NewsViewModel : PageViewModel
    {
        public MaterialsList List { get; set; }
        public MaterialsModel Item { get; set; }
    }
}
