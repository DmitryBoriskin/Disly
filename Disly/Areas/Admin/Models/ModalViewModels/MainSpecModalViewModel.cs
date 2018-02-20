using cms.dbModel.entity;
using System;


namespace Disly.Areas.Admin.Models
 {
     public class MainSpecModalViewModel : CoreViewModel
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
        /// справочник последних N событий, с выбранными событиями
        /// </summary>
        public GSShortModel[] SpecList { get; set; }
    }
 }