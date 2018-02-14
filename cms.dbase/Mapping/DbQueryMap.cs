using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using cms.dbModel.entity.cms;

namespace cms.dbase.Mapping
{
    internal static class DbQueryMap
    {
        internal static IQueryable<People> MapSearch(this IQueryable<content_people> q)
        {
            return q.Select(s => new People
            {
                Id = s.id,
                FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                Photo = s.c_photo,
                Posts = s.contentpeopleemployeepostslinkcontentpeoples.Select(ep => new PeoplePost
                {
                    Id = ep.contentpeopleemployeepostslinkcontentemployeeposts.id,
                    Name = ep.contentpeopleemployeepostslinkcontentemployeeposts.c_name,
                    Type = ep.n_type,
                    Org = new OrgsModel
                    {
                        Id = ep.contentpeopleemployeepostslinkcontentorgs.id,
                        Title = ep.contentpeopleemployeepostslinkcontentorgs.c_title
                        //Url = ep.contentpeopleemployeepostslinkcontentorgs.
                    }
                })
            });
        }
    }
}
