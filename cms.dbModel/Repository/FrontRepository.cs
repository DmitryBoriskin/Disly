using cms.dbModel.entity;
using cms.dbModel.entity.cms;
using System;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class abstract_FrontRepository
    {
        public abstract string getSiteId(string domainUrl);
        public abstract SiteSectionModel getView(string siteSection); //string siteId,
        public abstract SitesModel getSiteInfo(); //string domain
        public abstract string getSiteDefaultDomain(string siteId);
        public abstract string getSiteDefaultDomainByContentId(Guid contentId);
        public abstract UsersModel[] getSiteAdmins();
        public abstract SiteMapModel[] getSiteMapList(); //string domain
        public abstract SiteMapModel[] getSiteMapListShort(string path); //string domain
        public abstract string[] getSiteMapGroupMenu(Guid id);

        //Redirect from old portal methods
        public abstract SitesModel getSiteInfoByOldId(int id);
        public abstract SiteMapModel getSiteMapByOldId(int id);
        public abstract MaterialsModel getMaterialsByOldId(int id);

        // Карта сайта
        public abstract SiteMapModel getSiteMap(string path, string alias); //, string domain
        public abstract SiteMapModel getSiteMap(string frontSection);
        public abstract string[] getSiteMapSiblings(string path);
        public abstract List<SiteMapModel> getSiteMapSiblingElements(string path);
        public abstract SiteMapModel[] getSiteMapChild(Guid ParentId);
        public abstract List<Breadcrumbs> getBreadCrumbCollection(string Url); //, string domain

        //Banners
        public abstract BannersModel[] getBanners(); //string domain
        public abstract BannersModel getBanner(Guid id);

        //Materials
        public abstract List<MaterialFrontModule> getMaterialsModule(); //string domain
        public abstract MaterialFrontModule getMaterialsImportant();
        public abstract MaterialsList getMaterialsList(MaterialFilter filtr);
        public abstract MaterialsModel getMaterialsItem(string year, string month, string day, string alias); //, string domain
        public abstract MaterialsGroup[] getMaterialsGroup();

        //Departments and structure
        public abstract StructureModel[] getStructureList();
        public abstract StructureModel[] getStructures(); //string domain
        public abstract DopAddres[] getDopAddresStructur(Guid StrucId);
        public abstract StructureModel getStructureItem(int num); //string domain,
        public abstract Departments[] getDepartmentsList(Guid StructureId);
        public abstract Departments getDepartmentsItem(Guid Id);
        public abstract Departments getOvpDepartaments(Guid id);

        //Persons
        public abstract People[] getPeopleList(PeopleFilter filter);
        public abstract People getPeopleItem(Guid id);
        public abstract string getPeopleSnils(Guid id);
        public abstract StructureModel[] getDeparatamentsSelectList(); //string domain
        public abstract PeoplePost[] getPeoplePosts();//string domain

        public abstract OrgsModel getOrgInfo(string siteId);
        public abstract string getOid();
        public abstract OrgsAdministrative[] getAdministrative(string domain);

        //лпу
        public abstract OrgType[] getOrgTypes();
        public abstract OrgFrontModel[] getOrgModels(Guid? type);
        public abstract OrgFrontModel[] getOrgsModel(string tab, string idtype);
        public abstract OrgsAdministrative getLeaderOrg(Guid OrgId);
        public abstract string spotDomainContent(Guid? ContentId);
        public abstract string getOrgTypeName(Guid id);
        public abstract DepartmentAffiliationModel[] getDepartmentAffiliations();
        public abstract string getAffiliationDepartment(Guid id);
        public abstract MedicalService[] getMedicalServices(string domain);
        public abstract OrgFrontModel[] getOrgPortalModels(Guid service);
        public abstract string getMedicalServiceTitle(Guid id);

        //врачи портала
        public abstract DoctorList getDoctorsList(FilterParams filter);

        //Обратная связь
        public abstract FeedbacksList getFeedbacksList(FilterParams filtr);
        public abstract FeedbackModel getFeedbackItem(Guid id);
        public abstract bool insertFeedbackItem(FeedbackModel feedback);
        public abstract bool updateFeedbackItem(FeedbackModel feedback);

        //
        public abstract AnketaModel getLastWorksheetItem();

        //Vote
        public abstract IEnumerable<VoteModel> getVote(string Ip); //string domain,
        public abstract VoteModel getVoteItem(Guid id, string Ip);
        public abstract VoteAnswer[] getVoteAnswer(Guid VoteId, string Ip);
        public abstract VoteStat getVoteStat(Guid AnswerId, Guid VoteId, string Ip);
        public abstract bool GiveVote(Guid VoteId, string[] AnswerId, string Ip);

        //Attached Documents
        public abstract DocumentsModel[] getAttachDocuments(Guid id);

        //события
        public abstract EventsList getEvents(FilterParams filter);
        public abstract EventsModel getEvent(int num, string alias);

        //фотоальбом
        public abstract PhotoModel[] getPhotoList(Guid id);

        //Главные специалисты

        public abstract MainSpecialistModel[] getMainSpecialistList(FilterParams filter);        

        public abstract MainSpecialistModel[] getMainSpecialistContacts();
        public abstract People[] getMainSpecialistMembers(PeopleFilter filter);
        public abstract OrgsModel getOrgItem(Guid id);
        
        
        public abstract MainSpecialistModel getMainSpecialistItem(Guid id);

        //вакансии
        public abstract VacanciesList getVacancy(FilterParams filter);
        public abstract VacancyModel getVacancyItem(Guid id);
    }
}