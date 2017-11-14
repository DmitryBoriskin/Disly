using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace cms.dbase
{
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        private Guid _currentUserId = Guid.Empty;
        private string _ip = string.Empty;
        private string _domain = string.Empty;

        /// <summary>
        /// Конструктор
        /// </summary>
        public cmsRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }
        public cmsRepository(string ConnectionString, Guid UserId, string IP, string DomainUrl)
        {
            _context = ConnectionString;
            _domain = (!string.IsNullOrEmpty(DomainUrl))? getSiteId(DomainUrl): "";
            _ip = IP;
            _currentUserId = UserId;

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


        #region private methods of class

        // Определяем - это сайт организации, события или персоны
        private SiteContentType db_getDomainContentTypeId(CMSdb db, string domain)
        {
            try
            {
                var linkIdData = db.cms_sitess.Where(d => d.c_alias.Equals(domain)).SingleOrDefault();
                if (linkIdData != null)
                {
                    return new SiteContentType()
                    {
                        Id = linkIdData.f_content,
                        CType = linkIdData.c_content_type
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("cms_sites: Обнаружено более одной записи у поля, которое в принципе не может быть не уникальным!!!" + ex);
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Получаем id сайта по доменному имени
        /// </summary>
        /// <returns></returns>
        public override string getSiteId(string Domain)
        {
            using (var db = new CMSdb(_context))
            {
                string SiteId = String.Empty;

                var data = db.cms_sites_domainss.Where(w => w.c_domain == Domain).FirstOrDefault();

                SiteId = data.f_site;

                return SiteId;
            }
        }

        /// <summary>
        /// Данные сайта по id или доменному имени
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public override SitesModel getSite(Guid? Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.id == Id)
                    .Select(s => new SitesModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        LongTitle = s.c_name_long,
                        Alias = s.c_alias,
                        Adress = s.c_adress,
                        Phone = s.c_phone,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Site = s.c_url,
                        Worktime = s.c_worktime,
                        //Logo = s.c_logo,
                        Logo = new Photo
                        {
                            Url = s.c_logo
                        },
                        DomainList = getSiteDomains(s.c_alias),
                        ContentId = (Guid)s.f_content,
                        Type = s.c_content_type,
                        SiteOff = s.b_site_off
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        public override SitesModel getSite(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.c_alias == domain)
                    .Select(s => new SitesModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        LongTitle = s.c_name_long,
                        Alias = s.c_alias,
                        Adress = s.c_adress,
                        Phone = s.c_phone,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Site = s.c_url,
                        Worktime = s.c_worktime,
                        //Logo = s.c_logo,
                        Logo = new Photo
                        {
                            Url = s.c_logo
                        },
                        ContentId = (Guid)s.f_content,
                        Type = s.c_content_type,
                        Facebook = s.c_facebook,
                        Vk = s.c_vk,
                        Instagramm = s.c_instagramm,
                        Odnoklassniki = s.c_odnoklassniki,
                        Twitter = s.c_twitter
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        /// <summary>
        /// Обновляется информация по сайту
        /// </summary>
        /// <param name="item">модель сайта</param>
        /// <returns></returns>
        public override bool updateSiteInfo(SitesModel item, Guid user, string ip)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var query = db.cms_sitess
                       .Where(w => w.id.Equals(item.Id));

                    if (query.Any())
                    {
                        db.cms_sitess
                            .Where(w => w.id.Equals(item.Id))
                            .Set(u => u.c_name, item.Title)
                            .Set(u => u.c_name_long, item.LongTitle)
                            .Set(u => u.c_alias, item.Alias)
                            .Set(u => u.c_facebook, item.Facebook)
                            .Set(u => u.c_vk, item.Vk)
                            .Set(u => u.c_instagramm, item.Instagramm)
                            .Set(u => u.c_odnoklassniki, item.Odnoklassniki)
                            .Set(u => u.c_twitter, item.Twitter)
                            .Set(s => s.c_logo, item.Logo.Url)
                            .Update();

                        //insertLog(user, ip, "update", item.Id, String.Empty, "Sites", item.Title);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Sites,
                            Action = LogAction.update,
                            PageId = item.Id,
                            PageName = item.Title,
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

        #region Работа с логами
        public override cmsLogModel[] getCmsUserLog(Guid UserId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sv_log_userss.
                    Where(w => w.f_user == UserId).
                    OrderByDescending(o => o.d_date).
                    Select(s => new cmsLogModel
                    {
                        PageId = s.f_page,
                        PageName = s.c_page_name,
                        Site = s.f_site,
                        Section = s.f_section,
                        Date = s.d_date,
                        Action = s.c_action_name
                    }).
                    Take(100);

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override cmsLogModel[] getCmsPageLog(Guid PageId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sv_logs.
                    Where(w => w.f_page == PageId).
                    Select(s => new cmsLogModel
                    {
                        PageId = s.id,
                        UserId = s.f_user,
                        Date = s.d_date,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Action = s.c_action_name
                    })
                    .OrderByDescending(o => o.Date);

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override void insertLog(LogModel log)
        {
            using (var db = new CMSdb(_context))
            {
                db.cms_logs.Insert(() => new cms_log
                {
                    d_date = DateTime.Now,
                   
                    f_site = log.Site,
                    f_section = log.Section.ToString(),
                    f_action = log.Action.ToString(),

                    f_page = log.PageId,
                    c_page_name = log.PageName,

                    f_user = log.UserId,
                    c_ip = log.IP,
                });
            }
        }
        #endregion

        #region CmsMenu
        public override bool check_cmsMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                bool result = false;

                int count = db.cms_menus.Where(w => w.id == id).Count();
                if (count > 0) result = true;

                return result;
            }
        }
        public override bool check_cmsMenu(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                bool result = false;

                int count = db.cms_menus.Where(w => w.c_alias == alias).Count();
                if (count > 0) result = true;

                return result;
            }
        }

        public override cmsMenuModel[] getCmsMenu(Guid user_id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_menu_groups
                    .Select(s => new cmsMenuModel
                    {
                        Num = s.num,
                        GroupName = s.c_title,
                        Alias = s.c_alias,
                        GroupItems = getCmsMenuItems(s.c_alias, user_id)
                    })
                    .OrderBy(o => o.Num);

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override cmsMenuItem[] getCmsMenuItems(string group_id, Guid user_id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sv_resolutionss.
                    Where(w => (w.f_group == group_id && w.b_read == true && w.c_user_id == user_id)).
                    Select(s => new cmsMenuItem
                    {
                        id = s.c_menu_id,
                        Permit = s.n_permit,
                        Alias = s.c_alias,
                        Title = s.c_title,
                        Class = s.c_class,
                        Group = s.f_group
                    })
                .OrderBy(o => o.Permit);

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override cmsMenuItem getCmsMenuItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_menus.
                    Where(w => w.id == id).
                    Select(s => new cmsMenuItem
                    {
                        id = s.id,
                        Permit = s.n_permit,
                        Alias = s.c_alias,
                        Title = s.c_title,
                        Desc = s.c_desc,
                        Class = s.c_class,
                        Group = s.f_group
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override cmsMenuType[] getCmsMenuType()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_menu_groups.
                    Select(s => new cmsMenuType
                    {
                        num = s.num,
                        text = s.c_title,
                        value = s.c_alias
                    })
                .OrderBy(o => o.num);

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override bool createCmsMenu(Guid id, cmsMenuItem Item, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    int Permit = 0;
                    Permit = db.cms_menus.Where(w => w.f_group == Item.Group).Select(s => s.n_permit).Count() + 1;

                    db.cms_menus
                        .Value(p => p.id, id)
                        .Value(p => p.n_permit, Permit)
                        .Value(p => p.c_title, Item.Title)
                        .Value(p => p.c_alias, Item.Alias)
                        .Value(p => p.c_class, Item.Class)
                        .Value(p => p.f_group, Item.Group)
                        .Value(p => p.c_desc, Item.Desc)
                       .Insert();


                    //insertLog(UserId, IP, "insert", id, String.Empty, "CmsMenu", Item.Title);
                    // логирование
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.CmsMenu,
                        Action = LogAction.insert,
                        PageId = id,
                        PageName = Item.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    //добавить права группы пользователей
                    Guid Dev_users = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    var data_group = db.cms_users_groups.Where(w => w.c_alias != "Developer").Select(s => new cms_users_group { id = s.id, c_alias = s.c_alias }).ToArray();//спсиок групп
                    var data_group_develop = db.cms_users_groups.Where(w => w.c_alias == "Developer").Select(s => new cms_users_group { id = s.id, c_alias = s.c_alias }).ToArray();//спсиок групп
                    var data_users = db.cms_userss.Where(w => w.id != Dev_users).Select(s => new cms_users { id = s.id }).ToArray();//спсиок пользователей
                    var data_users_develop = db.cms_userss.Where(w => w.id == Dev_users).Select(s => new cms_users { id = s.id }).ToArray();//разработчик системная учетная запись

                    //добавление прав в группы
                    foreach (cms_users_group s in data_group)
                    {
                        db.cms_resolutions_templatess
                            .Value(v => v.f_menu_id, id)
                            .Value(v => v.f_user_group, s.c_alias)
                            .Value(v => v.b_read, false)
                            .Value(v => v.b_write, false)
                            .Value(v => v.b_change, false)
                            .Value(v => v.b_delete, false)
                            .Insert();
                    }
                    foreach (cms_users_group s in data_group_develop)
                    {
                        db.cms_resolutions_templatess
                            .Value(v => v.f_menu_id, id)
                            .Value(v => v.f_user_group, s.c_alias)
                            .Value(v => v.b_read, true)
                            .Value(v => v.b_write, true)
                            .Value(v => v.b_change, true)
                            .Value(v => v.b_delete, true)
                            .Insert();
                    }
                    //добавить прав пользователям
                    foreach (cms_users s in data_users)
                    {
                        db.cms_resolutionss
                            .Value(v => v.c_menu_id, id)
                            .Value(v => v.c_user_id, s.id)
                            .Value(v => v.b_read, false)
                            .Value(v => v.b_write, false)
                            .Value(v => v.b_change, false)
                            .Value(v => v.b_delete, false)
                            .Value(v => v.b_importent, false)
                            .Insert();
                    }
                    foreach (cms_users s in data_users_develop)
                    {
                        db.cms_resolutionss
                            .Value(v => v.c_menu_id, id)
                            .Value(v => v.c_user_id, s.id)
                            .Value(v => v.b_read, true)
                            .Value(v => v.b_write, true)
                            .Value(v => v.b_change, true)
                            .Value(v => v.b_delete, true)
                            .Value(v => v.b_importent, false)
                            .Insert();
                    }

                    //insertLog(UserId, IP, "change_resolutions", id, String.Empty, "CmsMenu", Item.Title);
                    // логирование
                    log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.CmsMenu,
                        Action = LogAction.change_resolutions,
                        PageId = id,
                        PageName = Item.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        public override bool updateCmsMenu(Guid id, cmsMenuItem Item, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cms_menus.Where(w => w.id == id);

                    if (data != null)
                    {
                        data.Where(w => w.id == id)
                        .Set(p => p.c_title, Item.Title)
                        .Set(p => p.c_alias, Item.Alias)
                        .Set(p => p.c_class, Item.Class)
                        .Set(p => p.f_group, Item.Group)
                        .Set(p => p.c_desc, Item.Desc)
                        .Update();

                        //insertLog(UserId, IP, "update", id, String.Empty, "CmsMenu", Item.Title);
                        // логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.CmsMenu,
                            Action = LogAction.update,
                            PageId = id,
                            PageName = Item.Title,
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

        public override bool deleteCmsMenu(Guid id, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    string logTitle = db.cms_menus.Where(w => w.id == id).First().c_title;
                    int Num = db.cms_menus.Where(w => w.id == id).ToArray().First().n_permit;
                    string Group = db.cms_menus.Where(w => w.id == id).ToArray().First().f_group;

                    db.cms_menus
                        .Where(w => w.n_permit > Num && w.f_group == Group)
                        .Set(p => p.n_permit, p => p.n_permit - 1)
                        .Update();

                    db.cms_menus.Where(w => w.id == id).Delete();

                    //insertLog(UserId, IP, "delete", id, String.Empty, "CmsMenu", logTitle);
                    // логирование
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.CmsMenu,
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
            }
        }

        public override bool permit_cmsMenu(Guid id, int num, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                //db.cmsMenu_ChangePermit(id, num).ToArray();
                return true;
            }
        }
        #endregion

        #region Site
        public override SitesList getSiteList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_sitess.Where(w => w.c_alias != String.Empty);
                if (filtr.Disabled != null) //в данном случае используется для определения отключенных/включенных сайтов
                {
                    if ((bool)filtr.Disabled)
                    {
                        query = query.Where(w => w.b_site_off == filtr.Disabled);
                    }
                }

                if (filtr.SearchText != null)
                {
                    query = query.Where(w => w.c_name.Contains(filtr.SearchText));
                }


                query = query.OrderBy(o => new { o.c_name });

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new SitesModel
                        {
                            Id = s.id,
                            Title = s.c_name,
                            Alias = s.c_alias,
                            SiteOff = s.b_site_off,
                            Type = s.c_content_type,
                            DomainList = getSiteDomains(s.c_alias)
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    SitesModel[] sitesInfo = List.ToArray();

                    return new SitesList
                    {
                        Data = sitesInfo,
                        Pager = new Pager { page = filtr.Page, size = filtr.Size, items_count = ItemCount, page_count = ItemCount / filtr.Size }
                    };
                }
                return null;
            }
        }

        public override SitesShortModel[] getSiteListWithCheckedForUser(SiteFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_sitess
                    .Where(w => w.c_alias != String.Empty)
                    .OrderBy(o => new { o.c_name });

                var data = query
                    .Skip(filtr.Size * (filtr.Page - 1))
                    .Take(filtr.Size)
                    .Select(s => new SitesShortModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Alias = s.c_alias,
                        SiteOff = s.b_site_off,
                        Type = s.c_content_type,
                        DomainList = getSiteDomains(s.c_alias),
                        Checked = (filtr.UserId.HasValue)? s.fklinksitetousers.Any(u => u.f_user == filtr.UserId) ? true: false :false
                        });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        public override bool check_Site(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                bool rezult = false;
                int count = db.cms_sitess.Where(w => w.id == id).Count();
                if (count > 0) rezult = true;
                return rezult;
            }
        }

        public override bool insertSite(SitesModel ins, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    //проверка на существование строк с таким же алиасом
                    if (!db.cms_sitess.Where(w => w.c_alias == ins.Alias).Any())
                    {
                        db.cms_sitess
                          .Value(v => v.id, ins.Id)
                          .Value(v => v.c_alias, ins.Alias)
                          .Value(v => v.c_name, ins.Title)
                          .Value(v => v.c_content_type, ins.Type)
                          .Value(v => v.f_content, ins.ContentId)
                          .Insert();

                        // insertLog(UserId, IP, "insert", ins.Id, String.Empty, "Sites", ins.Title);
                        // логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Sites,
                            Action = LogAction.insert,
                            PageId = ins.Id,
                            PageName = ins.Title,
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

        public override bool updateSite(Guid id, SitesModel upd, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cms_sitess.Where(w => w.id == id);
                    if (data.Any())
                    {
                        data
                            .Set(s => s.c_name, upd.Title)
                            .Set(s => s.c_phone, upd.Phone)
                            .Set(s => s.c_fax, upd.Fax)
                            .Set(s => s.c_email, upd.Email)
                            .Set(s => s.c_url, upd.Site)
                            .Set(s => s.c_worktime, upd.Worktime)
                            //.Set(s => s.c_logo, upd.Logo)
                            //.Set(s => s.c_logo, upd.Logo.Url)
                            .Set(s => s.c_scripts, upd.Scripts)
                            .Set(s => s.b_site_off, upd.SiteOff)
                            .Update();
                        //Логирование
                        // insertLog(UserId, IP, "update", id, String.Empty, "Sites", upd.Title);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Sites,
                            Action = LogAction.update,
                            PageId = upd.Id,
                            PageName = upd.Title,
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

        public override bool deleteSite(Guid id, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cms_sitess.Where(w => w.id == id);
                    if (data.Any())
                    {
                        string logTitle = data.Select(s => s.c_name).FirstOrDefault();
                        data.Delete();
                        //логирование
                        //insertLog(UserId, IP, "delete", id, String.Empty, "Sites", logTitle);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Sites,
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

        private Domain[] getSiteDomains(CMSdb db, string SiteId)
        {
            var data = db.cms_sites_domainss.Where(w => w.f_site == SiteId);
            if (data.Any())
                return data.Select(s => new Domain()
                {
                    DomainName = s.c_domain,
                    id = s.id
                }).ToArray();
            return null;
        }
        //список доменных имен по алиасу сайта
        public override Domain[] getSiteDomains(string SiteId)
        {
            using (var db = new CMSdb(_context))
            {
                //var data = db.cms_sites_domainss.Where(w => w.f_site == SiteId);
                //if (data.Any())
                //    return data.Select(s => new Domain()
                //    {
                //        DomainName = s.c_domain,
                //        id = s.id
                //    }).ToArray();
                //return null;
                return getSiteDomains(db, SiteId);
            }
        }

        public override bool insertDomain(String SiteId, string NewDomain, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cms_sites_domainss.Where(w => w.c_domain == NewDomain);
                    if (!data.Any())
                    {
                        Guid NewGuid = Guid.NewGuid();
                        db.cms_sites_domainss
                                     .Value(v => v.id, NewGuid)
                                     .Value(v => v.f_site, SiteId)
                                     .Value(v => v.c_domain, NewDomain)
                                     .Insert();
                        //логирование
                        //insertLog(UserId, IP, "insert_domain", NewGuid, String.Empty, "Sites", NewDomain);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Sites,
                            Action = LogAction.insert_domain,
                            PageId = NewGuid,
                            PageName = NewDomain,
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

        public override bool deleteDomain(Guid id, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cms_sites_domainss.Where(w => w.id == id);
                    if (data.Any())
                    {
                        string domainName = data.Select(s => s.c_domain).SingleOrDefault();
                        data.Delete();

                        //логирование
                        // insertLog(UserId, IP, "delete_domain", id, _site, "Sites", _domain);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Sites,
                            Action = LogAction.delete_domain,
                            PageId = id,
                            PageName = domainName,
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
        /// Служит для определения идентификатора сайта
        /// </summary>
        /// <param name="ContentId">идентификатор контента</param>
        /// <returns></returns>
        public override string getSiteId(Guid ContentId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.f_content == ContentId);
                if (data.Any())
                    return data.First().id.ToString();

                return null;
            }
        }
        #endregion

        #region Content links to objects
        /// Добавляем связи новостей и организаций
        public override bool updateContentLinks(ContentLinkModel data)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    //Удаляем существующие связи, кроме того объекта, которому новость/событие принадлежит.
                    db.content_content_links
                                            .Where(w => w.f_content == data.ObjctId)
                                            .Where(w => w.f_link_type == data.LinkType.ToString().ToLower())
                                            .Where(w => w.b_origin != true)
                                            .Delete();

                    if (data.LinksId != null && data.LinksId.Count() > 0)
                    {
                        foreach (var link in data.LinksId)
                        {
                            var isExist = db.content_content_links
                                            .Where(t => t.f_content == data.ObjctId)
                                            .Where(t => t.f_link == link).Any();
                            if (!isExist)
                            {
                                //Добавляем дополнительные связи
                                db.content_content_links
                                                .Value(v => v.f_content, data.ObjctId)
                                                .Value(v => v.f_content_type, data.ObjctType.ToString().ToLower())
                                                .Value(v => v.f_link, link)
                                                .Value(v => v.f_link_type, data.LinkType.ToString().ToLower())
                                                .Insert();
                            }
                        }
                      }
                        tran.Commit();
                    }
                    return true;
                }
            }
        #endregion


        #region Person
        public override UsersList getPersonList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                //string[] filtr, string group, bool disabeld, int page, int size
                var query = db.content_peoples.Where(w => w.id != null);
                //if ((bool)filtr.Disabled)
                //{
                //    query = query.Where(w => w.b_disabled == filtr.Disabled);
                //}
                //if (filtr.Group != String.Empty)
                //{
                //    query = query.Where(w => w.f_group == filtr.Group);
                //}
                if (filtr.SearchText != null)
                {
                    foreach (string param in filtr.SearchText.Split(' '))
                    {
                        if (param != String.Empty)
                        {
                            query = query.Where(w => w.c_surname.Contains(param) || w.c_name.Contains(param) || w.c_patronymic.Contains(param));
                        }
                    }
                }


                query = query.OrderBy(o => new { o.c_surname, o.c_name });

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query.
                        Select(s => new UsersModel
                        {
                            Id = s.id,
                            Surname = s.c_surname,
                            Name = s.c_name,
                            FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic
                            //EMail = s.c_email,
                            //Group = s.f_group,
                            //GroupName = s.f_group_name,
                            //Disabled = s.b_disabled

                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    UsersModel[] usersInfo = List.ToArray();

                    return new UsersList
                    {
                        Data = usersInfo,
                        Pager = new Pager
                        {
                            page = filtr.Page,
                            size = filtr.Size,
                            items_count = ItemCount,
                            page_count = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                        }
                    };
                }
                return null;
            }
        }

        public override UsersModel getPerson(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_peoples.Where(w => w.id == id);
                if (data.Any())
                {
                    return data.Select(s => new UsersModel
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic
                    }).First();
                }
            }
            return null;
        }
        #endregion

    }
}
