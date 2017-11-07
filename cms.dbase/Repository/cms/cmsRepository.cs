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
        /// <summary>
        /// Конструктор
        /// </summary>
        public cmsRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }
        public cmsRepository(string ConnectionString)
        {
            _context = ConnectionString;
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
                        Logo = s.c_logo,
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
                        Logo = s.c_logo,
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
                        .Update();

                    insertLog(user, ip, "update", item.Id, String.Empty, "Sites", item.Title);
                    return true;
                }
                else
                {
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
                            SiteOff = s.b_site_off
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
        public override bool updSite(Guid id, SitesModel upd, Guid UserId, String IP)
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

       

        #region Content links to objects

        /// Добавляем связи новостей и организаций
        public override bool updateContentLinks(ContentLinkModel data)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    //Получаем ссылку на того кому принадлежит content(кто создал новость/событие)
                    Guid contentLink = Guid.Empty;
                    if(data.ObjctType == ContentType.MATERIAL)
                    {
                        contentLink = db.content_materialss
                                                .Where(s => s.id == data.ObjctId)
                                                .Select(s => s.f_content_origin)
                                                .SingleOrDefault();
                    }
                    if (data.ObjctType == ContentType.EVENT)
                    {
                        contentLink = db.content_eventss
                                                .Where(s => s.id == data.ObjctId)
                                                .Select(s => s.f_content_origin)
                                                .SingleOrDefault();
                    }

                    //Удаляем существующие связи, кроме того объекта, которому новость/событие принадлежит.
                    db.content_content_links
                                            .Where(w => w.f_content.Equals(data.ObjctId))
                                            .Where(w => !w.f_link.Equals(contentLink))
                                            .Delete();

                    if (data.LinksId != null && data.LinksId.Count() > 0)
                    {
                        foreach (var link in data.LinksId)
                        {
                            if (link != contentLink)
                            {
                                //Добавляем дополнительные связи
                                db.content_content_links
                                               .Value(v => v.f_content, data.ObjctId)
                                               .Value(v => v.f_link, link)
                                               .Value(v => v.f_link_type, ContentLinkType.ORG.ToString().ToLower())
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
    }
}
