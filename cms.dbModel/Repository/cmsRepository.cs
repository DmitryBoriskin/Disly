using cms.dbModel.entity;
using cms.dbModel.entity.cms;
using System;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class abstract_cmsRepository
    {
        public abstract string getSiteId(string Domain);

        public abstract SitesModel getSite(Guid? Id);
        public abstract SitesModel getSite(string domain);

        // !!!! Далее для всех методов не нужно передавать  Guid user, string ip пользователя, изменившего запись
        //public abstract bool updateSiteInfo(SitesModel item, Guid user, string ip);
        public abstract bool updateSiteInfo(SitesModel item);

        // Работа с логами
        public abstract cmsLogModel[] getCmsUserLog(Guid UserId);
        public abstract cmsLogModel[] getCmsPageLog(Guid PageId);
        public abstract void insertLog(LogModel log); //старое Guid UserId, string IP, string Action, Guid PageId, string PageName, string Section, string Site);

        // CmsMenu
        public abstract bool check_cmsMenu(Guid id);
        public abstract bool check_cmsMenu(string alias);

        public abstract cmsMenuModel[] getCmsMenu(Guid user_id);
        public abstract cmsMenuItem[] getCmsMenuItems(string group_id, Guid user_id);
        public abstract cmsMenuItem getCmsMenuItem(Guid id);
        public abstract cmsMenuType[] getCmsMenuType();

        public abstract bool createCmsMenu(Guid id, cmsMenuItem Item);
        public abstract bool updateCmsMenu(Guid id, cmsMenuItem Item);
        public abstract bool deleteCmsMenu(Guid id);
        public abstract bool permit_cmsMenu(Guid id, int num);

        // Все сайты портала
        public abstract SitesList getSiteList(FilterParams filtr);
        public abstract SitesShortModel[] getSiteListWithCheckedForUser(SiteFilter filtr);
        public abstract bool check_Site(Guid id);
        public abstract bool insertSite(SitesModel ins);
        public abstract bool updateSite(Guid id, SitesModel ins);
        public abstract bool deleteSite(Guid id);
        public abstract Domain[] getSiteDomains(string SiteId);
        public abstract bool insertDomain(String SiteId, string NewDomain);
        public abstract bool deleteDomain(Guid id);
        public abstract string getSiteId(Guid ContentId);

        // Все пользователи портала
        public abstract UsersList getUsersList(FilterParams filtr);
        public abstract UsersModel getUser(Guid id);
        public abstract bool createUser(Guid id, UsersModel Item);
        public abstract bool updateUser(Guid id, UsersModel Item);
        public abstract bool deleteUser(Guid id);

        // Для работы с пользователями
        public abstract bool check_user(Guid id);
        public abstract bool check_user(string email);
        public abstract void check_usergroup(Guid id, string group);
        public abstract void changePassword(Guid id, string Salt, string Hash);
        public abstract bool updateUserSiteLinks(UserSiteLinkModel link);
        //Все доступные группы на портале - справочник
        public abstract Catalog_list[] getUsersGroupList();


        //Группа пользователей
        public abstract GroupModel getGroup(string alias);
        public abstract bool updateGroup(GroupModel group);
        public abstract bool deleteGroup(string alias);

        public abstract ResolutionsModel[] getGroupResolutions(string alias);
        public abstract bool updateGroupClaims(GroupClaims GroupClaim);


        // Материалы
        public abstract MaterialsGroup[] getAllMaterialGroups();
        public abstract MaterialsGroup[] getMaterialGroups(Guid materialId);

        public abstract MaterialsList getMaterialsList(MaterialFilter filtr);
        public abstract MaterialsModel getMaterial(Guid id, string domain);

        public abstract bool insertCmsMaterial(MaterialsModel material);
        public abstract bool updateCmsMaterial(MaterialsModel material);
        public abstract bool deleteCmsMaterial(Guid id);
        public abstract MaterialsGroup[] getMaterialsGroups();

        public abstract bool updateContentLinks(ContentLinkModel model);

        // Events
        public abstract EventsList getEventsList(EventFilter filtr);
        public abstract EventsShortModel[] getLastEventsListWithCheckedFor(EventFilter filtr);
        public abstract EventsModel getEvent(Guid id);

        public abstract bool updateCmsEvent(EventsModel eventData);
        public abstract bool insertCmsEvent(EventsModel eventData);
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
        public abstract OrgsList getOrgsList(OrgFilter filtr);
        public abstract OrgsModel[] getOrgs(OrgFilter filtr);
        public abstract OrgsModel getOrgItem(Guid id);

        public abstract OrgsShortModel[] getOrgsListWhithChekedFor(OrgFilter filtr);

        public abstract bool insertOrg(Guid id, OrgsModel model);
        public abstract bool updateOrg(Guid id, OrgsModel model);
        public abstract bool deleteOrg(Guid id);
        public abstract bool sortOrgs(Guid id, int new_num);

        public abstract StructureModel[] getStructureList(Guid id);
        public abstract StructureModel getStructure(Guid id);
        public abstract bool insertStructure(Guid id, Guid OrgId, StructureModel insert);
        public abstract bool updateStructure(Guid id, StructureModel insert);
        public abstract bool deleteStructure(Guid id);
        public abstract bool sortStructure(Guid id, int new_num);

        public abstract bool insOvp(Guid IdStructure, Guid OrgId, StructureModel insertStructure);
        public abstract bool setOvp(Guid IdStructure, StructureModel updStructure, Departments updDepart);

        public abstract Departments[] getDepartmentsList(Guid id);
        public abstract Departments getDepartamentItem(Guid id);
        public abstract DepartmentsPhone[] getDepartmentsPhone(Guid id);
        public abstract Breadcrumbs[] getBreadCrumbOrgs(Guid id, string type);
        public abstract bool insDepartmentsPhone(Guid idDepart, string Label, string Value);
        public abstract bool delDepartmentsPhone(int id);
        public abstract bool sortDepartament(Guid id, int new_num);
        public abstract People[] getPeopleDepartment(Guid idDepart);
        public abstract bool insDepartament(Guid id, Guid Structure, Departments insert);
        public abstract bool updDepartament(Guid id,  Departments update);
        public abstract bool delDepartament(Guid id);
        public abstract People[] getPersonsThisDepartment(Guid idStructure);
        public abstract bool insPersonsThisDepartment(Guid idDepart, Guid IdLinkPeopleForOrg, string status, string post);
        public abstract bool delPersonsThisDepartment(Guid idDep, Guid idPeople);

        public abstract OrgType[] getOrgTypesList(OrgTypeFilter filter);

        public abstract List<OrgType> getOrgByType(Guid id);
        public abstract OrgsModelSmall[] getOrgSmall(Guid id, Guid material);
        public abstract bool setCheckedOrgs(Guid id, Guid material);
        public abstract OrgsModelSmall[] getOrgAttachedToTypes(Guid id);

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
        public abstract bool createSiteMapItem(Guid id, SiteMapModel item);
        public abstract bool updateSiteMapItem(Guid id, SiteMapModel item);
        public abstract SiteMapMenu[] getSiteMapFrontSectionList();
        public abstract SiteMapMenu getSiteMapMenu(Guid id);
        public abstract Catalog_list[] getSiteMapMenuTypes();
        public abstract bool createOrUpdateSiteMapMenu(SiteMapMenu item);
        public abstract bool deleteSiteMapItem(Guid id);
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
        public abstract bool createBanner(Guid id, BannersModel item);
        public abstract bool updateBanner(Guid id, BannersModel item);
        public abstract bool deleteBanner(Guid id);
        public abstract bool permit_Banners(Guid id, int permit, string domain);

        //Разделы сайта
        public abstract SiteSectionList getSiteSectionList(FilterParams filtr);
        public abstract SiteSectionModel getSiteSectionItem(Guid id);
        public abstract bool deleteSiteSection(Guid id);
        public abstract bool updateSiteSection(SiteSectionModel upd);
        public abstract bool insertSiteSection(SiteSectionModel sitesection);

        // Главные специалисты
        public abstract EmployeePostModel[] getEmployeePosts();
        public abstract EmployeePostModel getEmployeePost(int id);
        public abstract MainSpecialistList getMainSpecialistList(FilterParams filter);
        public abstract MainSpecialistModel getMainSpecialistItem(Guid id);
        public abstract EmployeeModel[] getEmployeeList(int[] specialisations);
        public abstract bool createMainSpecialist(MainSpecialistModel item);
        public abstract bool updateMainSpecialist(MainSpecialistModel item);
        public abstract bool deleteMainSpecialist(Guid id);

        // Врачи
        public abstract EmployeeModel getEmployee(Guid id);
        public abstract bool updateEmployee(EmployeeModel item);

        //Опросы
        public abstract VoteList getVoteList(FilterParams filtr);
        public abstract VoteModel getVoteItem(Guid id, string domain);
        public abstract bool insVote(Guid id, VoteModel ins, string domain);
        public abstract bool updVote(Guid id, VoteModel ins);
        public abstract bool delVote(Guid id);
        public abstract bool insAnswer(Guid idVote, string Variant);
        public abstract bool permit_VoteAnswer(Guid id, Guid VoteId, int num);
        public abstract bool delVoteAnswer(Guid id);
    }
}