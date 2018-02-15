using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using cms.dbModel.entity.cms;
using LinqToDB;

namespace cms.dbase.Mapping
{
    internal static class DbQueryMap
    {
        internal static IQueryable<People> MapSearch(this IQueryable<content_people> query, IQueryable<cms_sites> sites)
        {
            return query
                    .Where(w => w.employeespostspeoples.Any(b => b.employeespostsspecializations.b_doctor))
                    .Where(w => w.contentpeopleorglinks.Any(a => !a.b_dismissed))
                    .OrderBy(o => o.c_surname)
                    .Select(s => new People
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                        Photo = s.c_photo,
                        Posts = s.employeespostspeoples.Select(p => new EmployeePost
                        {
                            Id = p.f_post,
                            Name = p.employeespostsspecializations.c_name,
                            Type = p.n_type,
                            Org = new OrgsShortModel
                            {
                                Id = p.contentemployeespostsorgs.id,
                                Title = p.contentemployeespostsorgs.c_title,
                                Url = sites.Where(w => w.f_content.Equals(p.contentemployeespostsorgs.id))
                                           .Select(r => r.fksitesdomainss.FirstOrDefault().c_domain)
                                           .SingleOrDefault()
                            }
                        })
                    });
        }
    }
}
