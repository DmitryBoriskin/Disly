using cms.dbModel.entity;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class AbstractOnlineRegistryRepository
    {
        public abstract Doctor[] getVDoctors(string oid);
        public abstract Doctor[] getVDoctors(string oid, string snils);
        public abstract Hospital getHospital(string oid);
    }
}
