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
        public Guid[] PersonsId { get; set; }
        /// <summary>
        /// События
        /// </summary>
        //public PersonsShort[] Persons { get; set; }


        //К новостям ничего не привязывается

    }
}