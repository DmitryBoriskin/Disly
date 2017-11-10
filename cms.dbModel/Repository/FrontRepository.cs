using cms.dbModel.entity;
using System;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class abstract_FrontRepository
    {
        public abstract string getSiteId(string Domain);
        public abstract string getView(string siteId, string siteSection);
        public abstract SitesModel getSiteInfo(string domain);
        public abstract SiteMapModel[] getSiteMapList(string domain);
        public abstract string[] getSiteMapGroupMenu(Guid id);
        public abstract BannersModel[] getBanners(string domain);

        public abstract SiteMapModel getSiteMap(string path, string alias, string domain);
        public abstract SiteMapModel[] getSiteMapChild(Guid ParentId);
        public abstract List<Breadcrumbs> getBreadCrumbCollection(string Url, string domain);

        public abstract MaterialsList getMaterialsList(FilterParams filtr);
        public abstract MaterialsModel getMaterialsItem(string year, string month, string day, string alias, string domain);
        public abstract MaterialsGroup[] getMaterialsGroup();

        public abstract StructureModel[] getStructures(string domain);
        public abstract StructureModel getStructureItem(string domain, int num);
        public abstract Departments[] getDepartmentsList(Guid StructureId);
        public abstract Departments getDepartmentsItem(Guid Id);

        public abstract List<MaterialFrontModule> getMaterialsModule(string domain);


        public abstract People[] getPeopleList(FilterParams filter);
        public abstract People getPeopleItem(Guid id, string domain);
    }
}