using cms.dbModel.entity;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы контактов
    /// </summary>
    public class ContatcsViewModel : PageViewModel
    {   
        /// <summary>
        /// Организация
        /// </summary>
        public OrgsModel OrgItem { get; set; }

        /// <summary>
        /// Список структур
        /// </summary>
        public StructureModel[] Structures { get; set; }      

        /// <summary>
        /// Административный персонал
        /// </summary>
        public OrgsAdministrative[] Administrativ { get; set; }

        /// <summary>
        /// тип информации показываемой на странице 
        /// </summary>
        public string Type { get; set; }

    }
}
