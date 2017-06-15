using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class cmsVideoModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Desc { get; set; }
        public string VideoUrl { get; set; }
        public string PreviewUrl { get; set; }
        public bool Convert { get; set; }        
    }
}
