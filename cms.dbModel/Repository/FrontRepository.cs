using cms.dbModel.entity;
using System;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class abstract_FrontRepository
    {
        public abstract string getSiteId(string domainUrl);
        public abstract string getView(string siteSection); //string siteId,
        public abstract SitesModel getSiteInfo(); //string domain
        public abstract SiteMapModel[] getSiteMapList(); //string domain
        public abstract SiteMapModel[] getSiteMapListShort(); //string domain
        public abstract string[] getSiteMapGroupMenu(Guid id);

        public abstract SiteMapModel getSiteMap(string path, string alias); //, string domain
        public abstract SiteMapModel[] getSiteMapChild(Guid ParentId);
        public abstract List<Breadcrumbs> getBreadCrumbCollection(string Url); //, string domain

        //Banners
        public abstract BannersModel[] getBanners(); //string domain
        public abstract BannersModel getBanner(Guid id);

        //Materials
        public abstract List<MaterialFrontModule> getMaterialsModule(); //string domain
        public abstract MaterialsList getMaterialsList(FilterParams filtr);
        public abstract MaterialsModel getMaterialsItem(string year, string month, string day, string alias); //, string domain
        public abstract MaterialsGroup[] getMaterialsGroup();

        //Departments and structure
        public abstract StructureModel[] getStructures(); //string domain
        public abstract StructureModel getStructureItem(int num); //string domain,
        public abstract Departments[] getDepartmentsList(Guid StructureId);
        public abstract Departments getDepartmentsItem(Guid Id);
        public abstract Departments getOvpDepartaments(Guid id);

        //Persons
        public abstract People[] getPeopleList(FilterParams filter);
        public abstract People getPeopleItem(Guid id);
        public abstract string getPeopleSnils(Guid id);
        public abstract StructureModel[] getDeparatamentsSelectList(); //string domain
        public abstract PeoplePost[] getPeoplePosts();//string domain

        public abstract OrgsModel getOrgInfo(); //string domain
        public abstract string getOid(string domain);
        public abstract OrgsAdministrative[] getAdministrative(string domain);

        //лпу
        public abstract OrgType[] getOrgTypes();
        public abstract OrgFrontModel[] getOrgModels(Guid? type);
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
        public abstract bool insertFeedbackItem(FeedbackModel feedback);

        //Vote
        public abstract IEnumerable<VoteModel> getVote(string Ip); //string domain,
        public abstract VoteModel getVoteItem(Guid id, string Ip);
        public abstract VoteAnswer[] getVoteAnswer(Guid VoteId, string Ip);
        public abstract VoteStat getVoteStat(Guid AnswerId, Guid VoteId, string Ip);
        public abstract bool GiveVote(Guid VoteId, string[] AnswerId, string Ip);

        //Attached Documents
        public abstract DocumentsModel[] getAttachDocuments(Guid id);
    }
}