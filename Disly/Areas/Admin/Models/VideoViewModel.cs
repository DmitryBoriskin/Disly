using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class VideoViewModel : CoreViewModel
    {
        public cmsVideoModel[] List { get; set; }
        public cmsVideoModel Item { get; set; }
    }
}