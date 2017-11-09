using cms.dbModel.entity;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Web;
 
 namespace Disly.Areas.Admin.Models
 {
     public class OrgsModalViewModel : CoreViewModel
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
        /// Выбранные организации, к которым привязываем новость, чтобы биндить данные
        /// </summary>
        public Guid[] OrgsId { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public OrgsShortModel[] OrgsList { get; set; }
        /// <summary>
        /// Справочник всех типов организаций
        /// </summary>
        public OrgType[] OrgsTypes { get; set; }
    }
 }