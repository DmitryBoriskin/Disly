using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class AccountModel
    {
        public Guid id { get; set; }
        public string Mail { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        public string Group { get; set; }        
        public string Surname { get; set; }
        public string Name { get; set; }        
        public string Patronymic { get; set; }
        public bool CountError { get; set; }
        public DateTime? LockDate { get; set; }
        public bool Disabled { get; set; }
        public DomainList[] Domains { get; set; }
    }

    public class DomainList
    {
        public int Permit { get; set; }
        public string SiteId { get; set; }
        public string DomainName { get; set; }
    }
}

