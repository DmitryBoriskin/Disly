using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Models
{
    public class OrgsViewModel : CoreViewModel
    {                
        public OrgsModel Item { get; set; }
        public OrgsModel[] OrgList { get; set; }
        public StructureModel StructureItem { get; set; }
        public Departments DepartmentItem { get; set; }

        public BreadCrumb[] BreadCrumbOrg { get; set; }
        public SelectList PeopleList { get; set; }
        public SelectList PeopleLStatus { get; set; }
    }
}