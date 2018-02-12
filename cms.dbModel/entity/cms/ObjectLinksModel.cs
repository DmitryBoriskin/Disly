using cms.dbModel.entity.cms;
using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class ObjectLinks
    {
        /// <summary>
        /// События
        /// </summary>
        public Guid[] EventsId { get; set; }
        /// <summary>
        /// События
        /// </summary>
        public EventsModel[] Events { get; set; }

        /// <summary>
        /// События
        /// </summary>
        public Guid[] OrgsId { get; set; }
        /// <summary>
        /// События
        /// </summary>
        public OrgsModel[] Orgs { get; set; }

        /// <summary>
        /// События
        /// </summary>
        public Guid[] SpecId { get; set; }
        /// <summary>
        /// События
        /// </summary>
        public MainSpecialistModel[] Specs { get; set; }

        /// <summary>
        /// События
        /// </summary>
        public Guid[] SitesId { get; set; }
        /// <summary>
        /// Сайты
        /// </summary>
        public SitesModel[] Sites { get; set;}

        /// <summary>
        /// Привязка к персоне/ главному специалисту
        /// </summary>
        public Guid[] PersonsId { get; set; }
    }
}