using cms.dbModel.entity;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Web;
 
 namespace Disly.Areas.Admin.Models
 {
     public class EventsModalViewModel : CoreViewModel
     {
        /// <summary>
        /// Id Новости или События
        /// </summary>
        public Guid ObjctId { get; set; }
        /// <summary>
        /// Тип Новость или Событие
        /// </summary>
        public ContentType ObjctType { get; set; }

        /// <summary>
        /// Выбранные события, к которым привязываем новость
        /// </summary>
        public Guid[] EventsId { get; set; }
        /// <summary>
        /// справочник последних N событий, с выбранными событиями
        /// </summary>
        public EventsShortModel[] EventsList { get; set; }
    }
 }