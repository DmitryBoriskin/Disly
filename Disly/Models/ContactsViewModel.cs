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
        public OrgsAdministrative[] Administrativ { get; set; }
        /// <summary>
        /// тип информации показываемой на странице 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Навигация
        /// </summary>
        public MaterialsGroup[] Nav { get; set; }
        /// <summary>
        /// Доболнительная информация из библиотеки
        /// </summary>
        public SiteMapModel DopInfo { get; set; }
    }
}
