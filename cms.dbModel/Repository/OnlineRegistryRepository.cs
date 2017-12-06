using cms.dbModel.entity;

namespace cms.dbModel
{
    public abstract class AbstractOnlineRegistryRepository
    {
        public abstract Doctor[] getVDoctors(string oid);
        public abstract Doctor[] getVDoctors(string oid, string snils);
        public abstract Hospital getHospital(string oid);
        public abstract Hierarhy getHierarhy(int id);
    }
}
