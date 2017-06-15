using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class PhotoAlbumsViewModel : CoreViewModel
    {
        public PhotoAlbumsModel[] List { get; set; }
        public PhotoAlbumsModel Item { get; set; }

        public PhotosModel[] PhotoList { get; set; }
        //public PhotosModel PhotoItem { get; set; }
    }
}