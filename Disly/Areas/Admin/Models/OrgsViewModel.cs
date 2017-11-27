using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая организации во внешней части
    /// </summary>
    public class OrgsViewModel : CoreViewModel
    {                
        /// <summary>
        /// Единичная запись организации
        /// </summary>
        public OrgsModel Item { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public OrgsModel[] OrgList { get; set; }

        /// <summary>
        /// Структура
        /// </summary>
        public StructureModel StructureItem { get; set; }

        /// <summary>
        /// Департамент
        /// </summary>
        public Departments DepartmentItem { get; set; }

        /// <summary>
        /// Хлебные крошки
        /// </summary>
        public Breadcrumbs[] BreadCrumbOrg { get; set; }

        /// <summary>
        /// Список сотрудников
        /// </summary>
        public SelectList PeopleList { get; set; }

        /// <summary>
        /// Список статусов сотрудников
        /// </summary>
        public SelectList PeopleLStatus { get; set; }

        /// <summary>
        /// Типы организаций
        /// </summary>
        public OrgType[] Types { get; set; }
        /// <summary>
        /// административный персонал
        /// </summary>
        public  OrgsAdministrativ AdministrativItem { get; set; }

        /// <summary>
        /// Ведомственная принадлежность
        /// </summary>
        public DepartmentAffiliationModel[] DepartmentAffiliations { get; set; }
    }
}