using cms.dbModel.entity;
using cms.dbModel.entity.cms;
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
        public abstract SitesList getSiteList(FilterParams filtr, int page, int size);
        public abstract bool check_Site(Guid id);
        public abstract bool insSite(SitesModel ins, Guid UserId, String IP);
        public abstract bool updSite(Guid id, SitesModel ins, Guid UserId, String IP);
        public abstract bool delSite(Guid id, Guid UserId, String IP);
        public abstract Domain[] getSiteDomains(string SiteId);
        public abstract bool insDomain(String SiteId, string NewDomain, Guid UserId, String IP);
        public abstract bool delDomain(Guid id, Guid UserId, String IP);
        public abstract string getIdSite(Guid ContentId);

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

        public abstract bool insertCmsMaterial(MaterialsModel material);
        public abstract bool updateCmsMaterial(MaterialsModel material);
        public abstract bool deleteCmsMaterial(Guid id);


        // Events
        public abstract EventsList getEventsList(FilterParams filtr);
        public abstract EventModel getEvent(Guid id);

        public abstract bool updateCmsEvent(EventModel eventData);
        public abstract bool insertCmsEvent(EventModel eventData);
        public abstract bool deleteCmsEvent(Guid id);


        // Персоны
        public abstract UsersList getPersonList(FilterParams filtr);
        public abstract UsersModel getPerson(Guid id);

        // Vacancies
        public abstract VacanciesList getVacanciesList(FilterParams filtr);
        public abstract VacancyModel getVacancy(Guid id);

        public abstract bool insertCmsVacancy(VacancyModel vacancy);
        public abstract bool updateCmsVacancy(VacancyModel vacancy);
        public abstract bool deleteCmsVacancy(Guid id);

        //Orgs
        public abstract OrgsModel[] getOrgs(FilterParams filtr);
        public abstract OrgsModel getOrgItem(Guid id);
        public abstract bool insOrgs(Guid id, OrgsModel model, Guid UserId, String IP);
        public abstract bool setOrgs(Guid id, OrgsModel model, Guid UserId, String IP);
        public abstract bool delOrgs(Guid id, Guid UserId, String IP);
        public abstract bool sortOrgs(Guid id, int new_num);

        public abstract StructureModel[] getStructureList(Guid id);
        public abstract StructureModel getStructure(Guid id);
        public abstract bool insStructure(Guid id, Guid OrgId, StructureModel insert, Guid UserId, String IP);
        public abstract bool setStructure(Guid id, StructureModel insert, Guid UserId, String IP);
        public abstract bool delStructure(Guid id, Guid UserId, String IP);
        public abstract bool sortStructure(Guid id, int new_num);

        public abstract bool insOvp(Guid IdStructure, Guid OrgId, StructureModel insertStructure, Guid UserId, String IP);
        public abstract bool setOvp(Guid IdStructure, StructureModel updStructure, Guid UserId, String IP);

        public abstract Departments[] getDepartmentsList(Guid id);
        public abstract Departments getDepartamentItem(Guid id);
        public abstract DepartmentsPhone[] getDepartmentsPhone(Guid id);
        public abstract BreadCrumb[] getBreadCrumbOrgs(Guid id, string type);
        public abstract bool insDepartmentsPhone(Guid idDepart, string Label, string Value, Guid UserId, String IP);
        public abstract bool delDepartmentsPhone(int id);
        public abstract bool sortDepartament(Guid id, int new_num);
        public abstract People[] getPeopleDepartment(Guid idDepart);
        public abstract bool insDepartament(Guid id, Guid Structure, Departments insert, Guid UserId, String IP);
        public abstract bool updDepartament(Guid id,  Departments update, Guid UserId, String IP);
        public abstract bool delDepartament(Guid id, Guid UserId, String IP);
        public abstract People[] getPersonsThisDepartment(Guid idStructure);
        public abstract bool insPersonsThisDepartment(Guid idDepart, Guid IdLinkPeopleForOrg, string status, string post);
        public abstract bool delPersonsThisDepartment(Guid idDep, Guid idPeople);
        //Feedbacks
        public abstract FeedbacksList getFeedbacksList(FilterParams filtr);
        public abstract FeedbackModel getFeedback(Guid id);
        
        public abstract bool insertCmsFeedback(FeedbackModel eventData);
        public abstract bool updateCmsFeedback(FeedbackModel eventData);
        public abstract bool deleteCmsFeedback(Guid id);
        

        // Карта сайта
        public abstract SiteMapList getSiteMapList(string site, FilterParams filtr);
        public abstract SiteMapModel getSiteMapItem(Guid id);
        public abstract string[] getSiteMapGroupMenu(Guid id);
        public abstract int getCountSiblings(Guid id);
        public abstract bool checkSiteMap(Guid id);
        public abstract bool createSiteMapItem(Guid id, SiteMapModel item, Guid userId, string IP);
        public abstract bool updateSiteMapItem(Guid id, SiteMapModel item, Guid userId, string IP);
        public abstract SiteMapMenu[] getSiteMapFrontSectionList();
        public abstract SiteMapMenu getSiteMapMenu(Guid id);
        public abstract Catalog_list[] getSiteMapMenuTypes();
        public abstract bool createOrUpdateSiteMapMenu(SiteMapMenu item);
        public abstract bool deleteSiteMapItem(Guid id, Guid userId, string IP);
        public abstract SiteMapModel[] getSiteMapChildrens(Guid parent);
        public abstract BreadCrumbSiteMap[] getSiteMapBreadCrumbs(Guid? id);
        public abstract BreadCrumbSiteMap getSiteMapBreadCrumbItem(Guid id);
        public abstract bool permit_SiteMap(Guid id, int permit, string domain, string menuSort);

        // Баннеры
        public abstract BannersSectionModel[] getBannerSections(string domain);
        public abstract BannersSectionModel getBannerSection(Guid id, string domain, FilterParams filter);
        public abstract int getCountBannersBySectionAndDomain(Guid section, string domain);
        public abstract BannersListModel getBanners(Guid section, string domain, FilterParams filter);
        public abstract BannersModel getBanner(Guid id);
        public abstract bool checkBannerExist(Guid id);
        public abstract bool createBanner(Guid id, BannersModel item, Guid userId, string IP);
        public abstract bool updateBanner(Guid id, BannersModel item, Guid userId, string IP);
        public abstract bool deleteBanner(Guid id, Guid userId, string IP);
        public abstract bool permit_Banners(Guid id, int permit, string domain);
    }
}