using System;
using System.Collections.Generic;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с организациями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {

        /// <summary>
        /// Строим запрос на основе фильтра
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filtr"></param>
        /// <returns></returns>
        private IQueryable<content_orgs> QueryByOrgFilter(CMSdb db, OrgFilter filtr)
        {
            var query = db.content_orgss
                               .OrderBy(o => o.n_sort)
                               .AsQueryable();

            if (!string.IsNullOrEmpty(filtr.Domain))
            {
#warning  Дописать потом что должно происходить!!!
            }

            if (!string.IsNullOrEmpty(filtr.SearchText))
            {
                query = query.Where(w => w.c_title.Contains(filtr.SearchText));
            }

            if (filtr.RelId.HasValue && filtr.RelId.Value != Guid.Empty)
            {
                //В таблице ищем связи оранизация - контент (новость/событие)
                var objctLinks = db.content_content_links
                    .Where(s => s.f_content == filtr.RelId.Value)
                    .Where(s => s.f_link_type == "org")
                    .Where(s => s.f_content_type == filtr.RelType.ToString().ToLower());

                if (!objctLinks.Any())
                    query = query.Where(o => o.id == Guid.Empty); //Делаем заранее ложный запрос
                else
                {
                    var objctsId = objctLinks.Select(o => o.f_link);
                    query = query.Where(o => objctsId.Contains(o.id));
                }
            }

            return query;
        }

        private bool ContentLinkExists(Guid contentId, ContentType contentType, Guid linkId, ContentLinkType linkType)
        {
            using (var db = new CMSdb(_context))
            {
                var links = db.content_content_links
                    .Where(s => s.f_content == contentId)
                    .Where(s => s.f_content_type == contentType.ToString().ToLower())
                    .Where(s => s.f_link == linkId)
                    .Where(s => s.f_link_type == linkType.ToString().ToLower())
                    .Select(s => s.f_link);
                if (links.Any())
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Постраничный список организаций
        /// </summary>
        /// <param name="filtr"></param>
        /// <returns></returns>
        public override OrgsList getOrgsList(OrgFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                //Получаем сформированный запрос по фильтру
                var query = QueryByOrgFilter(db, filtr);

                var itemCount = query.Count();

                var data = query.Select(s => new OrgsModel()
                {
                    Id = s.id,
                    Title = s.c_title,
                    Sort = s.n_sort,
                    Types = s.contentorgstypeslinkorgs.Select(t => t.f_type).ToArray()
                });
                if (!data.Any())
                    return null;

                return new OrgsList
                {
                    Data = data.ToArray(),
                    Pager = new Pager
                    {
                        page = filtr.Page,
                        size = filtr.Size,
                        items_count = itemCount,
                        page_count = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                    }
                };
            }
        }

        /// <summary>
        /// Получаем список организаций по фильтру
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>         
        public override OrgsModel[] getOrgs(OrgFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                //Получаем сформированный запрос по фильтру
                var query = QueryByOrgFilter(db, filtr);

                //data.OrderBy(o => o.n_sort); ХЗ почему эта строка нормально не сортирует

                var data = query.Select(s => new OrgsModel()
                {
                    Id = s.id,
                    Title = s.c_title,
                    Sort = s.n_sort,
                    Types = s.contentorgstypeslinkorgs.Select(t => t.f_type).ToArray()
                });
                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получаем полный список доступных организаций с отмеченными значениями(для кот есть связи для объекта)
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>         
        public override OrgsShortModel[] getOrgsListWhithChekedFor(OrgFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                //Получаем сформированный запрос по фильтру
                var query = db.content_orgss.AsQueryable();

                if(!string.IsNullOrEmpty(filtr.Domain))
                {
                    //query = query;
                }
                if(filtr.RelId.HasValue && filtr.RelId.Value != Guid.Empty)
                {
                    var data = query
                        .Select(s => new OrgsShortModel()
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Types = (s.contentorgstypeslinkorgs.Select(t => t.f_type).Any())?
                                s.contentorgstypeslinkorgs.Select(t => t.f_type).ToArray(): null,
                        Checked = ContentLinkExists(filtr.RelId.Value, filtr.RelType, s.id, ContentLinkType.ORG)
                    });

                    if (data.Any())
                        return data.ToArray();

                }

                return null;
            }
        }

        /// <summary>
        /// Получаем организацию
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override OrgsModel getOrgItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                Guid[] types = null;
                var getTypes = getOrgTypesList(new OrgTypeFilter { OrgId = id });
                if (getTypes != null)
                    types = getTypes.Select(t => t.Id).ToArray();

                var data = db.content_orgss.Where(w => w.id == id)
                    .Select(s => new OrgsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        ShortTitle = s.c_title_short,
                        Phone = s.c_phone,
                        PhoneReception = s.c_phone_reception,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        DirecorPost = s.c_director_post,
                        DirectorF = s.f_director,
                        Contacts = s.c_contacts,
                        Address = s.c_adress,
                        GeopointX = s.n_geopoint_x,
                        GeopointY = s.n_geopoint_y,
                        Structure = getStructureList(s.id),
                        Frmp = s.f_frmp,
                        Types = types
                    });

                if (!data.Any()) return null;
                else return data.FirstOrDefault();
            }
        }

        /// <summary>
        /// Добавляем организацию
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="model">Организация</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool insertOrg(Guid id, OrgsModel model)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_orgss.Where(w => w.id == id);
                    if (!data.Any())
                    {
                        int MaxSort = 0;
                        try
                        {
                            MaxSort = db.content_orgss.Max(m => m.n_sort);
                        }
                        catch { }
                        MaxSort++;
                        db.content_orgss
                            .Value(s => s.id, id)
                            .Value(s => s.n_sort, MaxSort)
                            .Value(s => s.c_title, model.Title)
                            .Value(s => s.c_title_short, model.ShortTitle)
                            .Value(s => s.c_phone, model.Phone)
                            .Value(s => s.c_phone_reception, model.PhoneReception)
                            .Value(s => s.c_fax, model.Fax)
                            .Value(s => s.c_email, model.Email)
                            .Value(s => s.c_director_post, model.DirecorPost)
                            .Value(s => s.f_director, model.DirectorF)
                            .Value(s => s.c_contacts, model.Contacts)
                            .Value(s => s.c_adress, model.Address)
                            .Value(s => s.n_geopoint_x, model.GeopointX)
                            .Value(s => s.n_geopoint_y, model.GeopointY)
                            .Value(s => s.f_frmp, model.Frmp)
                            .Insert();

                        // обновляем типы мед. учреждений
                        if (model.Types != null)
                        {
                            // удаляем старые типы
                            db.content_orgs_types_links.Where(w => w.f_org.Equals(id)).Delete();

                            foreach (var t in model.Types)
                            {
                                var maxSortQuery = db.content_orgs_types_links
                                    .Where(w => w.f_org.Equals(id)).Select(s => s.n_sort);

                                int maxSort = maxSortQuery.Any() ? maxSortQuery.Max() : 0;

                                db.content_orgs_types_links
                                    .Value(v => v.f_org, id)
                                    .Value(v => v.f_type, t)
                                    .Value(v => v.n_sort, maxSort + 1)
                                    .Insert();
                            }
                        }

                        //логирование
                        // insertLog(UserId, IP, "insert", id, String.Empty, "Orgs", model.Title);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Orgs,
                            Action = LogAction.insert,
                            PageId = id,
                            PageName = model.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Обновляем организацию
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="model">Организация</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool updateOrg(Guid id, OrgsModel model)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_orgss.Where(w => w.id == id);
                    if (data.Any())
                    {
                        data
                            .Set(s => s.c_title, model.Title)
                            .Set(s => s.c_title_short, model.ShortTitle)
                            .Set(s => s.c_phone, model.Phone)
                            .Set(s => s.c_phone_reception, model.PhoneReception)
                            .Set(s => s.c_fax, model.Fax)
                            .Set(s => s.c_email, model.Email)
                            .Set(s => s.c_director_post, model.DirecorPost)
                            .Set(s => s.f_director, model.DirectorF)
                            .Set(s => s.c_contacts, model.Contacts)
                            .Set(s => s.c_adress, model.Address)
                            .Set(s => s.n_geopoint_x, model.GeopointX)
                            .Set(s => s.n_geopoint_y, model.GeopointY)
                            .Set(s => s.f_frmp, model.Frmp)
                            .Update();

                        // обновляем типы мед. учреждений
                        if (model.Types != null)
                        {
                            // удаляем старые типы
                            db.content_orgs_types_links.Where(w => w.f_org.Equals(id)).Delete();

                            foreach (var t in model.Types)
                            {
                                var maxSortQuery = db.content_orgs_types_links
                                    .Where(w => w.f_org.Equals(id)).Select(s => s.n_sort);

                                int maxSort = maxSortQuery.Any() ? maxSortQuery.Max() : 0;

                                db.content_orgs_types_links
                                    .Value(v => v.f_org, id)
                                    .Value(v => v.f_type, t)
                                    .Value(v => v.n_sort, maxSort + 1)
                                    .Insert();
                            }
                        }
                        
                        //логирование
                        //insertLog(UserId, IP, "update", id, String.Empty, "Orgs", model.Title);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Orgs,
                            Action = LogAction.update,
                            PageId = id,
                            PageName = model.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
                
            }
        }

        /// <summary>
        /// Удаляем организацию
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool deleteOrg(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_orgss.Where(w => w.id == id);
                    if (data.Any())
                    {
                        string logTitle = data.FirstOrDefault().c_title;
                        int ThisSort = data.FirstOrDefault().n_sort;
                        db.content_orgss.Where(w => w.n_sort > ThisSort).Set(p => p.n_sort, p => p.n_sort - 1).Update();//смещение n_sort
                        data.Delete();

                        //логирование
                        //insertLog(UserId, IP, "delete", id, String.Empty, "Orgs", logTitle);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Orgs,
                            Action = LogAction.delete,
                            PageId = id,
                            PageName = logTitle,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Сортировка организаций
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="new_num">Новое значение сортировки</param>
        /// <returns></returns>
        public override bool sortOrgs(Guid id, int new_num)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var actual_num = db.content_orgss.Where(w => w.id == id).FirstOrDefault().n_sort;
                    if (new_num > actual_num)
                    {
                        db.content_orgss
                            .Where(w => (w.n_sort > actual_num && w.n_sort <= new_num))
                            .Set(p => p.n_sort, p => p.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_orgss
                            .Where(w => w.n_sort < actual_num && w.n_sort >= new_num)
                            .Set(p => p.n_sort, p => p.n_sort + 1)
                            .Update();
                    }
                    db.content_orgss
                        .Where(w => w.id == id)
                        .Set(s => s.n_sort, new_num)
                        .Update();

                    tran.Commit();

                    return true;
                }
            }
        }

        /// <summary>
        /// Получаем список структурных подразделений
        /// </summary>
        /// <param name="id">Организация</param>
        /// <returns></returns>
        public override StructureModel[] getStructureList(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_org_structures.Where(w => w.f_ord == id).OrderBy(o => o.n_sort);
                if (data.Any())
                {
                    var List = data
                                .Select(s => new StructureModel()
                                {
                                    Id = s.id,
                                    Title = s.c_title,
                                    Ovp = s.b_ovp
                                });
                    return List.ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// Получаем структурное подразделение
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override StructureModel getStructure(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_org_structures.Where(w => w.id == id);
                if (data.Any())
                {
                    return data.Select(s => new StructureModel
                    {
                        Id = s.id,
                        OrgId = s.f_ord,
                        Title = s.c_title,
                        Adress = s.c_adress,
                        GeopointX = s.n_geopoint_x,
                        GeopointY = s.n_geopoint_y,
                        Phone = s.c_phone,
                        PhoneReception = s.c_phone_reception,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Routes = s.c_routes,
                        Schedule = s.c_schedule,
                        DirecorPost = s.c_director_post,
                        Ovp = s.b_ovp,
                        Departments = getDepartmentsList(s.id)

                        //f_direcor
                    }).FirstOrDefault();
                }
                return null;
            }
        }

        /// <summary>
        /// Добавляем структуру
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="OrgId">Организация</param>
        /// <param name="insert">Структура</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool insertStructure(Guid id, Guid OrgId, StructureModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    int MaxSort = db.content_org_structures
                    .Where(w => w.f_ord == OrgId)
                    .Any() ? db.content_org_structures.Where(w => w.f_ord == OrgId).Max(m => m.n_sort) : 0;
                    MaxSort++;

                    db.content_org_structures
                      .Value(v => v.id, id)
                      .Value(v => v.n_sort, MaxSort)
                      .Value(v => v.f_ord, OrgId)
                      .Value(v => v.c_title, insert.Title)
                      .Value(v => v.c_adress, insert.Adress)
                      .Value(v => v.n_geopoint_x, insert.GeopointX)
                      .Value(v => v.n_geopoint_y, insert.GeopointY)
                      .Value(v => v.c_phone, insert.Phone)
                      .Value(v => v.c_phone_reception, insert.PhoneReception)
                      .Value(v => v.c_fax, insert.Fax)
                      .Value(v => v.c_email, insert.Email)
                      .Value(v => v.c_routes, insert.Routes)
                      .Value(v => v.c_schedule, insert.Schedule)
                      .Value(v => v.c_director_post, insert.DirecorPost)
                      .Value(v => v.f_director, insert.DirectorF)
                      .Insert();

                    //логирование
                    //insertLog(UserId, IP, "insert", id, String.Empty, "Orgs", insert.Title);
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Orgs,
                        Action = LogAction.insert,
                        PageId = id,
                        PageName = insert.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Обновляем структуру
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="insert">Структура</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool updateStructure(Guid id, StructureModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_org_structures.Where(w => w.id == id);
                    if (data.Any())
                    {
                        data
                        .Set(v => v.c_title, insert.Title)
                        .Set(v => v.c_adress, insert.Adress)
                        .Set(v => v.n_geopoint_x, insert.GeopointX)
                        .Set(v => v.n_geopoint_y, insert.GeopointY)
                        .Set(v => v.c_phone, insert.Phone)
                        .Set(v => v.c_phone_reception, insert.PhoneReception)
                        .Set(v => v.c_fax, insert.Fax)
                        .Set(v => v.c_email, insert.Email)
                        .Set(v => v.c_routes, insert.Routes)
                        .Set(v => v.c_schedule, insert.Schedule)
                        .Set(v => v.c_director_post, insert.DirecorPost)
                        .Set(v => v.f_director, insert.DirectorF)
                        .Update();

                        //логирование
                        //insertLog(UserId, IP, "update", id, String.Empty, "Orgs", insert.Title);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Orgs,
                            Action = LogAction.update,
                            PageId = id,
                            PageName = insert.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаляем структуру
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool deleteStructure(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_org_structures.Where(w => w.id == id);
                    Guid IdOrg = data.FirstOrDefault().f_ord;
                    int ThisSort = data.FirstOrDefault().n_sort;
                    string logTitle = data.FirstOrDefault().c_title;
                    if (data.Any())
                    {
                        data.Delete();
                        db.content_org_structures.Where(w => w.f_ord == IdOrg && w.n_sort > ThisSort)
                            .Set(p => p.n_sort, p => p.n_sort - 1)
                            .Update();//смещение n_sort

                        //логирование
                        //insertLog(UserId, IP, "delete", id, String.Empty, "Orgs", logTitle);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Orgs,
                            Action = LogAction.delete,
                            PageId = id,
                            PageName = logTitle,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Сортировка структуры
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="new_num">Новое значение сортировки</param>
        /// <returns></returns>
        public override bool sortStructure(Guid id, int new_num)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var thisdata = db.content_org_structures.Where(w => w.id == id).FirstOrDefault();
                    int actual_num = thisdata.n_sort;
                    Guid OrgId = thisdata.f_ord;
                    if (new_num > actual_num)
                    {
                        db.content_org_structures
                            .Where(w => w.f_ord == OrgId && w.n_sort > actual_num && w.n_sort <= new_num)
                            .Set(p => p.n_sort, p => p.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_org_structures
                            .Where(w => w.f_ord == OrgId && w.n_sort < actual_num && w.n_sort >= new_num)
                            .Set(p => p.n_sort, p => p.n_sort + 1)
                            .Update();
                    }
                    db.content_org_structures
                        .Where(w => w.f_ord == OrgId && w.id == id)
                        .Set(s => s.n_sort, new_num)
                        .Update();

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Добавляем ОВП
        /// </summary>        
        /// <returns></returns>
        public override bool insOvp(Guid IdStructure, Guid OrgId, StructureModel insertStructure)
        {
            using (var db = new CMSdb(_context))
            {
                content_org_structure cdStructur = db.content_org_structures.Where(w => w.id == IdStructure).SingleOrDefault();
                if (cdStructur != null)
                {
                    throw new Exception("Запись с таким Id уже существует");
                }
                int MaxSort = 0;
                try
                {
                    MaxSort = db.content_org_structures.Where(w => w.f_ord == OrgId).Max(m => m.n_sort);
                }
                catch { }
                MaxSort++;

                cdStructur = new content_org_structure
                {
                    id = IdStructure,
                    f_ord = OrgId,
                    n_sort = MaxSort,
                    c_title = insertStructure.Title,
                    c_adress = insertStructure.Adress,
                    c_phone = insertStructure.PhoneReception,
                    c_fax = insertStructure.Fax,
                    c_email = insertStructure.Email,
                    n_geopoint_x = insertStructure.GeopointX,
                    n_geopoint_y = insertStructure.GeopointY,
                    c_schedule = insertStructure.Schedule,
                    c_routes = insertStructure.Routes,
                    c_director_post = insertStructure.DirecorPost,
                    f_director = insertStructure.DirectorF,
                    b_ovp = true
                };

                content_departments cdDepart = new content_departments
                {
                    id = Guid.NewGuid(),
                    n_sort = 1,
                    f_structure = IdStructure,
                    c_title = insertStructure.Title,
                    c_adress = insertStructure.Adress
                };
                string logTitle = insertStructure.Title;
                using (var tran = db.BeginTransaction())
                {
                    db.Insert(cdStructur);
                    db.Insert(cdDepart);
                    tran.Commit();
                    
                    //логирование
                    //insertLog(UserId, IP, "insert", IdStructure, String.Empty, "Orgs", logTitle);
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Orgs,
                        Action = LogAction.insert,
                        PageId = IdStructure,
                        PageName = logTitle,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    return true;
                }
            }
        }

        /// <summary>
        /// Обновляем ОВП
        /// </summary>
        /// <param name="IdStructure">Идентификатор структура</param>
        /// <param name="updStructure">Структура</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool setOvp(Guid IdStructure, StructureModel updStructure)
        {
            using (var db = new CMSdb(_context))
            {
                content_org_structure cdStructur = db.content_org_structures.Where(w => w.id == IdStructure).SingleOrDefault();
                if (cdStructur == null)
                {
                    throw new Exception("Запись с таким Id не существует");
                }
                cdStructur.c_title = updStructure.Title;
                cdStructur.c_adress = updStructure.Adress;
                cdStructur.n_geopoint_x = updStructure.GeopointX;
                cdStructur.n_geopoint_y = updStructure.GeopointY;
                cdStructur.c_phone = updStructure.PhoneReception;
                cdStructur.c_fax = updStructure.Fax;
                cdStructur.c_email = updStructure.Email;
                cdStructur.c_schedule = updStructure.Schedule;
                cdStructur.c_routes = updStructure.Routes;
                cdStructur.c_director_post = updStructure.DirecorPost;
                cdStructur.f_director = updStructure.DirectorF;

                content_departments cdDepart = db.content_departmentss.Where(w => w.f_structure == IdStructure).FirstOrDefault();
                if (cdDepart == null)
                {
                    throw new Exception("У данного ОВП в базе не существует отдела");
                }
                cdDepart.c_title = updStructure.Title;
                using (var tran = db.BeginTransaction())
                {
                    db.Update(cdStructur);
                    db.Update(cdDepart);
                    tran.Commit();
                    //логирование
                    //insertLog(UserId, IP, "update", IdStructure, String.Empty, "Orgs", updStructure.Title);
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Orgs,
                        Action = LogAction.update,
                        PageId = IdStructure,
                        PageName = updStructure.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    return true;
                }
            }
        }

        /// <summary>
        /// Отделение
        /// </summary>
        /// <param name="id">идентификатор структурного подразделения</param>
        /// <returns>отделения входящие в струкутрное подразделенеи</returns>
        public override Departments[] getDepartmentsList(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_departmentss.Where(w => w.f_structure == id).OrderBy(o => o.n_sort);
                if (data.Any())
                {
                    return data.Select(s => new Departments()
                    {
                        Id = s.id,
                        Title = s.c_title
                    }).ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// Возвращает департамент, если подходящего значения нет. то возвращает пустую модель Departments
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Departments getDepartamentItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_departmentss.Where(w => w.id == id);
                if (data.Any())
                {
                    return data.Select(s => new Departments
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text=s.c_adress,
                        StructureF = s.f_structure,
                        Phones = getDepartmentsPhone(s.id),
                        Peoples = getPeopleDepartment(s.id)
                    }).First();
                }
                return null;
            }
        }

        /// <summary>
        /// Телефонные номера департамента
        /// </summary>
        /// <param name="id">идентификатор отделения</param>
        /// <returns></returns>
        public override DepartmentsPhone[] getDepartmentsPhone(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_departments_phones.Where(w => w.f_department == id);
                if (data.Any())
                    return data
                            .Select(s => new DepartmentsPhone()
                            {
                                Id = s.id,
                                Label = s.c_key,
                                Value = s.c_val
                            })
                                        .ToArray();
            }
            return null;
        }

        /// <summary>
        /// Хлебные крошки раздела Orgs
        /// </summary>
        /// <param name="id">идентификатор элемента относительно которого нужно построить хлебные крошки</param>
        /// <param name="type">тип раздела orgs- в качество него скорее всего будем брать action name</param>
        /// <returns></returns>
        public override Breadcrumbs[] getBreadCrumbOrgs(Guid id, string type)
        {
            using (var db = new CMSdb(_context))
            {
                var MyBread = new Stack<Breadcrumbs>();
                MyBread.Push(new Breadcrumbs
                {
                    Title = "Организации",
                    Url = "/admin/orgs/"
                });
                #region item
                if (type == "item")
                {
                    var data = db.content_departmentss.Where(w => w.id == id).FirstOrDefault();
                    MyBread.Push(new Breadcrumbs
                    {
                        Title = data.c_title,
                        Url = "/admin/orgs/item/" + data.id
                    });
                }
                #endregion
                #region structure
                if (type == "structure")
                {
                    var data = db.content_org_structures.Where(w => w.id == id).FirstOrDefault();
                    var ParentStructure = db.content_orgss.Where(w => w.id == data.f_ord).FirstOrDefault();

                    MyBread.Push(new Breadcrumbs
                    {
                        Title = ParentStructure.c_title,
                        Url = "/admin/orgs/item/" + ParentStructure.id
                    });
                    MyBread.Push(new Breadcrumbs
                    {
                        Title = data.c_title,
                        Url = "/admin/orgs/structure/" + data.id
                    });
                }
                #endregion
                #region ovp
                if (type == "ovp")
                {
                    var data = db.content_org_structures.Where(w => w.id == id).FirstOrDefault();
                    var ParentStructure = db.content_orgss.Where(w => w.id == data.f_ord).FirstOrDefault();

                    MyBread.Push(new Breadcrumbs
                    {
                        Title = ParentStructure.c_title,
                        Url = "/admin/orgs/item/" + ParentStructure.id
                    });
                    MyBread.Push(new Breadcrumbs
                    {
                        Title = data.c_title,
                        Url = "/admin/orgs/ovp/" + data.id
                    });
                }
                #endregion
                #region department
                if (type == "department")
                {
                    var data = db.content_departmentss.Where(w => w.id == id).FirstOrDefault();
                    var ParentStructure = db.content_org_structures.Where(w => w.id == data.f_structure).FirstOrDefault();
                    var ParentOrg = db.content_orgss.Where(w => w.id == ParentStructure.f_ord).FirstOrDefault();

                    MyBread.Push(new Breadcrumbs
                    {
                        Title = ParentOrg.c_title,
                        Url = "/admin/orgs/item/" + ParentOrg.id
                    });
                    MyBread.Push(new Breadcrumbs
                    {
                        Title = ParentStructure.c_title,
                        Url = "/admin/orgs/structure/" + ParentStructure.id
                    });
                    MyBread.Push(new Breadcrumbs
                    {
                        Title = data.c_title,
                        Url = "/admin/orgs/department/" + data.id
                    });
                }
                #endregion
                return MyBread.Reverse().ToArray();
            }
        }

        /// <summary>
        /// Добавляет значение в список телефонов отдела
        /// </summary>
        /// <param name="idDepart"></param>
        /// <param name="Label"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override bool insDepartmentsPhone(Guid idDepart, string Label, string Value)
        {
            using (var db = new CMSdb(_context))
            {
                int Sort = 1;
                var data = db.content_departments_phones.Where(w => w.f_department == idDepart);
                if (data.Any()) Sort = data.Max(m => m.n_sort) + 1;
                db.content_departments_phones
                   .Value(v => v.f_department, idDepart)
                   .Value(v => v.c_key, Label)
                   .Value(v => v.c_val, Value)
                   .Value(v => v.n_sort, Sort)
                   .Insert();
                
                //логирование
                //insertLog(UserId, IP, "insert_phone_depart", idDepart, String.Empty, "Orgs", Label);
                var log = new LogModel()
                {
                    Site = _domain,
                    Section = LogSection.Orgs,
                    Action = LogAction.insert,
                    PageId = idDepart,
                    PageName = Label,
                    UserId = _currentUserId,
                    IP = _ip,
                };
                insertLog(log);


                return true;
            }
        }

        /// <summary>
        /// Удаление телефона
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool delDepartmentsPhone(int id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_departments_phones.Where(w => w.id == id);

                if (data.Any())
                {
                    string logtitle = data.FirstOrDefault().c_val;
                    data.Delete();
                }
            }
            return true;
        }

        /// <summary>
        /// Получаем список сотрудников по департаменту
        /// </summary>
        /// <param name="idDepart">Департамент</param>
        /// <returns></returns>
        public override People[] getPeopleDepartment(Guid idDepart)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_people_departments
                           .Where(w => w.f_department == idDepart)
                           .Select(s => new People()
                           {
                               Id = s.id,
                               FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic
                               //,
                               //IdLinkOrg=s.
                           });
                if (data.Any()) return data.ToArray();
                return null;
            }
        }

        /// <summary>
        /// Добавляем департамент
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="Structure">Структура</param>
        /// <param name="insert">Департамент</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool insDepartament(Guid id, Guid Structure, Departments insert)
        {
            using (var db = new CMSdb(_context))
            {
                content_departments cdDepart = db.content_departmentss
                                                 .Where(p => p.id == id)
                                                 .SingleOrDefault();
                if (cdDepart != null)
                {
                    throw new Exception("Запись с таким Id уже существует");
                }
                int MaxSort = 0;
                try
                {
                    MaxSort = db.content_departmentss.Where(w => w.f_structure == Structure).Max(m => m.n_sort);
                }
                catch { }
                MaxSort++;

                cdDepart = new content_departments
                {
                    id = id,
                    f_structure = Structure,
                    c_title = insert.Title,
                    c_adress=insert.Text,
                    n_sort = MaxSort,
                    f_director=insert.DirectorF,
                    c_director_post= insert.DirecorPost
                };

                using (var tran = db.BeginTransaction())
                {
                    db.Insert(cdDepart);
                    tran.Commit();

                    //логирование
                    // insertLog(UserId, IP, "insert", id, String.Empty, "Site", insert.Title);
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Sites,
                        Action = LogAction.insert,
                        PageId = id,
                        PageName = insert.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);
                }
                return true;
            }
        }

        /// <summary>
        /// Обновляем департамент
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="insert">Департамент</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool updDepartament(Guid id, Departments insert)
        {
            using (var db = new CMSdb(_context))
            {
                content_departments cdDepart = db.content_departmentss
                                              .Where(p => p.id == id)
                                              .SingleOrDefault();
                if (cdDepart == null)
                {
                    throw new Exception("Запись с таким Id не существует");
                }
                cdDepart.c_title = insert.Title;
                cdDepart.c_adress = insert.Text;
                cdDepart.f_director = insert.DirectorF;
                cdDepart.c_director_post = insert.DirecorPost;

                using (var tran = db.BeginTransaction())
                {
                    db.Update(cdDepart);
                    tran.Commit();
                }
                return true;
            }
        }

        /// <summary>
        /// Удаляем департамент
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="UserId">Пользователь</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool delDepartament(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                content_departments cdDepart = db.content_departmentss
                                                .Where(p => p.id == id)
                                                .SingleOrDefault();
                if (cdDepart == null)
                {
                    throw new Exception("Запись с таким Id не найдена");
                }
                Guid IdStruct = cdDepart.f_structure;
                int ThisSort = cdDepart.n_sort;
                using (var tran = db.BeginTransaction())
                {
                    db.content_departmentss.Where(w => w.f_structure == IdStruct && w.n_sort > ThisSort).Set(p => p.n_sort, p => p.n_sort - 1).Update();//смещение n_sort
                    db.Delete(cdDepart);

                    tran.Commit();
                }
                return true;
            }
        }

        /// <summary>
        /// Сортируем департаменты
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="new_num">Новое значение сортировки</param>
        /// <returns></returns>
        public override bool sortDepartament(Guid id, int new_num)
        {
            using (var db = new CMSdb(_context))
            {
                var thisdata = db.content_departmentss.Where(w => w.id == id).FirstOrDefault();
                int actual_num = thisdata.n_sort;
                Guid OrgId = thisdata.f_structure;
                if (new_num > actual_num)
                {
                    db.content_departmentss
                        .Where(w => w.f_structure == OrgId && w.n_sort > actual_num && w.n_sort <= new_num)
                        .Set(p => p.n_sort, p => p.n_sort - 1)
                        .Update();
                }
                else
                {
                    db.content_departmentss
                        .Where(w => w.f_structure == OrgId && w.n_sort < actual_num && w.n_sort >= new_num)
                        .Set(p => p.n_sort, p => p.n_sort + 1)
                        .Update();
                }
                db.content_departmentss
                    .Where(w => w.f_structure == OrgId && w.id == id)
                    .Set(s => s.n_sort, new_num)
                    .Update();

                return true;
            }
        }

        /// <summary>
        /// Получаем список сотрудников по департаменту
        /// </summary>
        /// <param name="idDepar">Департамент</param>
        /// <returns></returns>
        public override People[] getPersonsThisDepartment(Guid idDepar)
        {
            using (var db = new CMSdb(_context))
            {
                var data_dep = db.content_departmentss.Where(w => w.id == idDepar);
                if (data_dep.Any())
                {
                    Guid idStructure = data_dep.First().f_structure;

                    var data_str = db.content_org_structures.Where(w => w.id == idStructure);
                    if (data_str.Any())
                    {
                        //нужно показать только персон не добавленных в отделение
                        Guid OrgId = data_str.First().f_ord;
                        var PeopleList = db.content_people_org_links
                                           .Where(w => w.f_org == OrgId)
                                           .Where(w => (w.fkcontentpeopleorgdepartmentlinks == null || w.fkcontentpeopleorgdepartmentlinks.FirstOrDefault().f_department != idDepar))
                                           .Select(s => new People
                                           {
                                               FIO = s.fkcontentpeopleorglink.c_surname + " " + s.fkcontentpeopleorglink.c_name + " " + s.fkcontentpeopleorglink.c_patronymic,
                                               Id = s.id
                                           }).ToArray();
                        return PeopleList.Any() ? PeopleList : null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Цепяет врача к департаменту(отделу)
        /// </summary>
        /// <param name="idDepart">id департамента(отдела)</param>
        /// <param name="IdLinkPeopleForOrg">идентификатор связи пользователя с организацией</param>
        /// <param name="status">стаус: старшая медсестра, начальник отделени ....</param>
        /// <param name="post">Должность</param>
        /// <returns></returns>
        public override bool insPersonsThisDepartment(Guid idDepart, Guid IdLinkPeopleForOrg, string status, string post)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    //проверям подключен ли данный пользователь к данному отделу
                    var data = db.content_people_department_links.Where(w => (w.f_department == idDepart && w.f_people == IdLinkPeopleForOrg));
                    if (!data.Any())
                    {
                        content_people_department_link newdata = new content_people_department_link
                        {
                            f_department = idDepart,
                            f_people = IdLinkPeopleForOrg,
                            c_status = status,
                            c_post = post
                        };
                        db.Insert(newdata);
                        tran.Commit();
                        return true;
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// Удаляем связь сотрудника из департамента
        /// </summary>
        /// <param name="idDep">Департамент</param>
        /// <param name="idPeople">Сотрудник</param>
        /// <returns></returns>
        public override bool delPersonsThisDepartment(Guid idDep, Guid idPeople)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_people_department_links.Where(w => w.f_department == idDep && w.f_people == idPeople);

                if (data.Any())
                {
                    data.Delete();
                }
            }
            return true;
        }
        
        public override OrgType[] getOrgTypesList(OrgTypeFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_orgs_typess.AsQueryable();
                             

                if (filter.Id.HasValue && filter.Id.Value != Guid.Empty)
                {
                    query = query.Where(s => s.id == filter.Id.Value);
                }

                if (filter.OrgId.HasValue && filter.OrgId.Value != Guid.Empty)
                {
                    query = query.Where(s => s.contentorgstypeslinkorgtypess.Any(o => o.f_org == filter.OrgId.Value));
                }

                var data = query
                    .OrderBy(s => s.n_sort)
                    .Select(s => new OrgType
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort
                    });

                if (!data.Any()) return null;
                else return data.ToArray();
            }
        }


         /// <summary>
        /// Получим список типов организаций с привязанными к ним организациями
        /// </summary>
        /// <returns></returns>
        public override List<OrgType> getOrgByType(Guid material)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_orgs_typess
                    .OrderBy(o => o.n_sort)
                    .Select(s => new OrgType
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort,
                        Orgs = getOrgSmall(s.id, material)
                    });

                if (!data.Any()) return null;
                else return data.ToList();
            }
        }

        /// <summary>
        /// Получим список организаций по типу
        /// </summary>
        /// <param name="id">Тип</param>
        /// <returns></returns>
        public override OrgsModelSmall[] getOrgSmall(Guid id, Guid material)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_orgs_by_types
                    .Where(w => w.f_type.Equals(id))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new OrgsModelSmall
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort,
                        Check = setCheckedOrgs(s.id, material)
                    });

                if (!data.Any()) return null;
                else return data.ToArray();
            }
        }

#warning удалить дублирование
        /// <summary>
        /// Отметим выбранные организации
        /// <param name="id">Идентификатор</param>
        /// </summary>
        public override bool setCheckedOrgs(Guid id, Guid material)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_content_links
                    .Where(w => w.f_content.Equals(material))
                    .Where(w => w.f_link.Equals(id));

                return data.Any();
            }
        }

        /// <summary>
        /// Получаем список организаций, прикреплённых к каким-то типам
        /// </summary>
        /// <returns></returns>
        public override OrgsModelSmall[] getOrgAttachedToTypes(Guid material)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_orgs_not_attacheds
                    .OrderBy(o => o.c_title)
                    .Select(s => new OrgsModelSmall
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Check = setCheckedOrgs(s.id, material)
                    });

                if (!data.Any()) return null;
                else return data.ToArray();
            }
        }
    }
}