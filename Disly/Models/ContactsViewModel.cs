using cms.dbModel.entity;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class ContatcsViewModel : PageViewModel
    {        
        public OrgsModel OrgItem { get; set; }
        public StructureModel[] Structures { get; set; }        
    }
}
