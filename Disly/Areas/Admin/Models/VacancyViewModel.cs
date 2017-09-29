using cms.dbModel.entity;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Web;
 
 namespace Disly.Areas.Admin.Models
 {
     public class VacanciesViewModel : CoreViewModel
     {
         public VacanciesList List { get; set; }
         public VacancyModel Item { get; set; }
     }
 }