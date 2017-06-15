using cms.dbModel.entity;
using System;

namespace cms.dbModel
{
    public abstract class abstract_FrontRepository
    {
        public abstract string getSiteId(string Domain);

        public abstract string getView(string siteId, string siteSection);

    }
}