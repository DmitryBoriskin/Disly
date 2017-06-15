using cms.dbModel.entity;
using System;

namespace cms.dbModel
{
    public abstract class abstract_AccountRepository
    {
        public abstract AccountModel getCmsAccount(string Email);
        public abstract AccountModel getCmsAccount(Guid Id);

        public abstract DomainList[] getUserDomains(Guid Id);

        public abstract ResolutionsModel getCmsUserResolutioInfo(Guid _userId, string _pageUrl);

        public abstract void changePasswordUser(Guid id, string NewSalt, string NewHash, string IP);

        public abstract void insertLog(Guid PageId, Guid UserId, string Action, string IP);
    }
}