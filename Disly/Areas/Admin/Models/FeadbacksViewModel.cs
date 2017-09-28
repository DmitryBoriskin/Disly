using cms.dbModel.entity;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Web;
 
 namespace Disly.Areas.Admin.Models
 {
     public class FeedbacksViewModel : CoreViewModel
     {
         public FeedbacksList List { get; set; }
         public FeedbackModel Item { get; set; }
     }
 }