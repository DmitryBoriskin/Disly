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

    //Модель описывающая права группы на раздел сайта
    public class O
    {
        /// <summary>
        /// Алиас группы
        /// </summary>
        public string GroupAlias { get; set; }
        /// <summary>
        /// Тип раздела сайта, к которому настраивается доступ
        /// </summary>
        public Guid ContentId { get; set; }
        /// <summary>
        /// Тип действия
        /// </summary>
        public string Claim { get; set; }
        /// <summary>
        /// разрешено
        /// </summary>
        public bool Checked { get; set; }
    }

}