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
        public override EmployeePostModel[] getEmployeePosts()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_employee_postss
                    .Where(w => w.b_doctor)
                    .OrderBy(o => o.id)
                    .Select(s => new EmployeePostModel
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
        public override EmployeePostModel getEmployeePost(int id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_employee_postss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new EmployeePostModel
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
        public override MainSpecialistList getMainSpecialistList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_main_specialistss
                    .Select(s => s);

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query
                        .Where(w => w.c_name.ToLower()
                        .Contains(filter.SearchText.ToLower()));
                }

                var itemCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .OrderBy(o => o.c_name)
                    .Select(s => new MainSpecialistModel
                    {
                        Id = s.id,
                        Name = s.c_name,
                        Desc = s.c_desc
                    });

                if (!list.Any()) return null;
                else
                {
                    return new MainSpecialistList()
                    {
                        Data = list.ToArray(),
                        Pager = new Pager()
                        {
                            Page = filter.Page,
                            Size = filter.Size,
                            ItemsCount = itemCount,
                            //PageCount = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                        }
                    };
                }
            }
        }

        /// <summary>
        /// Единичная запись главного специалиста
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override MainSpecialistModel getMainSpecialistItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_main_specialistss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new MainSpecialistModel
                    {
                        Id = s.id,
                        Name = s.c_name,
                        Desc = s.c_desc,
                        SiteId = (from site in db.cms_sitess
                                  join cms in db.content_main_specialistss on site.f_content equals cms.id
                                  where cms.id.Equals(s.id)
                                  select site.id).SingleOrDefault(),
                        Specialisations = (from l in db.content_main_specialist_specialisations_links
                                           join m in db.content_main_specialistss
                                           on l.f_main_specialist equals m.id
                                           where l.f_main_specialist.Equals(s.id)
                                           select l.f_specialisation).ToArray(),
                        EmployeeMainSpecs = (from l in db.content_main_specialist_employees_links
                                             join m in db.content_main_specialistss
                                             on l.f_main_specialist equals m.id
                                             where (l.f_main_specialist.Equals(s.id) && l.f_type.Equals("main"))
                                             select l.f_people).ToArray(),
                        EmployeeExpSoviet = (from l in db.content_main_specialist_employees_links
                                             join m in db.content_main_specialistss
                                             on l.f_main_specialist equals m.id
                                             where (l.f_main_specialist.Equals(s.id) && l.f_type.Equals("soviet"))
                                             select l.f_people).ToArray()
                    });

                if (!query.Any()) return null;
                else return query.SingleOrDefault();
            }
        }

        /// <summary>
        /// Получаем список сотрудников для выпадающих списков
        /// </summary>
        /// <returns></returns>
        public override EmployeeModel[] getEmployeeList(int[] specialisations)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_content_sv_people_postss
                    .Where(w => specialisations.Contains(w.f_post))
                    .OrderBy(o => o.c_surname)
                    .ThenBy(o => o.c_name)
                    .ThenBy(o => o.c_patronymic)
                    .Select(s => new EmployeeModel
                    {
                        Id = s.id,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic
                    });

                if (!query.Any()) return null;
                else return query.ToArray();
            }
        }

        /// <summary>
        /// Получаем список всех врачей
        /// </summary>
        /// <returns></returns>
        public override EmployeeModel[] getEmployeeList()
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from p in db.content_peoples
                             join pepl in db.content_people_employee_posts_links on p.id equals pepl.f_people
                             join ep in db.content_employee_postss on pepl.f_post equals ep.id
                             orderby p.c_surname, p.c_name, p.c_patronymic
                             where ep.b_doctor
                             select new EmployeeModel
                             {
                                 Id = p.id,
                                 Surname = p.c_surname,
                                 Name = p.c_name,
                                 Patronymic = p.c_patronymic
                             }).Distinct();

                if (!query.Any()) return null;
                else return query.ToArray();
            }
        }

        /// <summary>
        /// Создаём главного специалиста
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool createMainSpecialist(MainSpecialistModel item)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_main_specialistss
                    .Value(v => v.id, item.Id)
                    .Value(v => v.c_name, item.Name)
                    .Value(v => v.c_desc, item.Desc)
                    .Insert();

                // добавляем специализации
                if (item.Specialisations != null)
                {
                    foreach (var sp in item.Specialisations)
                    {
                        db.content_main_specialist_specialisations_links
                            .Value(v => v.f_main_specialist, item.Id)
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
        public override bool updateMainSpecialist(MainSpecialistModel item)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_main_specialistss
                    .Where(w => w.id.Equals(item.Id))
                    .Set(u => u.c_name, item.Name)
                    .Set(u => u.c_desc, item.Desc)
                    .Update();

                // подчищаем таблицу перед обновлением
                db.content_main_specialist_employees_links
                    .Where(w => w.f_main_specialist.Equals(item.Id))
                    .Delete();

                // добавляем главных специалистов
                if (item.EmployeeMainSpecs != null)
                {
                    foreach (var m in item.EmployeeMainSpecs)
                    {
                        db.content_main_specialist_employees_links
                            .Value(v => v.f_main_specialist, item.Id)
                            .Value(v => v.f_people, m)
                            .Value(v => v.f_type, "main")
                            .Insert();
                    }
                }

                // добавление экспертного состава
                if (item.EmployeeExpSoviet != null)
                {
                    foreach (var s in item.EmployeeExpSoviet)
                    {
                        db.content_main_specialist_employees_links
                            .Value(v => v.f_main_specialist, item.Id)
                            .Value(v => v.f_people, s)
                            .Value(v => v.f_type, "soviet")
                            .Insert();
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Удаляем главного специалиста
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool deleteMainSpecialist(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_main_specialistss
                    .Where(w => w.id.Equals(id));

                if (!query.Any()) return false;
                else
                {
                    var mainSp = query.SingleOrDefault();

                    db.content_main_specialistss
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
        /// Получаем идентификатор сайта главного специалиста
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override Guid? getMainSpecLinkByDomain(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from s in db.cms_sitess
                             join ms in db.content_main_specialistss on s.f_content equals ms.id
                             where s.c_alias.Equals(domain)
                             select ms.id);

                if (!query.Any()) return null;
                return query.SingleOrDefault();
            }
        }
    }
}
