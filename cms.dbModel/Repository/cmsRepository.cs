using cms.dbModel.entity;
using System;

namespace cms.dbModel
{
    public abstract class abstract_cmsRepository
    {
        public abstract string getSiteId(string Domain);

        public abstract SitesModel getSite(Guid? Id);
        public abstract SitesModel getSite(string domain);

        // Работа с логами
        public abstract cmsLogModel[] getCmsUserLog(Guid UserId);
        public abstract cmsLogModel[] getCmsPageLog(Guid PageId);
        public abstract void insertLog(Guid UserId, string IP, string Action, Guid PageId, string PageName, string Section, string Site);

        // CmsMenu
        public abstract bool check_cmsMenu(Guid id);
        public abstract bool check_cmsMenu(string alias);

        public abstract cmsMenuModel[] getCmsMenu(Guid user_id);
        public abstract cmsMenuItem[] getCmsMenuItems(string group_id, Guid user_id);
        public abstract cmsMenuItem getCmsMenuItem(Guid id);
        public abstract cmsMenuType[] getCmsMenuType();
        public abstract bool createCmsMenu(Guid id, cmsMenuItem Item, Guid UserId, string IP);
        public abstract bool updateCmsMenu(Guid id, cmsMenuItem Item, Guid UserId, string IP);
        public abstract bool deleteCmsMenu(Guid id, Guid UserId, string IP);
        public abstract bool permit_cmsMenu(Guid id, int num, Guid UserId, string IP);

        // Все сайты портала
        public abstract SitesList getSiteList(string[] filtr, int page, int size);

        // Все пользователи портала
        public abstract bool check_user(Guid id);
        public abstract bool check_user(string email);
        public abstract void check_usergroup(Guid id, string group, Guid UserId, string IP);
        
        public abstract UsersList getUsersList(FilterParams filtr);
        public abstract UsersModel getUser(Guid id);
        public abstract bool createUser(Guid id, UsersModel Item, Guid UserId, string IP);
        public abstract bool updateUser(Guid id, UsersModel Item, Guid UserId, string IP);
        public abstract bool deleteUser(Guid id, Guid UserId, string IP);

        public abstract void changePassword(Guid id, string Salt, string Hash, Guid UserId, string IP);

        public abstract Catalog_list[] getUsersGroupList();
        public abstract UsersGroupModel getUsersGroup(string alias);

        public abstract ResolutionsModel[] getGroupResolutions(string alias);
        
        // Материалы
        public abstract MaterialsList getMaterialsList(FilterParams filtr);
        public abstract MaterialsModel getMaterial(Guid id);
        
        //Orgs
        public abstract OrgsModel[] getOrgs(FilterParams filtr);
        
        // Персоны
        public abstract UsersList getPersonList(FilterParams filtr);
        public abstract UsersModel getPerson(Guid id);
    }
}