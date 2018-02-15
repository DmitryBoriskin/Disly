using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;
using cms.dbModel.entity.cms;

namespace cms.dbase
{
    /// <summary>
    /// Репозитория для работы с главным специалистом
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Получим список должностей сотрудников
        /// </summary>
        /// <returns></returns>
        public override EmployeePost[] getEmployeePosts()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_specializationss
                    .Where(w => w.b_doctor)
                    .OrderBy(o => o.id)
                    .Select(s => new EmployeePost
                    {
                        Id = s.id,
                        Parent = s.n_parent,
                        Name = s.c_name
                    });

                if (!query.Any()) return null;
                else return query.ToArray();
            }
        }

        /// <summary>
        /// Получим должность сотрудников
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override EmployeePost getEmployeePost(int id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_specializationss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new EmployeePost
                    {
                        Id = s.id,
                        Parent = s.n_parent,
                        Name = s.c_name
                    });

                if (!query.Any()) return null;
                else return query.SingleOrDefault();
            }
        }

        /// <summary>
        /// Получим список главных специалистов
        /// </summary>
        /// <returns></returns>
        public override GSList getGSList(GSFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_gss.AsQueryable();

                int AllCount = 0;
                if (query.Any()) AllCount = query.Count();

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query
                        .Where(w => w.c_name.ToLower()
                        .Contains(filter.SearchText.ToLower()));
                }

                if (filter.RelId.HasValue && filter.RelId.Value != Guid.Empty)
                {
                    var spec = db.content_content_links
                                    .Where(p => p.f_content == filter.RelId.Value)
                                    .Where(p => p.f_content_type == filter.RelType.ToString().ToLower())
                                    .Where(p => p.f_link_type == ContentLinkType.SPEC.ToString().ToLower())
                                    .Select(p => p.f_link);

                    if (spec.Any())
                    {
                        query = query.Where(s => spec.Contains(s.id));
                    }
                    else
                        return null;

                }

                var itemCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .OrderBy(o => o.c_name)
                    .Select(s => new GSModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Desc = s.c_desc
                    });

                if (!list.Any()) return null;
                else
                {
                    return new GSList()
                    {
                        Data = list.ToArray(),
                        Pager = new Pager()
                        {
                            Page = filter.Page,
                            Size = filter.Size,
                            ItemsCount = itemCount,
                            //PageCount = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                        },
                        AllCount = AllCount
                    };
                }
            }
        }

        /// <summary>
        /// Единичная запись главного специалиста
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override GSModel getGSItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_gss
                    .Where(w => w.id.Equals(id))
                    .Select(
                     s =>
                      new GSModel
                      {
                          Id = s.id,
                          Title = s.c_name,
                          Desc = s.c_desc,
                          SiteId = (from site in db.cms_sitess
                                    join cms in db.content_gss on site.f_content equals cms.id
                                    where cms.id.Equals(s.id)
                                    select site.id).SingleOrDefault(),
                          Specialisations = s.gsspecialisationsgss.Where(sp => sp.f_gs == id).Any() ?
                                                s.gsspecialisationsgss
                                                        .Where(sp => sp.f_gs == id)
                                                        .Select(sp => sp.f_specialisation)
                                                        .ToArray()
                                                : null,
                          //(from l in db.content_main_specialist_specialisationss
                          // join m in db.content_main_specialistss
                          // on l.f_main_specialist equals m.id
                          // where l.f_main_specialist.Equals(s.id)
                          // select l.f_specialisation).ToArray(),
                          //Получаем в контроллере 
                          //EmployeeMainSpecs = (from l in db.content_main_specialist_peoples
                          //                     join m in db.content_main_specialistss
                          //                     on l.f_main_specialist equals m.id
                          //                     where (l.f_main_specialist.Equals(s.id) && l.f_type.Equals("main"))
                          //                     select l.f_people).ToArray(),

                          //Получаем в контроллере 
                          //EmployeeExpSoviet = (from l in db.content_main_specialist_peoples
                          //                     join m in db.content_main_specialistss
                          //                     on l.f_main_specialist equals m.id
                          //                     where (l.f_main_specialist.Equals(s.id) && l.f_type.Equals("soviet"))
                          //                     select l.f_people).ToArray()

                      });

                if (!query.Any()) return null;
                else return query.SingleOrDefault();
            }
        }

        /// <summary>
        /// Список врачей в гс по типу
        /// </summary>
        /// <param name="mainSpecialistId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override People[] getGSMembers(Guid mainSpecialistId, GSMemberType type)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_gs_memberss
                            .Where(p => p.f_gs == mainSpecialistId)
                            .Where(p => p.f_type == type.ToString().ToLower())
                            .Select(p => new People()
                            {
                                IdLinkGS = p.id,
                                Id = p.f_people,
                                FIO = String.Format("{0} {1} {2}", p.gsmemberspeople.c_surname, p.gsmemberspeople.c_name, p.gsmemberspeople.c_patronymic),
                               
                                //Дописать
                            });
                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        

        /// <summary>
        /// Получаем список сотрудников для выпадающих списков
        /// </summary>
        /// <returns></returns>
        public override EmployeeModel[] getEmployeeList(int[] specialisations = null)
        {
            using (var db = new CMSdb(_context))
            {

                var query = db.cms_content_sv_employee_postss.AsQueryable();

                if(specialisations != null && specialisations.Count() > 0)
                {
                    query = query.Where(w => w.f_post != null && specialisations.Contains(w.f_post.Value));
                }

#warning Исправить в базе (Guid)s.id

                var data = query
                    .OrderBy(o => o.c_surname)
                    .ThenBy(o => o.c_name)
                    .ThenBy(o => o.c_patronymic)
                    .Select(s => new EmployeeModel
                    {
                        Id = (Guid)s.id, 
                        PeopleId = s.f_people,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic
                        
                    });

                if (query.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Создаём главного специалиста
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool createGS(GSModel item)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_gss
                    .Value(v => v.id, item.Id)
                    .Value(v => v.c_name, item.Title)
                    .Value(v => v.c_desc, item.Desc)
                    .Insert();

                // добавляем специализации
                if (item.Specialisations != null)
                {
                    foreach (var sp in item.Specialisations)
                    {
                        db.content_gs_specialisationss
                            .Value(v => v.f_gs, item.Id)
                            .Value(v => v.f_specialisation, sp)
                            .Insert();
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Обновляем главного специалиста
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool updateGS(GSModel item)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_gss
                    .Where(w => w.id.Equals(item.Id))
                    .Set(u => u.c_name, item.Title)
                    .Set(u => u.c_desc, item.Desc)
                    .Update();

                // подчищаем таблицу перед обновлением
                db.content_gs_memberss
                    .Where(w => w.f_gs.Equals(item.Id))
                    .Delete();

                return true;
            }
        }

        /// <summary>
        /// Удаляем главного специалиста
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool deleteGS(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_gss
                    .Where(w => w.id.Equals(id));

                if (!query.Any()) return false;
                else
                {
                    var mainSp = query.SingleOrDefault();

                    db.content_gss
                        .Where(w => w.id.Equals(id))
                        .Delete();

                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Materials,
                        Action = LogAction.delete,
                        PageId = mainSp.id,
                        PageName = mainSp.c_name,
                        UserId = _currentUserId,
                        IP = _ip,
                    };

                    insertLog(log);

                    return true;
                }
            }
        }
        /// <summary>
        /// Добавляем врача в гс
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool addGSMember(GSMemberModel item)
        {
            // добавляем доктора в гс
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var newMemberId = Guid.NewGuid();
                    db.content_gs_memberss
                                .Value(v => v.id, newMemberId)
                                .Value(v => v.f_gs, item.Id)
                                .Value(v => v.f_people, item.PeopleId)
                                .Value(v => v.f_type, item.MemberType.ToString().ToLower())
                                .Insert();


                    if (item.Employee != null && item.Employee.Orgs != null && item.Employee.Orgs.Count() > 0)
                    {
                        foreach (var org in item.Employee.Orgs)
                        {
                            var orgId = (org.Id != Guid.Empty) ? org.Id : (Guid?)null;
                            var cdContact = new content_gs_members_contacts()
                            {
                                id = Guid.NewGuid(),
                                f_gs_member = newMemberId,

                                //Записываем либо orgId, либо введенные вручную данные
                                f_org = orgId,
                                c_org_title = (orgId == null) ? org.Title : null,
                                c_org_address = (orgId == null) ? org.Address : null,
                                c_org_phone = (orgId == null) ? org.Phone : null,
                                c_org_fax = (orgId == null) ? org.Fax : null,
                                c_org_site = (orgId == null) ? org.Url : null
                            };
                            db.Insert(cdContact);
                        }
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Удаляем врача из гс
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool deleteGSMember(Guid id)
        {
            // добавляем доктора в гс
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    db.content_gs_members_contactss
                            .Where(v => v.f_gs_member == id)
                            .Delete();

                    db.content_gs_memberss
                            .Where(v => v.id == id)
                            .Delete();

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Получаем идентификатор сайта главного специалиста
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override Guid? getGSLinkByDomain(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from s in db.cms_sitess
                             join ms in db.content_gss on s.f_content equals ms.id
                             where s.c_alias.ToLower().Equals(domain)
                             select ms.id);

                if (!query.Any()) return null;
                return query.SingleOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtr"></param>
        /// <returns></returns>
        public override GSShortModel[] getGSWithCheckedFor(GSFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_gss
                                .Where(s => s.id != filtr.RelId); //Исключаем само гс

                if (filtr.RelId.HasValue && filtr.RelId.Value != Guid.Empty)
                {
                    var List = query
                    .Select(s => new GSShortModel()
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Checked = ContentLinkExists(filtr.RelId.Value, filtr.RelType, s.id, ContentLinkType.SPEC),
                        Origin = ContentLinkOrigin(filtr.RelId.Value, filtr.RelType, s.id, ContentLinkType.SPEC)
                    });

                    if (List.Any())
                        return List.ToArray();
                }

                return null;

            }
        }
    }
}
