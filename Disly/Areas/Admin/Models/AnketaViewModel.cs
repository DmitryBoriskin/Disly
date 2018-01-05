using cms.dbModel.entity;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Web;
 
 namespace Disly.Areas.Admin.Models
 {
     public class AnketaViewModel : CoreViewModel
     {
        public AnketasList List { get; set; }
        public AnketaModel Item { get; set; }
     }
}