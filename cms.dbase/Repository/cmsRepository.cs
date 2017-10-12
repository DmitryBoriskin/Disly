using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using cms.dbModel.entity.cms;

namespace cms.dbase
{
    public class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        /// <summary>
        /// Конструктор
        /// </summary>
        public cmsRepository()
        {
            _context = "defaultConnection";
        }
        public cmsRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }

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
                        Alias = s.c_alias,
                        Adress = s.c_adress,
                        Phone = s.c_phone,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Site = s.c_url,
                        Worktime = s.c_worktime,
                        Logo = s.c_logo,
                        DomainList = getSiteDomains(s.c_alias),
                        ContentId=(Guid)s.f_content,
                        Type=s.c_content_type,
                        SiteOff= s.b_site_off
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
                        Alias = s.c_alias,
                        Adress = s.c_adress,
                        Phone = s.c_phone,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Site = s.c_url,
                        Worktime = s.c_worktime,
                        Logo = s.c_logo
                        //,
                        //DomainList= getSiteDomains(s.c_alias)
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
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
        public override void insertLog(Guid UserId, string IP, string Action, Guid PageId, string Site, string Section, string PageName)
        {
            using (var db = new CMSdb(_context))
            {
                db.cms_logs.Insert(() => new cms_log
                {
                    d_date = DateTime.Now,
                    f_page = PageId,
                    c_page_name = PageName,
                    f_section = Section,
                    f_site = Site,
                    f_user = UserId,
                    c_ip = IP,
                    f_action = Action
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

                // логирование
                insertLog(UserId, IP, "insert", id, String.Empty, "CmsMenu", Item.Title);

                //добавить права группе
                //добавить права группы пользователей
                Guid Dev_users = Guid.Parse("00000000-0000-0000-0000-000000000000");
                var data_group = db.cms_users_groups.Where(w => w.c_alias != "Developer").Select(s => new cms_users_group { id = s.id, c_alias = s.c_alias }).ToArray();//спсиок групп
                var data_group_develop = db.cms_users_groups.Where(w => w.c_alias == "Developer").Select(s => new cms_users_group { id = s.id, c_alias = s.c_alias }).ToArray();//спсиок групп
                var data_users = db.cms_userss.Where(w => w.id != Dev_users).Select(s => new cms_users { id = s.id }).ToArray();//спсиок пользователей
                var data_users_develop = db.cms_userss.Where(w => w.id == Dev_users).Select(s => new cms_users { id = s.id }).ToArray();//разработчик системная учетная запись

                //добавление прав в группы и 
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

                // логирование
                insertLog(UserId, IP, "change_resolutions", id, String.Empty, "CmsMenu", Item.Title);
                return true;
            }
        }
        public override bool updateCmsMenu(Guid id, cmsMenuItem Item, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
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

                    // логирование
                    insertLog(UserId, IP, "update", id, String.Empty, "CmsMenu", Item.Title);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool deleteCmsMenu(Guid id, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                string logTitle = db.cms_menus.Where(w => w.id == id).First().c_title;
                int Num = db.cms_menus.Where(w => w.id == id).ToArray().First().n_permit;
                string Group = db.cms_menus.Where(w => w.id == id).ToArray().First().f_group;

                db.cms_menus
                    .Where(w => w.n_permit > Num && w.f_group == Group)
                    .Set(p => p.n_permit, p => p.n_permit - 1)
                    .Update();

                db.cms_menus.Where(w => w.id == id).Delete();

                // логирование
                insertLog(UserId, IP, "delete", id, String.Empty, "CmsMenu", logTitle);
                return true;
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
        public override SitesList getSiteList(FilterParams filtr, int page, int size)
        {            
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_sitess.Where(w => w.c_alias != String.Empty);
                if (filtr.Disabled != null) //в данном случае используется для определения отключенных/включенных сайтов
                {
                    if ((bool)filtr.Disabled) {
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
                            SiteOff=s.b_site_off
                        }).
                        Skip(size * (page - 1)).
                        Take(size);

                    SitesModel[] sitesInfo = List.ToArray();

                    return new SitesList
                    {
                        Data = sitesInfo,
                        Pager = new Pager { page = page, size = size, items_count = ItemCount, page_count = ItemCount / size }
                    };
                }
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
        public override bool insSite(SitesModel ins, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                //проверка на существование строк с таким же алиасом
                int count = db.cms_sitess.Where(w => w.c_alias == ins.Alias).Count();
                if (count < 1)
                {
                    db.cms_sitess
                      .Value(v => v.id, ins.Id)
                      .Value(v => v.c_alias, ins.Alias)
                      .Value(v => v.c_name, ins.Title)
                      .Value(v => v.c_content_type, ins.Type)
                      .Value(v => v.f_content, ins.ContentId)
                      .Insert();
                    //Логирование
                    insertLog(UserId, IP, "insert", ins.Id, String.Empty, "Sites", ins.Title);
                    return true;
                }
                else
                {
                    return false;
                }                                
            }
        }
        public override bool updSite(Guid id,SitesModel upd, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
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
                        .Set(s => s.c_logo, upd.Logo)
                        .Set(s => s.c_scripts, upd.Scripts)
                        .Set(s => s.b_site_off, upd.SiteOff)
                        .Update();
                    //Логирование
                    insertLog(UserId, IP, "update", id, String.Empty, "Sites", upd.Title);
                    return true;
                }
                return false;
            }
        }
        public override bool delSite(Guid id, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.id == id);
                string logTitle = data.Select(s => s.c_name).FirstOrDefault();
                if (data.Any())
                {
                    data.Delete();
                    //логирование
                    insertLog(UserId, IP, "delete", id, String.Empty, "Sites", logTitle);
                    return true;
                }
                return false;
            }
        }
        //список доменных имен по алиасу сайта
        public override Domain[] getSiteDomains(string SiteId)
        {
            using (var db = new CMSdb(_context))
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
        }

        public override bool insDomain(String SiteId, string NewDomain, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
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
                    insertLog(UserId, IP, "insert_domain", NewGuid, String.Empty, "Sites", NewDomain);
                    return true;
                }
                else return false;                
            }
        }
        public override bool delDomain(Guid id, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sites_domainss.Where(w => w.id == id);
                if (data.Any())
                {
                    string _domain = data.Select(s => s.c_domain).First();
                    string _site = data.Select(s => s.f_site).First();

                    data.Delete();
                    //логирование
                    insertLog(UserId, IP, "delete_domain", id, _site, "Sites", _domain);
                    return true;
                }
                else return false;
            }
        }
        /// <summary>
        /// Служит для определения идентификатора сайта
        /// </summary>
        /// <param name="ContentId">идентификатор контента</param>
        /// <returns></returns>
        public override string getIdSite(Guid ContentId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.f_content == ContentId);
                if (data.Any()) return data.First().id.ToString();
                return null;
            }
        }
        #endregion

        #region PortalUsers
        public override bool check_user(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                bool result = false;

                int count = db.cms_userss.Where(w => w.id == id).Count();
                if (count > 0) result = true;

                return result;
            }
        }
        public override bool check_user(string email)
        {
            using (var db = new CMSdb(_context))
            {
                bool result = false;

                int count = db.cms_userss.Where(w => w.c_email == email).Count();
                if (count > 0) result = true;

                return result;
            }
        }
        public override void check_usergroup(Guid id, string group, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                string groupName = db.cms_userss.Where(w => w.id == id).First().f_group;
                string logTitle = db.cms_userss.Where(w => w.id == id).Select(s => s.c_surname + " " + s.c_name).First().ToString();

                if (group != groupName)
                {
                    // Удаляем все права пользователя
                    db.cms_resolutionss.
                        Where(w => w.c_user_id == id).
                        Delete();
                    // Назначение прав по шаблону группы
                    ResolutionsModel[] GroupResolution = db.cms_resolutions_templatess.
                        Where(w => w.f_user_group == group).
                        Select(s => new ResolutionsModel
                        {
                            MenuId = s.f_menu_id,
                            Read = s.b_read,
                            Write = s.b_write,
                            Change = s.b_change,
                            Delete = s.b_delete
                        }).ToArray();

                    foreach (ResolutionsModel m in GroupResolution)
                    {
                        db.cms_resolutionss
                            .Value(v => v.c_user_id, id)
                            .Value(v => v.c_menu_id, m.MenuId)
                            .Value(v => v.b_read, m.Read)
                            .Value(v => v.b_write, m.Write)
                            .Value(v => v.b_change, m.Change)
                            .Value(v => v.b_delete, m.Delete)
                            .Insert();
                    }
                    // логирование
                    insertLog(UserId, IP, "change_resolutions", id, String.Empty, "Users", logTitle);
                }
            }
        }

        public override UsersList getUsersList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                //string[] filtr, string group, bool disabeld, int page, int size
                var query = db.cms_sv_userss.Where(w => w.id != null);
                if ((bool)filtr.Disabled)
                {
                    query = query.Where(w => w.b_disabled == filtr.Disabled);
                }
                if (filtr.Group != String.Empty)
                {
                    query = query.Where(w => w.f_group == filtr.Group);
                }
                foreach (string param in filtr.SearchText.Split(' '))
                {
                    if (param != String.Empty)
                    {
                        query = query.Where(w => w.c_surname.Contains(param) || w.c_name.Contains(param) || w.c_patronymic.Contains(param) || w.c_email.Contains(param));
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
                            EMail = s.c_email,
                            Group = s.f_group,
                            GroupName = s.f_group_name,
                            Disabled = s.b_disabled

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
        public override UsersModel getUser(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_userss.
                    Where(w => w.id == id).
                    Select(s => new UsersModel
                    {
                        Id = s.id,
                        EMail = s.c_email,
                        Group = s.f_group,
                        Post = s.c_post,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Birthday = s.d_birthday,
                        Sex = s.b_sex,
                        Photo = s.c_photo,
                        Adres = s.c_adres,
                        Phone = s.c_phone,
                        Mobile = s.c_mobile,
                        Contacts = s.c_contacts,
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool createUser(Guid id, UsersModel Item, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_userss.Where(w => w.id == id);
                if (!data.Any())
                {
                    db.cms_userss
                    .Value(p => p.id, id)
                    .Value(p => p.c_surname, Item.Surname)
                    .Value(p => p.c_name, Item.Name)
                    .Value(p => p.c_patronymic, Item.Patronymic)
                    .Value(p => p.f_group, Item.Group)
                    .Value(p => p.b_sex, Item.Sex)
                    .Value(p => p.d_birthday, Item.Birthday)
                    .Value(p => p.c_post, Item.Post)
                    .Value(p => p.c_adres, Item.Adres)
                    .Value(p => p.c_phone, Item.Phone)
                    .Value(p => p.c_mobile, Item.Mobile)
                    .Value(p => p.c_email, Item.EMail)
                    .Value(p => p.c_salt, Item.Salt)
                    .Value(p => p.c_hash, Item.Hash)
                    .Value(p => p.c_contacts, Item.Contacts)
                    .Value(p => p.b_disabled, Item.Disabled)
                   .Insert();

                    // логирование
                    insertLog(UserId, IP, "insert", id, String.Empty, "Users", Item.Surname + " " + Item.Name);

                    // Назначение прав по шаблону группы
                    ResolutionsModel[] GroupResolution = db.cms_resolutions_templatess.
                        Where(w => w.f_user_group == Item.Group).
                        Select(s => new ResolutionsModel
                        {
                            MenuId = s.f_menu_id,
                            Read = s.b_read,
                            Write = s.b_write,
                            Change = s.b_change,
                            Delete = s.b_delete
                        }).ToArray();

                    foreach (ResolutionsModel m in GroupResolution)
                    {
                        db.cms_resolutionss
                            .Value(v => v.c_user_id, id)
                            .Value(v => v.c_menu_id, m.MenuId)
                            .Value(v => v.b_read, m.Read)
                            .Value(v => v.b_write, m.Write)
                            .Value(v => v.b_change, m.Change)
                            .Value(v => v.b_delete, m.Delete)
                            .Insert();
                    }
                    // логирование
                    insertLog(UserId, IP, "change_resolutions", id, String.Empty, "Users", Item.Surname + " " + Item.Name);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool updateUser(Guid id, UsersModel Item, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                check_usergroup(id, Item.Group, UserId, IP);

                var data = db.cms_userss.Where(w => w.id == id);

                if (data != null)
                {
                    data.Where(w => w.id == id)
                    .Set(p => p.c_surname, Item.Surname)
                    .Set(p => p.c_name, Item.Name)
                    .Set(p => p.c_patronymic, Item.Patronymic)
                    .Set(p => p.f_group, Item.Group)
                    .Set(p => p.b_sex, Item.Sex)
                    .Set(p => p.d_birthday, Item.Birthday)
                    .Set(p => p.c_post, Item.Post)
                    .Set(p => p.c_adres, Item.Adres)
                    .Set(p => p.c_phone, Item.Phone)
                    .Set(p => p.c_mobile, Item.Mobile)
                    .Set(p => p.c_email, Item.EMail)
                    .Set(p => p.c_contacts, Item.Contacts)
                    .Set(p => p.b_disabled, Item.Disabled)
                    .Update();

                    // логирование
                    insertLog(UserId, IP, "update", id, String.Empty, "Users", Item.Surname + " " + Item.Name);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool deleteUser(Guid id, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                string logTitle = db.cms_userss.Where(w => w.id == id).Select(s => s.c_surname + " " + s.c_name).First().ToString();
                db.cms_userss.Where(w => w.id == id).Delete();

                // логирование
                insertLog(UserId, IP, "delete", id, String.Empty, "Users", logTitle);
                return true;
            }
        }

        public override void changePassword(Guid id, string Salt, string Hash, Guid UserId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                string logTitle = db.cms_userss.Where(w => w.id == id).Select(s => s.c_surname + " " + s.c_name).First().ToString();
                var data = db.cms_userss.Where(w => w.id == id);

                if (data != null)
                {
                    data.Where(w => w.id == id)
                    .Set(p => p.c_salt, Salt)
                    .Set(p => p.c_hash, Hash)
                    .Update();

                    // логирование
                    insertLog(UserId, IP, "change_pass", id, String.Empty, "Users", logTitle);
                }
            }
        }


        public override Catalog_list[] getUsersGroupList()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_users_groups.
                    Select(s => new Catalog_list
                    {
                        text = s.c_title,
                        value = s.c_alias
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override UsersGroupModel getUsersGroup(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_users_groups.
                    Where(w => w.c_alias == alias).
                    Select(s => new UsersGroupModel
                    {
                        GroupName = s.c_title,
                        Alias = s.c_alias,
                        GroupResolutions = getGroupResolutions(s.c_alias)
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        public override ResolutionsModel[] getGroupResolutions(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sv_resolutions_templatess.
                    Where(w => w.f_user_group == alias).
                    OrderBy(o => new { o.f_group, o.n_permit }).
                    Select(s => new ResolutionsModel()
                    {
                        Title = s.c_title,
                        MenuId = s.id,
                        Read = s.b_read,
                        Write = s.b_write,
                        Change = s.b_change,
                        Delete = s.b_delete
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        #endregion

        #region Materials
        public override MaterialsList getMaterialsList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_materialss.Where(w => w.id != null);
                query = query.OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new MaterialsModel
                        {
                            Id = s.id,
                            Title = s.c_title,
                            Alias = s.c_alias,
                            Preview = s.c_preview,
                            Text = s.c_text,
                            Url = s.c_url,
                            UrlName = s.c_url_name,
                            Date = s.d_date,
                            Keyw = s.c_keyw,
                            Desc = s.c_desc,
                            Disabled = s.b_disabled,
                            Important = s.b_important
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    MaterialsModel[] materialsInfo = List.ToArray();

                    return new MaterialsList
                    {
                        Data = materialsInfo,
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
        public override MaterialsModel getMaterial(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_materialss.
                    Where(w => w.id == id).
                    Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Preview = s.c_preview,
                        Text = s.c_text,
                        Url = s.c_url,
                        UrlName = s.c_url_name,
                        Date = s.d_date,
                        Keyw = s.c_keyw,
                        Desc = s.c_desc,
                        Disabled = s.b_disabled,
                        Important = s.b_important
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        public override bool insertCmsMaterial(MaterialsModel material)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_materials cdMaterial = db.content_materialss
                                                .Where(p => p.id == material.Id)
                                                .SingleOrDefault();
                    if (cdMaterial != null)
                    {
                        throw new Exception("Запись с таким Id уже существует");
                    }

                    cdMaterial = new content_materials
                    {
                        id = material.Id,
                        c_title = material.Title,
                        c_alias = material.Alias,
                        c_text = material.Text,
                        d_date = material.Date,
                        c_preview = material.Preview,
                        c_url = material.Url,
                        c_url_name = material.UrlName,
                        c_desc = material.Desc,
                        c_keyw = material.Keyw,
                        b_important = material.Important,
                        b_disabled = material.Disabled,
                        n_day = material.Date.Day,
                        n_month = material.Date.Month,
                        n_year = material.Date.Year
                    };

                    using (var tran = db.BeginTransaction())
                    {
                        db.Insert(cdMaterial);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool updateCmsMaterial(MaterialsModel material)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_materials cdMaterial = db.content_materialss
                                                .Where(p => p.id == material.Id)
                                                .SingleOrDefault();
                    if (cdMaterial == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    cdMaterial.c_title = material.Title;
                    cdMaterial.c_alias = material.Alias;
                    cdMaterial.c_text = material.Text;
                    cdMaterial.d_date = material.Date;
                    cdMaterial.c_preview = material.Preview;
                    cdMaterial.c_url = material.Url;
                    cdMaterial.c_url_name = material.UrlName;
                    cdMaterial.c_desc = material.Desc;
                    cdMaterial.c_keyw = material.Keyw;
                    cdMaterial.b_important = material.Important;
                    cdMaterial.b_disabled = material.Disabled;
                    cdMaterial.n_day = material.Date.Day;
                    cdMaterial.n_month = material.Date.Month;
                    cdMaterial.n_year = material.Date.Year;

                    using (var tran = db.BeginTransaction())
                    {
                        db.Update(cdMaterial);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool deleteCmsMaterial(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_materials cdMaterial = db.content_materialss
                                                .Where(p => p.id == id)
                                                .SingleOrDefault();
                    if (cdMaterial == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Delete(cdMaterial);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        #endregion

        #region Events
        public override EventModel getEvent(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_eventss.
                    Where(w => w.id == id).
                    Select(s => new EventModel
                    {
                        Id = s.id,
                        Num = s.num,
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Place = s.c_place,
                        EventMaker = s.c_organizer,
                        Preview = s.c_preview,
                        Text = s.c_text,
                        Url = s.c_url,
                        UrlName = s.c_url_name,
                        DateBegin = s.d_date,
                        DateEnd = s.d_date_end,
                        Annually = s.b_annually,
                        KeyW = s.c_keyw,
                        Desc = s.c_desc,
                        Disabled = s.b_disabled,
                        SiteId = getIdSite(s.id)                        
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override EventsList getEventsList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_eventss.Where(w => w.id != null);
                query = query.OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new EventModel
                        {
                            Id = s.id,
                            Num = s.num,
                            Title = s.c_title,
                            Alias = s.c_alias,
                            Place = s.c_place,
                            EventMaker = s.c_organizer,
                            Preview = s.c_preview,
                            Text = s.c_text,
                            Url = s.c_url,
                            UrlName = s.c_url_name,
                            DateBegin = s.d_date,
                            DateEnd = s.d_date_end,
                            Annually = s.b_annually,
                            KeyW = s.c_keyw,
                            Desc = s.c_desc,
                            Disabled = s.b_disabled
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    EventModel[] eventsInfo = List.ToArray();

                    return new EventsList
                    {
                        Data = eventsInfo,
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
        
        public override bool insertCmsEvent(EventModel eventData)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_events cdEvent = db.content_eventss
                                                .Where(p => p.id == eventData.Id)
                                                .SingleOrDefault();
                    if (cdEvent != null)
                    {
                        throw new Exception("Запись с таким Id уже существует");
                    }

                    var EndDate = (eventData.DateEnd.HasValue) ? eventData.DateEnd.Value : eventData.DateBegin;
                    cdEvent = new content_events
                    {
                        id = eventData.Id,
                        c_alias = eventData.Alias,
                        c_title = eventData.Title,
                        c_text = eventData.Text,
                        c_place = eventData.Place,
                        c_organizer = eventData.EventMaker,
                        c_preview = eventData.Preview,
                        c_desc = eventData.Desc,
                        c_keyw = eventData.KeyW,
                        b_annually = eventData.Annually,
                        b_disabled = eventData.Disabled,
                        d_date = eventData.DateBegin,
                        d_date_end = EndDate,
                        c_url = eventData.Url,
                        c_url_name = eventData.UrlName,
                        n_date_begin_day = int.Parse(eventData.DateBegin.ToString("MMdd")),
                        n_date_end_day = int.Parse(EndDate.ToString("MMdd"))
                    };
                    if (!eventData.Annually)
                    {
                        cdEvent.n_date_begin_year = eventData.DateBegin.Year;
                        cdEvent.n_date_end_year = eventData.DateEnd.Value.Year;
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Insert(cdEvent);
                        //insertLog(UserId, IP, "change_resolutions", id, String.Empty, "Users", logTitle);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool updateCmsEvent(EventModel eventData)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_events cdEvent = db.content_eventss
                                                .Where(p => p.id == eventData.Id)
                                                .SingleOrDefault();
                    if (cdEvent == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    var EndDate = (eventData.DateEnd.HasValue) ? eventData.DateEnd.Value : eventData.DateBegin;

                    cdEvent.c_alias = eventData.Alias;
                    cdEvent.c_title = eventData.Title;
                    cdEvent.c_text = eventData.Text;
                    cdEvent.c_place = eventData.Place;
                    cdEvent.c_organizer = eventData.EventMaker;
                    cdEvent.c_preview = eventData.Preview;
                    cdEvent.c_desc = eventData.Desc;
                    cdEvent.c_keyw = eventData.KeyW;
                    cdEvent.b_annually = eventData.Annually;
                    cdEvent.b_disabled = eventData.Disabled;
                    cdEvent.d_date = eventData.DateBegin;
                    cdEvent.d_date_end = EndDate;
                    cdEvent.c_url = eventData.Url;
                    cdEvent.c_url_name = eventData.UrlName;
                    cdEvent.n_date_begin_day = int.Parse(eventData.DateBegin.ToString("MMdd"));
                    cdEvent.n_date_end_day = int.Parse(EndDate.ToString("MMdd"));

                    if (!eventData.Annually)
                    {
                        cdEvent.n_date_begin_year = eventData.DateBegin.Year;
                        cdEvent.n_date_end_year = eventData.DateEnd.Value.Year;
                    }
                    else
                    {
                        cdEvent.n_date_begin_year = null;
                        cdEvent.n_date_end_year = null;
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Update(cdEvent);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool deleteCmsEvent(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_events cdEvent = db.content_eventss
                                                .Where(p => p.id == id)
                                                .SingleOrDefault();
                    if (cdEvent == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Delete(cdEvent);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        #endregion

        #region Orgs
        public override OrgsModel[] getOrgs(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_orgss.OrderBy(o => o.n_sort).AsQueryable();
                if (filtr.SearchText != null)
                {
                    data = data.Where(w => (w.c_title.Contains(filtr.SearchText)));
                }
                //data.OrderBy(o => o.n_sort); ХЗ почему эта строка нормально не сортирует                
                var list = data.Select(s => new OrgsModel()
                {
                    Id = s.id,
                    Title = s.c_title,
                    Sort = s.n_sort
                });
                if (list.Any()) return list.ToArray();
                return null;
            }
        }
        public override OrgsModel getOrgItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
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
                                Structure = getStructureList(s.id)
                            })
                            .FirstOrDefault();
                if (data != null) return data;
                return null;
            }
        }
        public override bool insOrgs(Guid id, OrgsModel model, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
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
                        .Insert();
                    //логирование
                    insertLog(UserId, IP, "insert", id, String.Empty, "Orgs", model.Title);
                    return true;
                }
                return false;
            }
        }
        public override bool setOrgs(Guid id, OrgsModel model, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
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
                        .Update();
                    //логирование
                    insertLog(UserId, IP, "update", id, String.Empty, "Orgs", model.Title);
                    return true;
                }
                return false;                
            }
        }        
        public override bool delOrgs(Guid id, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_orgss.Where(w => w.id == id);
                if (data.Any())
                {
                    string logTitle = data.FirstOrDefault().c_title;
                    int ThisSort = data.FirstOrDefault().n_sort;
                    db.content_orgss.Where(w => w.n_sort > ThisSort).Set(p => p.n_sort, p => p.n_sort - 1).Update();//смещение n_sort
                    data.Delete();
                    //логирование
                    insertLog(UserId, IP, "delete", id, String.Empty, "Orgs", logTitle);
                    return true;
                }
                return false;
            }
        }
        public override bool sortOrgs(Guid id, int new_num)
        {
            using (var db = new CMSdb(_context))
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

                return true;
            }
        }


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
        public override bool insStructure(Guid id, Guid OrgId, StructureModel insert, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                int MaxSort = 0;
                try
                {
                    MaxSort = db.content_org_structures.Where(w => w.f_ord == OrgId).Max(m => m.n_sort);
                }
                catch { }
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
                insertLog(UserId, IP, "insert", id, String.Empty, "Orgs", insert.Title);
                return true;
            }
        }
        public override bool setStructure(Guid id, StructureModel insert, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
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
                    insertLog(UserId, IP, "update", id, String.Empty, "Orgs", insert.Title);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool delStructure(Guid id, Guid UserId, String IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_org_structures.Where(w => w.id == id);
                Guid IdOrg = data.FirstOrDefault().f_ord;
                int ThisSort = data.FirstOrDefault().n_sort;
                string logTitle = data.FirstOrDefault().c_title;
                if (data.Any())
                {

                    data.Delete(); 
                    db.content_org_structures.Where(w => w.f_ord == IdOrg && w.n_sort > ThisSort).Set(p => p.n_sort, p => p.n_sort - 1).Update();//смещение n_sort
                    //логирование
                    insertLog(UserId, IP, "delete", id, String.Empty, "Orgs", logTitle);
                    return true;
                }
                return false;
            }
        }
        public override bool sortStructure(Guid id, int new_num)
        {
            using (var db = new CMSdb(_context))
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

                return true;
            }
        }


        /// <summary>
        /// Добавляем ОВП
        /// </summary>        
        /// <returns></returns>
        public override bool insOvp(Guid IdStructure, Guid OrgId, StructureModel insertStructure, Guid UserId, String IP)
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
                    insertLog(UserId, IP, "insert", IdStructure, String.Empty, "Orgs", logTitle);
                    return true;
                }
            }
        }
        public override bool setOvp(Guid IdStructure, StructureModel updStructure, Guid UserId, String IP)
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
                    insertLog(UserId, IP, "update", IdStructure, String.Empty, "Orgs", updStructure.Title);
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
        public override BreadCrumb[] getBreadCrumbOrgs(Guid id, string type)
        {
            using (var db = new CMSdb(_context))
            {
                var MyBread = new Stack<BreadCrumb>();
                MyBread.Push(new BreadCrumb
                {
                    Title = "Организации",
                    Url = "/admin/orgs/"
                });
                #region item
                if (type == "item")
                {
                    var data = db.content_departmentss.Where(w => w.id == id).FirstOrDefault();
                    MyBread.Push(new BreadCrumb
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

                    MyBread.Push(new BreadCrumb
                    {
                        Title = ParentStructure.c_title,
                        Url = "/admin/orgs/item/" + ParentStructure.id
                    });
                    MyBread.Push(new BreadCrumb
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

                    MyBread.Push(new BreadCrumb
                    {
                        Title = ParentStructure.c_title,
                        Url = "/admin/orgs/item/" + ParentStructure.id
                    });
                    MyBread.Push(new BreadCrumb
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

                    MyBread.Push(new BreadCrumb
                    {
                        Title = ParentOrg.c_title,
                        Url = "/admin/orgs/item/" + ParentOrg.id
                    });
                    MyBread.Push(new BreadCrumb
                    {
                        Title = ParentStructure.c_title,
                        Url = "/admin/orgs/structure/" + ParentStructure.id
                    });
                    MyBread.Push(new BreadCrumb
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
        public override bool insDepartmentsPhone(Guid idDepart, string Label, string Value, Guid UserId, String IP)
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
                insertLog(UserId, IP, "insert_phone_depart", idDepart, String.Empty, "Orgs", Label);
                return true;
            }
        }
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
                           });
                if (data.Any()) return data.ToArray();
                return null; 
            }          
        }
        public override bool insDepartament(Guid id, Guid Structure, Departments insert, Guid UserId, String IP)
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
                    n_sort = MaxSort
                };
                using (var tran = db.BeginTransaction())
                {
                    db.Insert(cdDepart);
                    tran.Commit();                    
                }                
                return true;
            }
        }
        public override bool updDepartament(Guid id, Departments insert, Guid UserId, String IP)
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
                using (var tran = db.BeginTransaction())
                {
                    db.Update(cdDepart);
                    tran.Commit();                    
                }
                return true;
            }
        }
        public override bool delDepartament(Guid id, Guid UserId, String IP)
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
                    return data.Select(s => new UsersModel {
                                Id=s.id,
                                FIO= s.c_surname+" "+s.c_name+" "+s.c_patronymic
                            }).First();
                }
            }
           return null;
        }
        #endregion

        #region Vacancies
        public override VacanciesList getVacanciesList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_vacanciess.Where(w => w.id != null);
                query = query.OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new VacancyModel
                        {
                            Id = s.id,
                            Profession = s.c_profession,
                            Post = s.c_post,
                            Date = s.d_date,
                            Experience = s.с_experience,
                            Сonditions = s.с_conditions,
                            Salary = s.c_salary,
                            Desc = s.c_desc,
                            Temporarily = s.b_temporarily,
                            Disabled = s.b_disabled
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    VacancyModel[] eventsInfo = List.ToArray();

                    return new VacanciesList
                    {
                        Data = eventsInfo,
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
        public override VacancyModel getVacancy(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_vacanciess.
                    Where(w => w.id == id).
                    Select(s => new VacancyModel
                    {
                        Id = s.id,
                        Profession = s.c_profession,
                        Post = s.c_post,
                        Date = s.d_date,
                        Сonditions = s.с_conditions,
                        Experience = s.с_experience,
                        Salary = s.c_salary,
                        Desc = s.c_desc,
                        Temporarily = s.b_temporarily,
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        public override bool insertCmsVacancy(VacancyModel vacancy)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_vacancies cdVacancy = db.content_vacanciess
                                                .Where(p => p.id == vacancy.Id)
                                                .SingleOrDefault();
                    if (cdVacancy != null)
                    {
                        throw new Exception("Запись с таким Id уже существует");
                    }

                    cdVacancy = new content_vacancies
                    {
                        id = vacancy.Id,
                        c_profession = vacancy.Profession,
                        c_post = vacancy.Post,
                        d_date = vacancy.Date,
                        с_conditions = vacancy.Сonditions,
                        с_experience = vacancy.Experience,
                        c_salary = vacancy.Salary,
                        c_desc = vacancy.Desc,
                        b_temporarily = vacancy.Temporarily,
                        b_disabled = vacancy.Disabled
                    };

                    using (var tran = db.BeginTransaction())
                    {
                        db.Insert(cdVacancy);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool updateCmsVacancy(VacancyModel vacancy)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_vacancies cdVacancy = db.content_vacanciess
                                                .Where(p => p.id == vacancy.Id)
                                                .SingleOrDefault();
                    if (cdVacancy == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    cdVacancy.c_profession = vacancy.Profession;
                    cdVacancy.c_post = vacancy.Post;
                    cdVacancy.d_date = vacancy.Date;
                    cdVacancy.с_conditions = vacancy.Сonditions;
                    cdVacancy.с_experience = vacancy.Experience;
                    cdVacancy.c_salary = vacancy.Salary;
                    cdVacancy.c_desc = vacancy.Desc;
                    cdVacancy.b_temporarily = vacancy.Temporarily;
                    cdVacancy.b_disabled = vacancy.Disabled;

                    using (var tran = db.BeginTransaction())
                    {
                        db.Update(cdVacancy);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool deleteCmsVacancy(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_vacancies cdVacancy = db.content_vacanciess
                                                .Where(p => p.id == id)
                                                .SingleOrDefault();
                    if (cdVacancy == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Delete(cdVacancy);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }

        #endregion

        #region FeedBacks
        public override FeedbacksList getFeedbacksList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_feedbackss.Where(w => w.id != null);
                query = query.OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new FeedbackModel
                        {
                            Id = s.id,
                            Title = s.c_title,
                            Text = s.c_text,
                            Date = s.d_date,
                            SenderName = s.c_sender_name,
                            SenderEmail = s.c_sender_email,
                            Answer = s.c_answer,
                            Answerer = s.c_answerer,
                            IsNew = s.b_new,
                            Disabled = s.b_disabled
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    FeedbackModel[] eventsInfo = List.ToArray();

                    return new FeedbacksList
                    {
                        Data = eventsInfo,
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
        public override FeedbackModel getFeedback(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_feedbackss.
                    Where(w => w.id == id).
                    Select(s => new FeedbackModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        SenderName = s.c_sender_name,
                        SenderEmail = s.c_sender_email,
                        Answer = s.c_answer,
                        Answerer = s.c_answerer,
                        IsNew = s.b_new,
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        public override bool insertCmsFeedback(FeedbackModel feedback)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_feedbacks cdFeedback = db.content_feedbackss
                                                .Where(p => p.id == feedback.Id)
                                                .SingleOrDefault();
                    if (cdFeedback != null)
                    {
                        throw new Exception("Запись с таким Id уже существует");
                    }

                    cdFeedback = new content_feedbacks
                    {
                        id = feedback.Id,
                        c_title = feedback.Title,
                        c_text = feedback.Text,
                        d_date = feedback.Date,
                        c_sender_name = feedback.SenderName,
                        c_sender_email = feedback.SenderEmail,
                        c_answer = feedback.Answer,
                        c_answerer = feedback.Answerer,
                        b_new = feedback.IsNew,
                        b_disabled = feedback.Disabled
                    };

                    using (var tran = db.BeginTransaction())
                    {
                        db.Insert(cdFeedback);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool updateCmsFeedback(FeedbackModel feedback)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_feedbacks cdFeedback = db.content_feedbackss
                                                .Where(p => p.id == feedback.Id)
                                                .SingleOrDefault();
                    if (cdFeedback == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    cdFeedback.c_title = feedback.Title;
                    cdFeedback.c_text = feedback.Text;
                    cdFeedback.c_sender_email = feedback.SenderEmail;
                    cdFeedback.c_sender_name = feedback.SenderName;
                    cdFeedback.c_answer = feedback.Answer;
                    cdFeedback.c_answerer = feedback.Answerer;
                    cdFeedback.d_date = feedback.Date;
                    cdFeedback.b_new = feedback.IsNew;
                    cdFeedback.b_disabled = feedback.Disabled;

                    using (var tran = db.BeginTransaction())
                    {
                        db.Update(cdFeedback);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool deleteCmsFeedback(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_feedbacks cdfeedback = db.content_feedbackss
                                                .Where(p => p.id == id)
                                                .SingleOrDefault();
                    if (cdfeedback == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Delete(cdfeedback);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        #endregion

        #region Карта сайта
        /// <summary>
        /// Получаем список записей карты сайта
        /// </summary>
        /// <param name="site">Алиас сайта</param>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override SiteMapList getSiteMapList(string site, FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                if (string.IsNullOrEmpty(filtr.Group))
                {
                    var query = db.content_sitemaps
                        .Where(w => w.f_site.Equals(site))
                        .Where(w => !w.id.Equals(null))
                        .Where(w => w.uui_parent.Equals(null))
                        .Where(w => !w.c_alias.Equals(" "));

                    if (query.Any())
                    {
                        int itemCount = query.Count();

                        var list = query.Select(s => new SiteMapModel
                        {
                            Id = s.id,
                            Site = s.f_site,
                            FrontSection = s.f_front_section,
                            Path = s.c_path,
                            Alias = s.c_alias,
                            Title = s.c_title,
                            Text = s.c_text,
                            Preview = s.c_preview,
                            Url = s.c_url,
                            Desc = s.c_desc,
                            Keyw = s.c_keyw,
                            Disabled = s.b_disabled,
                            DisabledMenu = s.b_disabled_menu,
                            Sort = s.n_sort,
                            ParentId = s.uui_parent,
                            CountSibling = getCountSiblings(s.id)
                        }).Skip(filtr.Size * (filtr.Page - 1))
                          .Take(filtr.Size);

                        var siteMapList = list.OrderBy(o => o.Sort).ToArray();

                        return new SiteMapList
                        {
                            Data = siteMapList,
                            Pager = new Pager
                            {
                                page = filtr.Page,
                                size = filtr.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                            }
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Guid? menuId = Guid.Parse(filtr.Group);

                    var query = db.content_sv_sitemap_menus
                        .Where(w => w.f_site.Equals(site))
                        .Where(w => !w.id.Equals(null))
                        .Where(w => w.f_menutype.Equals(menuId))
                        .OrderBy(o => o.menu_sort);

                    if (query.Any())
                    {
                        int itemCount = query.Count();

                        var list = query.Select(s => new SiteMapModel
                        {
                            Id = s.id,
                            Site = s.f_site,
                            FrontSection = s.f_front_section,
                            Path = s.c_path,
                            Alias = s.c_alias,
                            Title = s.c_title,
                            Text = s.c_text,
                            Preview = s.c_preview,
                            Url = s.c_url,
                            Desc = s.c_desc,
                            Keyw = s.c_keyw,
                            Disabled = s.b_disabled,
                            DisabledMenu = s.b_disabled_menu,
                            Sort = s.n_sort,
                            ParentId = s.uui_parent,
                            CountSibling = getCountSiblings(s.id)
                        }).Skip(filtr.Size * (filtr.Page - 1))
                          .Take(filtr.Size);

                        var siteMapList = list.ToArray();

                        return new SiteMapList
                        {
                            Data = siteMapList,
                            Pager = new Pager
                            {
                                page = filtr.Page,
                                size = filtr.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                            }
                        };
                    }
                    else
                    {
                        return null;
                    }
                }

            }
        }

        /// <summary>
        /// Получаем единичную запись карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override SiteMapModel getSiteMapItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                    .Where(w => w.id.Equals(id))
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,
                        Site = s.f_site,
                        FrontSection = s.f_front_section,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        Title = s.c_title,
                        Text = s.c_text,
                        Preview = s.c_preview,
                        Url = s.c_url,
                        Desc = s.c_desc,
                        Keyw = s.c_keyw,
                        Disabled = s.b_disabled,
                        DisabledMenu = s.b_disabled_menu,
                        Sort = s.n_sort,
                        ParentId = s.uui_parent,
                        CountSibling = getCountSiblings(s.id),
                        MenuGroups = getSiteMapGroupMenu(id)
                    });

                if (!data.Any()) { return null; }
                else { return data.FirstOrDefault(); }
            }
        }

        /// <summary>
        /// Получим группы меню для элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override string[] getSiteMapGroupMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemap_menutypess
                    .Where(w => w.f_sitemap.Equals(id))
                    .Select(s => s.f_menutype.ToString());

                if (!data.Any()) return null;
                else { return data.ToArray(); }
            }
        }

        /// <summary>
        /// Получим кол-во дочерних элементов карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override int getCountSiblings(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                int result = db.content_sitemaps
                    .Where(w => w.uui_parent.Equals(id))
                    .Count();

                return result;
            }
        }

        /// <summary>
        /// Проверяем существование элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override bool checkSiteMap(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                int count = db.content_sitemaps.Where(w => w.id == id).Count();
                bool result = count > 0 ? true : false;

                return result;
            }
        }

        /// <summary>
        /// Создаём новый раздел в карте сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="item">Элемент карты сайта</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="IP">Ip-адрес</param>
        /// <returns></returns>
        public override bool createSiteMapItem(Guid id, SiteMapModel item, Guid userId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps.Where(w => w.id.Equals(id));
                if (!data.Any())
                {
                    var queryMaxSort = db.content_sitemaps
                        .Where(w => w.f_site == item.Site)
                        .Where(w => w.c_path.Equals(item.Path))
                        .Select(s => s.n_sort);

                    int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 1;

                    db.content_sitemaps
                        .Value(p => p.id, id)
                        .Value(p => p.f_site, item.Site)
                        .Value(p => p.f_front_section, item.FrontSection)
                        .Value(p => p.c_path, item.Path)
                        .Value(p => p.c_alias, item.Alias)
                        .Value(p => p.c_title, item.Title)
                        .Value(p => p.c_preview, item.Preview)
                        .Value(p => p.c_url, item.Url)
                        .Value(p => p.c_desc, item.Desc)
                        .Value(p => p.c_keyw, item.Keyw)
                        .Value(p => p.b_disabled, item.Disabled)
                        .Value(p => p.b_disabled_menu, item.DisabledMenu)
                        .Value(p => p.n_sort, maxSort)
                        .Value(p => p.uui_parent, item.ParentId)
                        .Insert();

                    // группы меню
                    if (item.MenuGroups != null)
                    {
                        foreach (var m in item.MenuGroups)
                        {
                            Guid menuId = Guid.Parse(m);

                            var _maxSortMenu = db.content_sitemap_menutypess
                                .Where(w => w.f_site.Equals(item.Site))
                                .Where(w => w.f_menutype.Equals(menuId))
                                .Select(s => s.n_sort);

                            int mS = _maxSortMenu.Any() ? _maxSortMenu.Max() : 0;
                            
                            var menu = db.content_sitemap_menutypess
                                .Value(p => p.f_sitemap, id)
                                .Value(p => p.f_menutype, menuId)
                                .Value(p => p.f_site, item.Site)
                                .Value(p => p.n_sort, mS)
                                .Insert();
                        }
                    }

                    // логирование
                    insertLog(userId, IP, "insert", id, String.Empty, "SiteMap", item.Title);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Обновляем запись карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="item">Элемент карты сайта</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="IP">Ip-адрес</param>
        /// <returns></returns>
        public override bool updateSiteMapItem(Guid id, SiteMapModel item, Guid userId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps.Where(w => w.id.Equals(id));

                if (data.Any())
                {
                    var oldRecord = data.FirstOrDefault();

                    data.Where(w => w.id.Equals(id))
                        .Set(u => u.f_site, item.Site)
                        .Set(u => u.f_front_section, item.FrontSection)
                        .Set(u => u.c_path, item.Path)
                        .Set(u => u.c_alias, item.Alias)
                        .Set(u => u.c_title, item.Title)
                        .Set(u => u.c_text, item.Text)
                        .Set(u => u.c_preview, item.Preview)
                        .Set(u => u.c_url, item.Url)
                        .Set(u => u.c_desc, item.Desc)
                        .Set(u => u.c_keyw, item.Keyw)
                        .Set(u => u.b_disabled, item.Disabled)
                        .Set(u => u.b_disabled_menu, item.DisabledMenu)
                        .Update();

                    #region обновим алиасы для дочерних эл-тов

                    // заменяемый путь 
                    string _oldPath = oldRecord.c_path.Equals("/") ?
                        oldRecord.c_path + oldRecord.c_alias : oldRecord.c_path + "/" + oldRecord.c_alias;

                    // новый путь
                    string _newPath = item.Path.Equals("/") ?
                        item.Path + item.Alias : item.Path + "/" + item.Alias;

                    // список дочерних эл-тов для обновления алиаса
                    var listToUpdate = db.content_sitemaps
                        .Where(w => w.f_site.Equals(item.Site))
                        .Where(w => w.c_path.StartsWith(_oldPath));
                    
                    if (listToUpdate.Any())
                    {
                        listToUpdate
                            .Set(u => u.c_path, u => u.c_path.Replace(_oldPath, _newPath))
                            .Update();
                    }

                    #endregion
                    // группы меню
                    var menu = db.content_sitemap_menutypess
                        .Where(w => w.f_sitemap.Equals(id)).Delete();

                    if (item.MenuGroups != null)
                    {
                        foreach (var m in item.MenuGroups)
                        {
                            Guid menuId = Guid.Parse(m);

                            var _maxSortMenu = db.content_sitemap_menutypess
                                .Where(w => w.f_site.Equals(item.Site))
                                .Where(w => w.f_menutype.Equals(menuId));

                            int resmaxSortMenu = _maxSortMenu.Any() ? _maxSortMenu.Select(s => s.n_sort).Max() : 0;

                            var res = db.content_sitemap_menutypess
                                .Value(p => p.f_sitemap, id)
                                .Value(p => p.f_menutype, menuId)
                                .Value(p => p.f_site, item.Site)
                                .Value(p => p.n_sort, resmaxSortMenu + 1)
                                .Insert();
                        }
                    }

                    // логирование
                    insertLog(userId, IP, "update", id, String.Empty, "SiteMap", item.Title);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Получаем список типов страниц
        /// </summary>
        /// <returns></returns>
        public override SiteMapMenu[] getSiteMapFrontSectionList()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.front_sections
                    .Select(s => new SiteMapMenu
                    {
                        Text = s.c_name,
                        Value = s.c_alias
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        /// <summary>
        /// Список доступных типов меню для элемента карты сайта
        /// </summary>
        /// <returns></returns>
        public override Catalog_list[] getSiteMapMenuTypes()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemap_menuss
                    .OrderBy(o => o.n_sort)
                    .Select(s => new Catalog_list
                    {
                        text = s.c_title,
                        value = s.id.ToString()
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        /// <summary>
        /// Добавляем меню в карту сайта
        /// </summary>
        /// <param name="item">Элемент карты сайта</param>
        /// <returns></returns>
        public override bool createSiteMapMenu(SiteMapMenu item)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemap_menuss.Where(w => w.id.Equals(item.Id));
                if (!query.Any())
                {
                    var sortMax = db.content_sitemap_menuss.Select(s => s.n_sort).Max();

                    db.content_sitemap_menuss
                        .Value(v => v.id, Guid.NewGuid())
                        .Value(v => v.c_title, item.Text)
                        .Value(v => v.n_sort, sortMax + 1)
                        .Insert();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаляем элемент карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="IP">Ip-адрес</param>
        /// <returns></returns>
        public override bool deleteSiteMapItem(Guid id, Guid userId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var itemToDelete = db.content_sitemaps
                    .Where(w => w.id.Equals(id))
                    .Select(s => new SiteMapModel
                    {
                        Title = s.c_title,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        Sort = s.n_sort
                    }).FirstOrDefault();

                // Обновляем поле для сортировки для сестринских эл-тов
                var listToUpdate = db.content_sitemaps
                    .Where(w => w.c_path.Equals(itemToDelete.Path))
                    .Where(w => w.n_sort > itemToDelete.Sort);

                listToUpdate.Set(u => u.n_sort, u => u.n_sort - 1).Update();

                // Удаляем дочерние эл-ты 
                string pathToDrop = itemToDelete.Path.Equals("/") ?
                    itemToDelete.Path + itemToDelete.Alias :
                    itemToDelete.Path + "/" + itemToDelete.Alias;

                var listToDelete = db.content_sitemaps
                    .Where(w => w.id.Equals(id) || w.c_path.Contains(pathToDrop));
                
                if (listToDelete.Any())
                {
                    foreach (var item in listToDelete.ToArray())
                    {
                        // Логирование
                        insertLog(userId, IP, "delete", item.id, String.Empty, "SiteMap", item.c_title);
                        
                        var itemD = db.content_sitemaps
                            .Where(w => w.id == item.id)
                            .SingleOrDefault();

                        db.Delete(itemD);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Получаем список дочерних элементов для текущего
        /// </summary>
        /// <param name="parent">Родительский идентификатор</param>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapChildrens(Guid parent)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                    .Where(w => w.uui_parent.Equals(parent))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        Title = s.c_title,
                        Disabled = s.b_disabled,
                        DisabledMenu = s.b_disabled_menu,
                        Sort = s.n_sort,
                        CountSibling = getCountSiblings(s.id)
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        /// <summary>
        /// Получаем хлебные крошки для карты сайта
        /// </summary>
        /// <param name="id">Идентификатор элемента карты сайта</param>
        /// <returns></returns>
        public override BreadCrumbSiteMap[] getSiteMapBreadCrumbs(Guid? id)
        {
            List<BreadCrumbSiteMap> breadCrumbList = new List<BreadCrumbSiteMap>();

            if (!id.Equals(null))
            {
                BreadCrumbSiteMap item = getSiteMapBreadCrumbItem((Guid)id);

                while (item != null)
                {
                    breadCrumbList.Add(item);
                    if (!item.ParentId.Equals(null))
                    {
                        item = getSiteMapBreadCrumbItem((Guid)item.ParentId);
                    }
                    else { item = null; }
                }
            }

            breadCrumbList.Reverse();

            return breadCrumbList != null ? breadCrumbList.ToArray() : null;
        }

        /// <summary>
        /// Единичная запись хлебных крошек для карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override BreadCrumbSiteMap getSiteMapBreadCrumbItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                    .Where(w => w.id.Equals(id))
                    .Select(s => new BreadCrumbSiteMap
                    {
                        Id = s.id,
                        Title = s.c_title,
                        ParentId = s.uui_parent
                    });

                if (!data.Any()) { return null; }
                else { return data.FirstOrDefault(); }
            }
        }

        /// <summary>
        /// Меняем приоритет для сортировки карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="permit">Порядока сортировки</param>
        /// <param name="domain">Алиас сайта</param>
        /// <param name="menuSort">Сортировка в записи для типа меню</param>
        /// <returns></returns>
        public override bool permit_SiteMap(Guid id, int permit, string domain, string menuSort)
        {
            using (var db = new CMSdb(_context))
            {
                if (string.IsNullOrEmpty(menuSort))
                {
                    var data = db.content_sitemaps
                        .Where(w => w.id.Equals(id))
                        .Select(s => new SiteMapModel
                        {
                            Path = s.c_path,
                            Sort = s.n_sort
                        }).FirstOrDefault();

                    if (permit > data.Sort)
                    {
                        db.content_sitemaps
                            .Where(w => w.f_site.Equals(domain))
                            .Where(w => w.c_path.Equals(data.Path))
                            .Where(w => w.n_sort > data.Sort && w.n_sort <= permit)
                            .Set(u => u.n_sort, u => u.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_sitemaps
                            .Where(w => w.f_site.Equals(domain))
                            .Where(w => w.c_path.Equals(data.Path))
                            .Where(w => w.n_sort < data.Sort && w.n_sort >= permit)
                            .Set(u => u.n_sort, u => u.n_sort + 1)
                            .Update();
                    }
                    db.content_sitemaps
                        .Where(w => w.id.Equals(id))
                        .Set(u => u.n_sort, permit)
                        .Update();
                }
                else
                {
                    Guid m = Guid.Parse(menuSort);

                    var data = db.content_sv_sitemap_menus
                        .Where(w => w.id.Equals(id))
                        .Select(s => new SiteMapModel
                        {
                            MenuGr = s.f_menutype,
                            Sort = s.menu_sort
                        }).FirstOrDefault();

                    if (permit > data.Sort)
                    {
                        db.content_sitemap_menutypess
                            .Where(w => w.f_site.Equals(domain))
                            .Where(w => w.f_menutype.Equals(m))
                            .Where(w => w.n_sort > data.Sort && w.n_sort <= permit)
                            .Set(u => u.n_sort, u => u.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_sitemap_menutypess
                            .Where(w => w.f_site.Equals(domain))
                            .Where(w => w.f_menutype.Equals(m))
                            .Where(w => w.n_sort < data.Sort && w.n_sort >= permit)
                            .Set(u => u.n_sort, u => u.n_sort + 1)
                            .Update();
                    }
                    db.content_sitemap_menutypess
                        .Where(w => w.f_sitemap.Equals(id))
                        .Where(w => w.f_menutype.Equals(m))
                        .Set(u => u.n_sort, permit)
                        .Update();
                }
            }
            return true;
        }
        #endregion

        #region Баннеры
        /// <summary>
        /// Получаем список секций баннеров
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override BannersSectionModel[] getBannerSections(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_banner_sectionss
                    .OrderBy(o => o.n_sort)
                    .Select(s => new BannersSectionModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort,
                        Disabled = s.b_disabled,
                        CountBanners = getCountBannersBySectionAndDomain(s.id, domain),
                        Width = s.n_width,
                        Height = s.n_height
                    });
                if (!query.Any()) { return null; }
                else { return query.ToArray(); }
            }
        }

        /// <summary>
        /// Получаем отдельную секцию баннеров
        /// </summary>
        /// <param name="id">Id секции</param>
        /// <param name="domain">Домен</param>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        public override BannersSectionModel getBannerSection(Guid id, string domain, FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_banner_sectionss
                    .Where(w => w.id == id)
                    .Select(s => new BannersSectionModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort,
                        Disabled = s.b_disabled,
                        Width = s.n_width,
                        Height = s.n_height,
                        CountBanners = getCountBannersBySectionAndDomain(s.id, domain),
                        BannerList = getBanners(id, domain, filter)
                    });
                if (!query.Any()) return null;
                else { return query.FirstOrDefault(); }
            }
        }

        /// <summary>
        /// Получаем кол-во баннеров для сайта и секции
        /// </summary>
        /// <param name="section">Секция баннеров</param>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override int getCountBannersBySectionAndDomain(Guid section, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_bannerss
                    .Where(w => w.f_site == domain)
                    .Where(w => w.f_section == section)
                    .Count();
            }
        }

        /// <summary>
        /// Получим список баннеров для секции и домена
        /// </summary>
        /// <param name="section">Секция</param>
        /// <param name="domain">Домен</param>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        public override BannersListModel getBanners(Guid section, string domain, FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss
                    .Where(w => w.f_site == domain)
                    .Where(w => w.f_section == section);

                if (query.Any() && filter != null)
                {
                    int itemCount = query.Count();
                    var list = query
                    .Select(s => new BannersModel
                    {
                        Id = s.id,
                        Site = s.f_site,
                        Title = s.c_title,
                        Url = s.c_url,
                        Text = s.c_text,
                        Date = s.d_date,
                        Sort = s.n_sort,
                        Disabled = s.b_disabled,
                        Section = s.f_section,
                        Photo = new Photo
                        {
                            Url = s.c_photo
                        }
                    }).Skip(filter.Size * (filter.Page - 1)).Take(filter.Size);

                    var bannerList = list.OrderBy(o => o.Sort).ToArray();

                    return new BannersListModel
                    {
                        Data = bannerList,
                        Pager = new Pager
                        {
                            page = filter.Page,
                            size = filter.Size,
                            items_count = itemCount,
                            page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1
                                : itemCount / filter.Size
                        }
                    };
                }
                else { return null; }
            }
        }

        /// <summary>
        /// Получим баннер
        /// </summary>
        /// <param name="id">Id баннера</param>
        /// <returns></returns>
        public override BannersModel getBanner(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss
                    .Where(w => w.id == id)
                    .Select(s => new BannersModel
                    {
                        Id = s.id,
                        Site = s.f_site,
                        Title = s.c_title,
                        Url = s.c_url,
                        Text = s.c_text,
                        Date = s.d_date,
                        Sort = s.n_sort,
                        Disabled = s.b_disabled,
                        Section = s.f_section,
                        Photo = new Photo
                        {
                            Url = s.c_photo
                        }
                    });
                if (!query.Any()) return null;
                else { return query.FirstOrDefault(); }
            }
        }

        /// <summary>
        /// Проверим существование баннера
        /// </summary>
        /// <param name="id">Идентификатор баннера</param>
        /// <returns></returns>
        public override bool checkBannerExist(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_bannerss.Where(w => w.id == id).Count() > 0;
            }
        }

        /// <summary>
        /// Создадим новый баннер
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="item">Модель баннера</param>
        /// <param name="userId">Id-пользователя</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool createBanner(Guid id, BannersModel item, Guid userId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss.Where(w => w.id.Equals(id));

                if (!query.Any())
                {
                    var queryMaxSort = db.content_bannerss
                        .Where(w => w.f_site.Equals(item.Site))
                        .Where(w => w.f_section.Equals(item.Section))
                        .Select(s => s.n_sort);

                    int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 1;

                    db.content_bannerss
                        .Value(v => v.id, id)
                        .Value(v => v.f_site, item.Site)
                        .Value(v => v.c_title, item.Title)
                        .Value(v => v.c_photo, item.Photo != null ? item.Photo.Url : null)
                        .Value(v => v.c_url, item.Url)
                        .Value(v => v.c_text, item.Text)
                        .Value(v => v.d_date, item.Date)
                        .Value(v => v.n_sort, maxSort)
                        .Value(v => v.b_disabled, item.Disabled)
                        .Value(v => v.f_section, item.Section)
                        .Insert();

                    // логирование
                    insertLog(userId, IP, "insert", id, String.Empty, "Banners", item.Title);

                    return true;
                }
                else { return false; }
            }
        }

        /// <summary>
        /// Обновляем запись баннера
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="item">Модель баннера</param>
        /// <param name="userId">Id-пользователя</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool updateBanner(Guid id, BannersModel item, Guid userId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss.Where(w => w.id.Equals(id));

                if (query.Any())
                {
                    var oldRecord = query.FirstOrDefault();

                    string img = item.Photo == null ? oldRecord.c_photo : item.Photo.Url;

                    query
                        .Set(s => s.c_title, item.Title)
                        .Set(s => s.c_photo, img)
                        .Set(s => s.c_url, item.Url)
                        .Set(s => s.c_text, item.Text)
                        .Set(s => s.d_date, item.Date)
                        .Set(s => s.b_disabled, item.Disabled)
                        .Set(s => s.f_section, item.Section)
                        .Update();

                    // логирование
                    insertLog(userId, IP, "update", id, string.Empty, "Banners", item.Title);

                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Удаляем баннер
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <param name="userId">Id-пользователя</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool deleteBanner(Guid id, Guid userId, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss.Where(w => w.id.Equals(id));

                if (query.Any())
                {
                    string title = query.Select(s => s.c_title).FirstOrDefault();

                    db.content_bannerss.Where(w => w.id.Equals(id)).Delete();
                    
                    // логирование
                    insertLog(userId, IP, "delete", id, string.Empty, "Banners", title);

                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Меняем приоритет сортировки в баннерах для определённой секции
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <param name="permit">Приоритет</param>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override bool permit_Banners(Guid id, int permit, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_bannerss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new BannersModel
                    {
                        Section = s.f_section,
                        Sort = s.n_sort
                    });

                if (data.Any())
                {
                    var query = data.FirstOrDefault();
                    if (permit > query.Sort)
                    {
                        db.content_bannerss
                            .Where(w => w.f_site.Equals(domain))
                            .Where(w => w.f_section.Equals(query.Section))
                            .Where(w => w.n_sort > query.Sort && w.n_sort <= permit)
                            .Set(u => u.n_sort, u => u.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_bannerss
                            .Where(w => w.f_site.Equals(domain))
                            .Where(w => w.f_section.Equals(query.Section))
                            .Where(w => w.n_sort < query.Sort && w.n_sort >= permit)
                            .Set(u => u.n_sort, u => u.n_sort + 1)
                            .Update();
                    }
                    db.content_bannerss
                        .Where(w => w.id.Equals(id))
                        .Set(u => u.n_sort, permit)
                        .Update();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
