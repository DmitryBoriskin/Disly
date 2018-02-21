using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;
using System.Web;
using System.Collections.Generic;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы со статистикой
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        public override List<StatisticMaterial> getStatisticMaterials(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {   
                var query = db.get_Statistics(filter.Date, filter.DateEnd).AsQueryable().Select(s => new StaticticMaterialGroup()
                {
                    OrgId = s.f_link,
                    Rubric = s.c_alias,
                    Count = s.countgroup
                }).ToArray();

                var Orgs = db.content_orgss
                    .OrderBy(o=>o.c_title)
                    .Select(s=>new OrgsModel() {
                             Title=(s.c_title_short!=null)? s.c_title_short : s.c_title,
                             Id=s.id,                             
                         }).ToArray();

                List<StatisticMaterial> StatMaterial=new List<StatisticMaterial>();
                foreach (var item in Orgs)
                {
                    var obj = query.Where(w => w.OrgId == item.Id);

                    int? _all = (obj.Any())? obj.Select(e => e.Count).Sum():0;
                    int _CountActual = (int)((obj.Where(w => w.Rubric == "actual").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "actual").SingleOrDefault().Count : 0);
                    int _CountNews = (int)((obj.Where(w => w.Rubric == "news").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "news").SingleOrDefault().Count:0);
                    int _CountSmi = (int)((obj.Where(w => w.Rubric == "smi").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "smi").SingleOrDefault().Count:0);
                    int _CountMasterClasses = (int)((obj.Where(w => w.Rubric == "master-classes").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "master-classes").SingleOrDefault().Count:0);
                    int _CountGuests = (int)((obj.Where(w => w.Rubric == "guests").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "guests").SingleOrDefault().Count:0);
                    int _CountEvents = (int)((obj.Where(w => w.Rubric == "events").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "events").SingleOrDefault().Count:0);
                    int _CountPhoto = (int)((obj.Where(w => w.Rubric == "photo").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "photo").SingleOrDefault().Count:0);
                    int _CountVideo = (int)((obj.Where(w => w.Rubric == "video").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "video").SingleOrDefault().Count:0);
                    int _CountNewInMedicin = (int)((obj.Where(w => w.Rubric == "new-in-medicine").SingleOrDefault()!=null)? obj.Where(w => w.Rubric == "new-in-medicine").SingleOrDefault().Count:0);

                    StatisticMaterial elem = new StatisticMaterial
                    {
                        Title = item.Title,
                        Domain= getSiteDefaultDomain(item.Id),
                        CountAll = (int)((_all!=null)? _all:0),
                        CountActual =_CountActual,
                        CountNews = _CountNews,
                        CountSmi = _CountSmi,
                        CountMasterClasses = _CountMasterClasses,
                        CountGuests = _CountGuests,
                        CountEvents = _CountEvents,
                        CountPhoto = _CountPhoto,
                        CountVideo = _CountVideo,
                        CountNewInMedicin = _CountNewInMedicin,
                    };
                    StatMaterial.Add(elem);
                }
                return StatMaterial;
            }                
        }        
        public override List<StatisticMaterial> getStatisticMaterialsGs(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.get_Statistics(filter.Date, filter.DateEnd).AsQueryable().Select(s => new StaticticMaterialGroup()
                {
                    OrgId = s.f_link,
                    Rubric = s.c_alias,
                    Count = s.countgroup
                }).ToArray();

                var Gs = db.content_gss
                    .OrderBy(o => o.c_name)
                    .Select(s => new OrgsModel()
                    {
                        Title = s.c_name,
                        Id = s.id,
                    }).ToArray();

                List<StatisticMaterial> StatMaterial = new List<StatisticMaterial>();
                foreach (var item in Gs)
                {
                    var obj = query.Where(w => w.OrgId == item.Id);

                    int? _all = (obj.Any()) ? obj.Select(e => e.Count).Sum() : 0;
                    int _CountActual = (int)((obj.Where(w => w.Rubric == "actual").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "actual").SingleOrDefault().Count : 0);
                    int _CountNews = (int)((obj.Where(w => w.Rubric == "news").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "news").SingleOrDefault().Count : 0);
                    int _CountSmi = (int)((obj.Where(w => w.Rubric == "smi").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "smi").SingleOrDefault().Count : 0);
                    int _CountMasterClasses = (int)((obj.Where(w => w.Rubric == "master-classes").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "master-classes").SingleOrDefault().Count : 0);
                    int _CountGuests = (int)((obj.Where(w => w.Rubric == "guests").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "guests").SingleOrDefault().Count : 0);
                    int _CountEvents = (int)((obj.Where(w => w.Rubric == "events").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "events").SingleOrDefault().Count : 0);
                    int _CountPhoto = (int)((obj.Where(w => w.Rubric == "photo").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "photo").SingleOrDefault().Count : 0);
                    int _CountVideo = (int)((obj.Where(w => w.Rubric == "video").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "video").SingleOrDefault().Count : 0);
                    int _CountNewInMedicin = (int)((obj.Where(w => w.Rubric == "new-in-medicine").SingleOrDefault() != null) ? obj.Where(w => w.Rubric == "new-in-medicine").SingleOrDefault().Count : 0);

                    StatisticMaterial elem = new StatisticMaterial
                    {
                        Title = item.Title,
                        Domain = getSiteDefaultDomain(item.Id),
                        CountAll = (int)((_all != null) ? _all : 0),
                        CountActual = _CountActual,
                        CountNews = _CountNews,
                        CountSmi = _CountSmi,
                        CountMasterClasses = _CountMasterClasses,
                        CountGuests = _CountGuests,
                        CountEvents = _CountEvents,
                        CountPhoto = _CountPhoto,
                        CountVideo = _CountVideo,
                        CountNewInMedicin = _CountNewInMedicin,
                    };
                    StatMaterial.Add(elem);
                }
                return StatMaterial;
            }
        }
        

        public override List<StatisticFeedBack> getStatisticFeedBack()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.statistic_feedbacks;
                if (query.Any())
                {
                    return query.Select(s=>new StatisticFeedBack() {                        
                        Title=s.c_title,
                        Domain= getSiteDefaultDomain(s.c_alias),
                        AppealCount=(int)s.appeal,
                        AppealPublish=(int)s.appealpublish,
                        AppealNoPublish=(int)s.appealnopublish,
                        RewiewCount=(int)s.review,
                        RewiewAnswerCount= (int)s.reviewanswer,
                        RewiewNoAnswerCount= (int)s.reviewnoanswer
                    } ).ToList();
                }
                return null;
            }
        }
            
        

    }
}
