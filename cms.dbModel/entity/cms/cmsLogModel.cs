using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class cmsLogModel
    {
        public Guid PageId { get; set; }
        public string PageName { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public string Surname{ get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public string Section { get; set; }
        public string Action { get; set; }
        public string IP { get; set; }
    }
}
