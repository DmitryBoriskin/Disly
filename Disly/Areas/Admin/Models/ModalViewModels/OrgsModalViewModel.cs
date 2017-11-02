﻿using cms.dbModel.entity;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Web;
 
 namespace Disly.Areas.Admin.Models
 {
     public class OrgsModalViewModel : CoreViewModel
     {
        /// <summary>
        /// Список организаций по параметрам
        /// </summary>
        public OrgsModel[] OrgsList { get; set; }
        // <summary>
        /// справочник всех организаций
        /// </summary>
        public OrgsModel[] OrgsAll { get; set; }
        /// <summary>
        /// Справочник всех типов организаций
        /// </summary>
        public OrgType[] OrgsTypes { get; set; }
    }
 }