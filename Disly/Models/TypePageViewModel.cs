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
    public class TypePageViewModel : PageViewModel
    {         
        public SiteMapModel Item { get; set; }
        public SiteMapModel[] Child{ get; set; }
    }
}
