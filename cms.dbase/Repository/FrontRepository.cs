using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using cms.dbase.Mapping;

namespace cms.dbase
{
    public class FrontRepository : abstract_FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        private string _domain = string.Empty;

        //Создаем событие, на которое потом подпишемся
        public static event EventHandler<DislyEventArgs> DislyFrontEvent;
        private static void OnDislyEvent(DislyEventArgs eventArgs)
        {
            DislyFrontEvent(null, eventArgs);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

        public FrontRepository(string ConnectionString, string DomainUrl)
        {
            _context = ConnectionString;
            _domain = (!string.IsNullOrEmpty(DomainUrl)) ? getSiteId(DomainUrl) : "";
            _domain = "main";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

            _domain = (!string.IsNullOrEmpty(DomainUrl)) ? getSiteId(DomainUrl) : "";
            //_domain = "cheb-gkc";
        }

        #region redirect methods
        public override SitesModel getSiteInfoByOldId(int Id)
        {
            try
            {
                string domain = _domain;
                using (var db = new CMSdb(_context))
                {
                    var data = db.cms_sitess
                        .Where(w => w.c_alias.ToLower().Equals(domain))
                        .Select(s => new SitesModel
                        {
                            Id = s.id,
                            Title = s.c_name,
                            LongTitle = s.c_name_long,
                            Alias = s.c_alias.ToLower(),
                            Adress = s.c_adress,
                            Phone = s.c_phone,
                            Fax = s.c_fax,
                            Email = s.c_email,
                            Site = s.c_url,
                            Worktime = s.c_worktime,
                            Logo = new Photo
                            {
                                Url = s.c_logo
                            },
                            ContentId = (Guid)s.f_content,
                            ContentType = (ContentLinkType)Enum.Parse(typeof(ContentLinkType), s.c_content_type, true),
                            Type = s.c_content_type,
                            Facebook = s.c_facebook,
                            Vk = s.c_vk,
                            Instagramm = s.c_instagramm,
                            Odnoklassniki = s.c_odnoklassniki,
                            Twitter = s.c_twitter,
                            Theme = s.c_theme,
                            BackGroundImg = new Photo
                            {
                                Url = s.c_background_img
                            }
                        });

                    if (data.Any())
                        return data.SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> getSiteInfoByOldId for \"{0}\" oldid={}", _domain, Id);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }
            return null;
        }

        /// <summary>
        /// Элемент карты сайта по старому линку с ид
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override SiteMapModel getSiteMapByOldId(int id)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps.Where(w => w.n_old_id == id);
                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias.ToLower(),
                    Path = s.c_path,
                    Id = s.id,
                    FrontSection = s.f_front_section,
                    ParentId = s.uui_parent
                });

                if (data.Any())
                    return data.SingleOrDefault();

                return null;
            }
        }
        /// <summary>
        /// Новость по старому линку с ид
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override MaterialsModel getMaterialsByOldId(int id)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_materialss
                            .Where(w => w.n_old_id == id);

                if (query.Any())
                {
                    var material = query.Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Alias = s.c_alias.ToLower(),
                        Date = s.d_date,
                        Year = s.n_year,
                        Month = s.n_month,
                        Day = s.n_day,
                        PreviewImage = new Photo
                        {
                            Url = s.c_preview
                        }
                    }).SingleOrDefault();

                    db.content_materialss
                        .Where(w => w.id.Equals(material.Id))
                        .Set(u => u.n_count_see, u => u.n_count_see + 1)
                        .Update();

                    return material;
                }

                return null;
            }
        }
        #endregion

        /// <summary>
        /// Получение идентификатора сайта
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override string getSiteId(string domain)
        {
            try
            {
                if (string.IsNullOrEmpty(domain))
                    throw new Exception("FrontRepository: getSiteId Domain is empty!");

                using (var db = new CMSdb(_context))
                {
                    var data = db.cms_sites_domainss
                        .Where(w => w.c_domain == domain);

                    if (data.Any())
                    {
                        //Может быть найдено несколько записей по разным доменам, но ссылаются на один сайт
                        var _domain = data.FirstOrDefault();
                        return _domain.f_site;
                    }


                    //throw new Exception("FrontRepository: getSiteId Domain '" + domain + "' was not found!");
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("cmsRepository=> getSiteId for \"{0}\"", domain);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Error, message, ex));
            }
            return null;
        }

        /// <summary>
        /// Получение вьюхи
        /// </summary>
        /// <param name="siteSection">Секция</param>
        /// <returns></returns>
        public override SiteSectionModel getView(string siteSection) //string siteId,
        {
            string siteId = _domain;
            using (var db = new CMSdb(_context))
            {


                var query = (from s in db.front_site_sections
                             join v in db.front_page_viewss
                             on s.f_page_view equals v.id
                             where (s.f_site.Equals(siteId) && s.f_front_section.Equals(siteSection))
                             select v);
                if (query.Any())
                    return query.Select(s => new SiteSectionModel
                    {
                        Url = s.c_url,
                        UrlSpec = s.c_url_spec
                    }).FirstOrDefault();
                return null;
            }
        }

        /// <summary>
        /// Получение информации по сайту
        /// </summary>
        /// <returns></returns>
        public override SitesModel getSiteInfo(string domain = null)
        {
            if (string.IsNullOrEmpty(domain))
                domain = _domain;

            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess
                    .Where(w => w.c_alias.ToLower().Equals(domain))
                    .Select(s => new SitesModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        LongTitle = s.c_name_long,
                        Alias = s.c_alias.ToLower(),
                        Adress = s.c_adress,
                        Phone = s.c_phone,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Site = s.c_url,
                        Worktime = s.c_worktime,
                        Logo = new Photo
                        {
                            Url = s.c_logo
                        },
                        ContentId = (Guid)s.f_content,
                        ContentType = (ContentLinkType)Enum.Parse(typeof(ContentLinkType), s.c_content_type, true),
                        Type = s.c_content_type,
                        Scripts = s.c_scripts,
                        Facebook = s.c_facebook,
                        Vk = s.c_vk,
                        Instagramm = s.c_instagramm,
                        Odnoklassniki = s.c_odnoklassniki,
                        Twitter = s.c_twitter,
                        Theme = s.c_theme,
                        OldUrl=s.c_old_url,
                        DefaultDomain = getSiteDefaultDomain(domain),
                        BackGroundImg = new Photo
                        {
                            Url = s.c_background_img
                        }
                    });

                if (data.Any())
                    return data.SingleOrDefault();

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public override string getSiteDefaultDomainByContentId(Guid contentId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sites_domainss
                    .Where(w => w.fksitesdomains.f_content == contentId)
                    .Where(w => w.b_default == true);

                try
                {
                    return data.Select(p => p.c_domain).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    throw new Exception("FrontRepository > getSiteDefaultDomain : Обнаружено более одного домена по умолчанию " + contentId);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public override string getSiteDefaultDomain(string siteId)
        {
            if (string.IsNullOrEmpty(siteId))
                return null;

            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sites_domainss
                    .Where(w => w.f_site.ToLower() == siteId)
                    .Where(w => w.b_default == true);

                try
                {
                    return data.Select(p => p.c_domain).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    throw new Exception("FrontRepository > getSiteDefaultDomain : Обнаружено более одного домена по умолчанию " + siteId);
                }
            }
        }
        /// <summary>
        /// Получение информации по сайту
        /// </summary>
        /// <returns></returns>
        public override UsersModel[] getSiteAdmins()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_userss
                    .Where(w => w.f_group == "admin")
                    .Where(w => w.fklinkusertosites.Any(s => s.f_site == _domain))
                    .Select(s => new UsersModel
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                        EMail = s.c_email
                    });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получим список элементов карты сайта для контроллера
        /// </summary>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapListShort(string path)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => !w.b_disabled)
                    .Where(w => string.IsNullOrWhiteSpace(path) || w.c_path.Equals(path))
                    .OrderBy(o => o.c_path)
                    .ThenBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Title = s.c_title,
                        Path = s.c_path,
                        Alias = s.c_alias.ToLower(),
                        FrontSection = s.f_front_section
                    });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получение меню из карты сайта
        /// </summary>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapList()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_sitemap_menus
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => !w.b_disabled && !w.b_disabled_menu)
                    .Where(w => w.menu_alias != "plate") /*исключения*/
                    .OrderBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,
                        Site = s.f_site,
                        FrontSection = s.f_front_section,
                        Path = s.c_path,
                        Alias = s.c_alias.ToLower(),
                        Title = s.c_title,
                        //Text = s.c_text,
                        Preview = s.c_preview,
                        Url = s.c_url,
                        //Desc = s.c_desc,
                        //Keyw = s.c_keyw,
                        //Disabled = s.b_disabled,
                        //DisabledMenu = s.b_disabled_menu,
                        Sort = s.n_sort,
                        ParentId = s.uui_parent,
                        MenuAlias = s.menu_alias,
                        MenuSort = s.menu_sort,
                        MenuGroups = db.content_sitemap_menutypess
                                        .Where(w => w.f_sitemap.Equals(s.id))
                                        .Select(w => w.f_menutype.ToString())
                                        .ToArray(),
                        //MenuGroups = getSiteMapGroupMenu(s.id),
                        Photo = new Photo { Url = s.c_photo }
                    });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        public SiteMapModel[] getSiteMapList(string group)
        {            
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_sitemap_menus
                    .Where(w => w.f_site.Equals(_domain))
                    .Where(w => !w.b_disabled && !w.b_disabled_menu)
                    .Where(w=>w.menu_alias == group)
                    .OrderBy(o => o.menu_sort)
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,                        
                        FrontSection = s.f_front_section,
                        Path = s.c_path,
                        Alias = s.c_alias.ToLower(),
                        Title = s.c_title,                        
                        Preview = s.c_preview,
                        Url = s.c_url,                                                
                        ParentId = s.uui_parent,
                        MenuAlias = s.menu_alias,                        
                        MenuGroups = db.content_sitemap_menutypess
                                        .Where(w => w.f_sitemap.Equals(s.id))
                                        .Select(w => w.f_menutype.ToString())
                                        .ToArray(),                        
                        Photo = new Photo { Url = s.c_photo }
                    });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получение групп меню для элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор карты сайта</param>
        /// <returns></returns>
        public override string[] getSiteMapGroupMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemap_menutypess
                    .Where(w => w.f_sitemap.Equals(id))
                    .Select(s => s.f_menutype.ToString());

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }


        /// <summary>
        /// Получение списка баннеров
        /// </summary>
        /// <returns></returns>
        public override BannersModel[] getBanners()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.sv_sites_bannerss
                    .Where(b => b.site_alias == _domain)                    
                    .Where(b => b.banner_disabled == false)
                    .Where(b => b.banner_date_end > DateTime.Now || b.banner_date_end == null);


                    int itemCount = query.Count();

                    var list = query
                        .OrderBy(b => b.banner_sort)
                        .Select(s => new BannersModel()
                        {
                            Id = s.banner_id,
                            Title = s.banner_title,
                            Url = s.banner_url,
                            Text = s.banner_text,
                            Date = s.banner_date,
                            Sort = s.banner_sort,
                            SectionAlias = s.section_alias,
                            Photo = new Photo
                            {
                                Url = s.banner_image
                            }
                        });

                    return list.ToArray();
            }
        }

        public override BannersModel[] getBanners(string section)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.sv_sites_bannerss
                    .Where(b => b.site_alias == _domain)
                    .Where(b => b.section_alias == section)
                    .Where(b => b.banner_disabled == false)
                    .Where(b => 
                                (b.banner_date <= DateTime.Now && (b.banner_date_end > DateTime.Now || b.banner_date_end == null))
                                ||
                                (
                                    //признак ежегодности имеет смысл только в случае когда указана "дата окончания показа"
                                    b.b_annual==true 
                                    && b.banner_date_end!=null 
                                    && b.banner_date.DayOfYear<=DateTime.Now.DayOfYear 
                                    && ((DateTime)b.banner_date_end).DayOfYear>DateTime.Today.DayOfYear)
                                )
                    .OrderBy(o=>o.banner_sort);

                var list = query
                    .OrderBy(b => b.banner_sort)
                    .Select(s => new BannersModel()
                    {
                        Id = s.banner_id,
                        Title = s.banner_title,
                        Url = s.banner_url,
                        Text = s.banner_text,
                        //Date = s.banner_date,
                        //Sort = s.banner_sort,
                        //SectionAlias = s.section_alias,
                        Photo = new Photo
                        {
                            Url = s.banner_image
                        }
                    });
                if (list.Any())
                {
                    return list.ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// Получаем баннер
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override BannersModel getBanner(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new BannersModel
                    {
                        Id = s.id,
                        Url = s.c_url
                    });

                if (query.Any())
                {
                    db.content_bannerss
                        .Where(w => w.id.Equals(id))
                        .Set(u => u.n_count_click, u => u.n_count_click + 1)
                        .Update();

                    return query.SingleOrDefault();
                }

                return null;
            }
        }

        /// <summary>
        /// карта сайта
        /// </summary>
        /// <param name="path"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override SiteMapModel getSiteMap(string path, string alias) //, string domain
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site == domain)
                    .Where(w => w.b_disabled == false)
                    .Where(w => w.c_path.ToLower() == path.ToLower())
                    .Where(w => w.c_alias.ToLower() == alias.ToLower());

                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias.ToLower(),
                    Path = s.c_path,
                    Id = s.id,
                    FrontSection = s.f_front_section,
                    ParentId = s.uui_parent
                });

                if (data.Any())
                    return data.First();

                return null;
            }
        }

        /// <summary>
        /// Получим эл-т карты сайта по frontSection, что фактически является названием контроллера
        /// </summary>
        /// <param name="frontSection"></param>
        /// <returns></returns>
        public override SiteMapModel getSiteMap(string frontSection)
        {
            string domain = _domain;

            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site == domain)
                    .Where(w => w.b_disabled == false)
                    .Where(w => w.f_front_section.ToLower() == frontSection.ToLower());

                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias.ToLower(),
                    Path = s.c_path,
                    Id = s.id,
                    ParentId = s.uui_parent,
                    FrontSection = s.f_front_section
                });

                if (data.Any())
                    return data.First();

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override SiteMapModel getPageInfo(Guid id)
        {
            string domain = _domain;

            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site == domain)
                    .Where(w => w.b_disabled == false)
                    .Where(w => w.id == id);

                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias.ToLower(),
                    Path = s.c_path,
                    Id = s.id,
                    ParentId = s.uui_parent,
                    FrontSection = s.f_front_section
                });

                if (data.Any())
                    return data.First();

                return null;
            }
        }

        /// <summary>
        /// Получим сестринские эл-ты карты сайта по пути
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string[] getSiteMapSiblings(string path)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(_domain))
                    .Where(w => w.c_path.Equals(path))
                    .Select(s => s.c_alias.ToLower());

                if (!query.Any()) return null;
                return query.ToArray();
            }
        }

        /// <summary>
        /// Получим сестринские эл-ты из карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<SiteMapModel> getSiteMapSiblingElements(string path)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.b_disabled == false)
                    .Where(w => w.b_disabled_menu == false)
                    .Where(w => w.f_site.Equals(_domain))
                    .Where(w => w.c_path.Equals(path))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias.ToLower(),
                        Path = s.c_path,
                        FrontSection = s.f_front_section,
                        Url = s.c_url,
                        ParentId = s.uui_parent
                    });

                if (!query.Any()) return null;
                return query.ToList();
            }
        }

        /// <summary>
        /// Дочерние элементы
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapChild(Guid ParentId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                                 .Where(w => w.b_disabled == false)
                                 .Where(w => w.b_disabled_menu == false)
                                 .Where(w => w.uui_parent.Equals(ParentId))
                                 .Where(w => w.b_disabled_menu == false)
                                 .OrderBy(o => o.n_sort)
                                 .Select(c => new SiteMapModel
                                 {
                                     Title = c.c_title,
                                     Alias = c.c_alias.ToLower(),
                                     Path = c.c_path,
                                     FrontSection = c.f_front_section,
                                     Url = c.c_url
                                 });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Список прикрепленных лдокументов к элементу карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override DocumentsModel[] getAttachDocuments(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_documentss.Where(w => w.f_link == id)
                 .OrderBy(o => o.n_sort)
                 .Select(s => new DocumentsModel
                 {
                     id = s.id,
                     Title = s.c_title,
                     FilePath = s.c_file_path,
                     LinkId = s.f_link
                 });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получаем хлебные крошки
        /// </summary>
        /// <param name="Url">относительная ссылка на страницу</param>
        /// <returns></returns>
        public override List<Breadcrumbs> getBreadCrumbCollection(string Url)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                int _len = Url.Count();
                int _lastIndex = Url.LastIndexOf("/");
                List<Breadcrumbs> data = new List<Breadcrumbs>();

                while (_lastIndex > -1)
                {
                    string _path = Url.Substring(0, _lastIndex + 1).ToString();
                    string _alias = Url.Substring(_lastIndex + 1).ToString();
                    if (_alias == String.Empty) _alias = " ";

                    var getContentSitemaps = db.content_sitemaps
                                .Where(w => w.f_site == domain)
                                .Where(w => w.c_path == _path)
                                .Where(w => w.c_alias.ToLower() == _alias.ToLower())
                                //.Take(1)
                                .Select(w => new Breadcrumbs
                                {
                                    Title = w.c_title,
                                    Url = w.c_path + w.c_alias.ToLower()
                                });
                    if (getContentSitemaps.Any())
                        try
                        {
                            var itemContentSitemap = getContentSitemaps.SingleOrDefault();
                            if (itemContentSitemap != null)
                                data.Add(itemContentSitemap);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("FrontRepository > getBreadCrumbCollection: Found more than one record " + ex);
                        }
                    Url = Url.Substring(0, _lastIndex);
                    _len = Url.Count();
                    _lastIndex = Url.LastIndexOf("/");

                }
                if (data.Any())
                {
                    data.Reverse();
                    return data;
                }

                return data;
            }
        }

        /// <summary>
        /// Получаем новости для модуля на главной странице
        /// </summary>
        /// <returns></returns>
        public List<MaterialFrontModule> getMaterials(string group)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    var contentType = ContentType.MATERIAL.ToString().ToLower();

                    // список id-новостей для данного сайта

                    var materialIds = db.content_content_links
                        .Where(e => e.f_content_type == contentType)
                        .Where(e => db.cms_sitess.Any(t => t.c_alias.ToLower() == _domain && t.f_content == e.f_link))
                        .Select(e => e.f_content);
                    //.Join(db.cms_sitess.Where(o => o.c_alias.ToLower() == domain),
                    //        e => e.f_link,
                    //        o => o.f_content,
                    //        (e, o) => e.f_content
                    //        );

                    if (!materialIds.Any())
                        return null;

                    // список групп
                    var groups = db.content_materials_groupss
                        .Select(s => s.id).ToArray();

                    List<MaterialFrontModule> list = new List<MaterialFrontModule>();

                    foreach (var g in groups)
                    {
                        var query = db.content_sv_materials_groupss
                            .Where(w => materialIds.Contains(w.id))
                            .Where(w => w.group_id.Equals(g));

                        //если не анонсы
                        if (g != Guid.Parse("651CFEB9-E157-4F42-B40D-DE5A7DC1A8FC"))
                        {
                            query = query.Where(w => w.d_date <= DateTime.Now);
                        }

                        var data = query
                            .Where(w => w.b_disabled == false)
                            .OrderByDescending(o => o.d_date)
                            .Select(s => new MaterialFrontModule
                            {
                                Title = s.c_title,
                                Alias = s.c_alias.ToLower(),
                                Date = s.d_date,
                                GroupName = s.group_title,
                                GroupAlias = s.group_alias,
                                Photo = s.c_preview,
                                SmiType = s.c_smi_type
                            });


                        // берём последние 3 новости данной группы
                        if (data.Any())
                            list.AddRange(data.Take(2));
                    }

                    if (list.Any())
                        return list;
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> getMaterialsModule for \"{0}\" ", _domain);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }

            return null;
        }

        /// <summary>
        /// Получаем новости для модуля на главной странице
        /// </summary>
        /// <returns></returns>
        public override List<MaterialFrontModule> getMaterialsModule()
        {
            try
            {
                string domain = _domain;
                using (var db = new CMSdb(_context))
                {
                    var list = db.content_sv_last2materials_inallgroupss
                            .Where(s => s.c_domain == _domain)
                            .OrderByDescending(o=>o.d_date)
                            .Select(s => new MaterialFrontModule
                            {
                                Title = s.c_title,
                                Alias = s.c_alias,
                                Date = s.d_date,
                                GroupName = s.c_group_title,
                                GroupAlias = s.c_group_alias,
                                Photo = s.c_preview,
                                SmiType = s.c_smi_type
                            }).ToList();

                    return list;
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> getMaterialsModule for \"{0}\" ", _domain);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }

            return null;
        }
        public override IndexModel getMaterialsModuleNew()
        {
            try
            {
                string domain = _domain;
                IndexModel model = new IndexModel();
                using (var db = new CMSdb(_context))
                {
                    #region из пресс-центра


                    var list = db.content_sv_last2materials_inallgroupss
                            .Where(s => s.c_domain == _domain)
                            .OrderByDescending(o => o.d_date)
                            .Select(s => new MaterialFrontModule
                            {
                                Title = s.c_title,
                                Alias = s.c_alias,
                                Date = s.d_date,
                                GroupName = s.c_group_title,
                                GroupAlias = s.c_group_alias,
                                Photo = s.c_preview,
                                SmiType = s.c_smi_type
                            }).ToList();

                    model.ModuleAnnouncement = list.Where(w => w.GroupAlias == "announcement");
                    model.ModuleNews = list.Where(w => w.GroupAlias == "news");
                    model.ModuleActual = list.Where(w => w.GroupAlias == "actual");
                    model.ModuleEvents = list.Where(w => w.GroupAlias == "events");

                    if (domain == "main")
                    {
                        Guid ContentIdMain = Guid.Parse("cc8442ef-cece-4cd3-a544-da319e03c981");

                        #region newinmdicen
                        var contentType = ContentType.MATERIAL.ToString().ToLower();
                        List<MaterialFrontModule> listNew = new List<MaterialFrontModule>();

                        var query = db.content_sv_materials_groupss
                                 .Where(w => w.b_disabled == false)
                                 .Where(w => w.group_id.Equals(Guid.Parse("6303B7C5-5404-4EC0-AED2-1C308992C78A")))
                                 .Join(db.content_content_links.Where(w => w.f_link == ContentIdMain && w.f_content_type == contentType), n => n.id, m => m.f_content, (n, m) => n);

                        if (query.Any())
                        {
                            var data = query.OrderByDescending(o => o.d_date).Select(s => new MaterialFrontModule
                            {
                                Title = s.c_title,
                                Alias = s.c_alias.ToLower(),
                                Date = s.d_date,
                                GroupName = s.group_title,
                                GroupAlias = s.group_alias,
                                Photo = s.c_preview,
                                SmiType = s.c_smi_type
                            });
                            model.ModuleNewsWorld = data.Where(w => w.SmiType == "world").FirstOrDefault();
                            model.ModuleNewsChuv = data.Where(w => w.SmiType == "chuvashia").FirstOrDefault();
                            model.ModuleNewsRus = data.Where(w => w.SmiType == "russia").FirstOrDefault();
                        }
                        #endregion

                        #region important
                        var ImportantMater = db.content_content_links
                                            .Where(e => e.f_content_type == contentType)
                                            .Where(e => e.b_important == true)
                                            .Where(e => db.cms_sitess.Any(t => t.c_alias == _domain && t.f_content == e.f_link))
                                            .Select(e => e.f_content);
                        var queryImportantMater = db.content_materialss
                                      .Where(w => ImportantMater.Contains(w.id));
                        if (queryImportantMater.Any())
                        {
                            model.ImportantMaterials = queryImportantMater.Select(s => new MaterialFrontModule
                            {
                                Title = s.c_title,
                                Alias = s.c_alias,
                                Date = s.d_date,
                                Photo = s.c_preview
                            }).Single();
                        }
                        #endregion
                    }
                    else
                    {
                        model.ModulePhoto = list.Where(w => w.GroupAlias == "photo").FirstOrDefault();
                        model.ModuleVideo = list.Where(w => w.GroupAlias == "video").FirstOrDefault();
                    }
                    #endregion

                    #region banners

                    var queryBanners = db.sv_sites_bannerss
                    .Where(b => b.site_alias == _domain)
                    .Where(b => b.banner_disabled == false)
                    .Where(b => b.banner_date_end > DateTime.Now || b.banner_date_end == null);
                    if (queryBanners.Any())
                    {
                        model.BannerArrayIndex = queryBanners.OrderBy(b => b.banner_sort)
                                                             .Select(s => new BannersModel()
                                                             {
                                                                 Id = s.banner_id,
                                                                 Title = s.banner_title,
                                                                 Url = s.banner_url,
                                                                 Text = s.banner_text,
                                                                 Date = s.banner_date,
                                                                 Sort = s.banner_sort,
                                                                 SectionAlias = s.section_alias,
                                                                 Photo = new Photo
                                                                 {
                                                                     Url = s.banner_image
                                                                 }
                                                             }).ToArray();
                    }
                    #endregion

                    //#region plate
                    //var platepath = "plate";
                    //var queryplate = db.content_sitemaps
                    //                  .Where(w => w.f_site.Equals(domain))
                    //                  .Where(w => !w.b_disabled)
                    //                  .Where(w => string.IsNullOrWhiteSpace(platepath) || w.c_path.Equals(platepath));
                    //if (queryplate.Any())
                    //{
                    //    model.SitemapPlate= queryplate.OrderBy(o => o.c_path)
                    //                                  .ThenBy(o => o.n_sort)
                    //                                  .Select(s => new SiteMapModel
                    //                                  {
                    //                                      Title = s.c_title,
                    //                                      Path = s.c_path,
                    //                                      Alias = s.c_alias.ToLower(),
                    //                                      FrontSection = s.f_front_section
                    //                                  }).ToArray();
                    //}


                    //#endregion
                    model.SitemapPlate = getSiteMapList("plate");
                    model.BenifitBanners = getBanners("benefits");
                    if (domain != "main")
                    {
                        model.Slider = getBanners("slider");
                    }

                    return model;
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> getMaterialsModule for \"{0}\" ", _domain);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }

            return null;
        }



        /// <summary>
        /// Получаем новости группы 
        /// </summary>
        /// <returns></returns>
        //public override List<MaterialFrontModule> getMaterialsGroupNewInMedicin()
        //{
        //    string domain = _domain;
        //    using (var db = new CMSdb(_context))
        //    {
        //        var contentType = ContentType.MATERIAL.ToString().ToLower();

        //        // список id-новостей для данного сайта
        //        var materialIds = db.content_content_links.Where(e => e.f_content_type == contentType)
        //            .Join(db.cms_sitess.Where(o => o.c_alias.ToLower() == domain),
        //                    e => e.f_link,
        //                    o => o.f_content,
        //                    (e, o) => e.f_content
        //                    ).ToArray();

        //        if (!materialIds.Any())
        //            return null;



        //        List<MaterialFrontModule> list = new List<MaterialFrontModule>();

        //        var query = db.content_sv_materials_groupss
        //                 .Where(w => materialIds.Contains(w.id))
        //                 .Where(w => w.b_disabled == false)
        //                 .Where(w => w.group_id.Equals(Guid.Parse("6303B7C5-5404-4EC0-AED2-1C308992C78A")));





        //        if (query.Any())
        //        {
        //            var data = query.OrderByDescending(o => o.d_date).Select(s => new MaterialFrontModule
        //            {
        //                Title = s.c_title,
        //                Alias = s.c_alias.ToLower(),
        //                Date = s.d_date,
        //                GroupName = s.group_title,
        //                GroupAlias = s.group_alias,
        //                Photo = s.c_preview,
        //                SmiType = s.c_smi_type
        //            });
        //            var world = data.Where(w => w.SmiType == "world");
        //            if (world.Any())
        //            {
        //                list.AddRange(world.Take(1));
        //            }
        //            var rus = data.Where(w => w.SmiType == "russia");
        //            if (rus.Any())
        //            {
        //                list.AddRange(rus.Take(1));
        //            }
        //            var chuv = data.Where(w => w.SmiType == "chuvashia");
        //            if (world.Any())
        //            {
        //                list.AddRange(chuv.Take(1));
        //            }

        //            return list;
        //        }
        //        return null;
        //    }
        //}

        public override List<MaterialFrontModule> getMaterialsGroupNewInMedicin()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var contentType = ContentType.MATERIAL.ToString().ToLower();
                var fcontent = db.cms_sitess.Where(w => w.c_alias == domain).Single().f_content;

                List<MaterialFrontModule> list = new List<MaterialFrontModule>();

                var query = db.content_sv_materials_groupss                         
                         .Where(w => w.b_disabled == false)
                         .Where(w => w.group_id.Equals(Guid.Parse("6303B7C5-5404-4EC0-AED2-1C308992C78A")))
                         .Join(db.content_content_links.Where(w=>w.f_link== fcontent && w.f_content_type== contentType),n=>n.id,m=>m.f_content,(n,m)=>n);                

                if (query.Any())
                {
                    var data = query.OrderByDescending(o => o.d_date).Select(s => new MaterialFrontModule
                    {
                        Title = s.c_title,
                        Alias = s.c_alias.ToLower(),
                        Date = s.d_date,
                        GroupName = s.group_title,
                        GroupAlias = s.group_alias,
                        Photo = s.c_preview,
                        SmiType = s.c_smi_type
                    });
                    var world = data.Where(w => w.SmiType == "world");
                    if (world.Any())
                    {
                        list.AddRange(world.Take(1));
                    }
                    var rus = data.Where(w => w.SmiType == "russia");
                    if (rus.Any())
                    {
                        list.AddRange(rus.Take(1));
                    }
                    var chuv = data.Where(w => w.SmiType == "chuvashia");
                    if (world.Any())
                    {
                        list.AddRange(chuv.Take(1));
                    }

                    return list;
                }
                return null;
            }
        }


        /// <summary>
        /// Получаем главную новость сайта
        /// </summary>
        /// <returns></returns>
        public override MaterialFrontModule getMaterialsImportant()
        {
            using (var db = new CMSdb(_context))
            {
                var contentType = ContentType.MATERIAL.ToString().ToLower();

                var materials = db.content_content_links
                    .Where(e => e.f_content_type == contentType)
                    .Where(e => e.b_important == true)
                    .Where(e => db.cms_sitess.Any(t => t.c_alias== _domain && t.f_content == e.f_link))
                    .Select(e => e.f_content);

                if (!materials.Any())
                    return null;

                var query = db.content_materialss
                        .Where(w => materials.Contains(w.id));

                if (query.Any())
                {
                    return query.Select(s => new MaterialFrontModule
                    {
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Date = s.d_date,
                        Photo = s.c_preview
                    }).Single();
                }
                return null;
            }
        }


        /// <summary>
        /// Получим список новостей для определенной сущности
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(MaterialFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrEmpty(filter.Domain))
                {
                    var contentType = ContentType.MATERIAL.ToString().ToLower();

                    //Запрос типа:
                    //Select t.*, s.* from[dbo].[content_content_link] t left join[dbo].[cms_sites] s
                    //on t.f_link = s.f_content Where s.c_alias = 'main'
                    var materials = db.content_content_links.Where(e => e.f_content_type == contentType)
                        .Join(db.cms_sitess.Where(o => o.c_alias.ToLower() == filter.Domain),
                                e => e.f_link,
                                o => o.f_content,
                                (e, o) => e.f_content
                                );

                    if (!materials.Any())
                        return null;

                    var query = db.content_materialss
                                .Where(w => materials.Contains(w.id));

                    if (filter.Disabled != null)
                    {
                        query = query.Where(w => w.b_disabled == filter.Disabled);
                    }
                    if (!String.IsNullOrEmpty(filter.SearchText))
                    {
                        query = query.Where(w => w.c_title.ToLower().Contains(filter.SearchText.ToLower()));
                    }
                    if (filter.Date != null)
                    {
                        query = query.Where(w => w.d_date >= filter.Date);
                    }
                    if (filter.DateEnd != null)
                    {
                        query = query.Where(w => w.d_date <= filter.DateEnd);
                    }

                    if (!String.IsNullOrEmpty(filter.SmiType))
                    {
                        query = query.Where(w => w.c_smi_type == filter.SmiType);
                    }

                    if (!String.IsNullOrEmpty(filter.Category))
                    {
                        if (filter.Category != "announcement")
                        {
                            query = query.Where(w => w.d_date <= DateTime.Now);
                        }
                        query = query.Where(g => db.content_materials_groups_links
                                      .Any(t => t.fkcontentmaterialsgroups.c_alias == filter.Category && t.f_material == g.id));
                        //var category = db.content_materials_groupss.Where(w => w.c_alias== filter.Category);
                        //if (category.Any())
                        //{
                        //    var cat = category.First().id;
                        //    query = query
                        //                .Join(
                        //                        db.content_materials_groups_links
                        //                        .Where(o => o.f_group == cat),
                        //                        e => e.id, o => o.f_material, (o, e) => o
                        //                     );
                        //    //query = query.Where(w => w.d_date <= DateTime.Now && w.);
                        //}

                    }

                    query = query.OrderByDescending(w => w.d_date);

                    int itemCount = query.Count();

                    var materialsList = query
                            .Skip(filter.Size * (filter.Page - 1))
                            .Take(filter.Size)
                            .Select(s => new MaterialsModel
                            {
                                Id = s.id,
                                Title = s.c_title,
                                Alias = s.c_alias.ToLower(),
                                Year = s.n_year,
                                Month = s.n_month,
                                Day = s.n_day,
                                PreviewImage = new Photo()
                                {
                                    Url = s.c_preview
                                },
                                Text = s.c_text,
                                Url = s.c_url,
                                UrlName = s.c_url_name,
                                Date = s.d_date,
                                Keyw = s.c_keyw,
                                Desc = s.c_desc,
                                Disabled = s.b_disabled,
                                Important = s.b_important
                            });

                    if (materialsList.Any())
                        return new MaterialsList()
                        {
                            Data = materialsList.ToArray(),
                            Pager = new Pager()
                            {
                                Page = filter.Page,
                                Size = filter.Size,
                                ItemsCount = itemCount,
                                //PageCount = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                            }
                        };
                }

                return null;
            }
        }

        /// <summary>
        /// Новость
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override MaterialsModel getMaterialsItem(string year, string month, string day, string alias)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                int _year = Convert.ToInt32(year);
                int _month = Convert.ToInt32(month);
                int _day = Convert.ToInt32(day);


                var contentType = ContentType.MATERIAL.ToString().ToLower();


                var materials = db.content_content_links.Where(e => e.f_content_type == contentType)
                                                        .Join(db.cms_sitess.Where(o => o.c_alias.ToLower() == domain),
                                                                   e => e.f_link,
                                                                   o => o.f_content,
                                                                   (e, o) => e.f_content
                                                               );

                if (!materials.Any())
                    return null;

                var query = db.content_materialss
                            .Where(w => materials.Contains(w.id));



                query = query.Where(w => (w.n_year == _year) && (w.n_month == _month) && (w.n_day == _day) && (w.c_alias.ToLower() == alias.ToLower()));
                if (query.Any())
                {
                    var material = query.Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Url = s.c_url,
                        UrlName = s.c_url_name,
                        Date = s.d_date,
                        PreviewImage = new Photo
                        {
                            Url = s.c_preview
                        }
                    }).SingleOrDefault();

                    db.content_materialss
                        .Where(w => w.id.Equals(material.Id))
                        .Set(u => u.n_count_see, u => u.n_count_see + 1)
                        .Update();

                    return material;
                }

                return null;
            }
        }

        /// <summary>
        /// Выдает группы преесс-центра
        /// </summary>
        /// <returns></returns>
        public override MaterialsGroup[] getMaterialsGroup()
        {
            using (var db = new CMSdb(_context))
            {
                //var data = db.content_materials_groupss;
                //if (data.Any())
                //{
                //    return data.OrderBy(o => o.n_sort)
                //               .Select(s => new MaterialsGroup
                //               {
                //                   Alias = s.c_alias,
                //                   Title = s.c_title
                //               }).ToArray();
                //}
                //return null;

                var query = db.content_sv_materials_groups_for_sites
                    .Where(w => w.domain.Equals(_domain))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new MaterialsGroup
                    {
                        Alias = s.c_alias.ToLower(),
                        Title = s.c_title
                    });

                if (query.Any()) return query.ToArray();
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override StructureModel[] getStructureList()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sv_structure_list_sites.Where(w => w.site_alias == _domain);
                if (query.Any())
                {
                    return query
                        .OrderBy(o => o.n_sort)
                        .Select(s => new StructureModel()
                        {
                            Title = s.c_title,
                            TitleShort = s.c_title_short,
                            Num = s.num
                        }).ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// Список структурных подразделений
        /// </summary>
        /// <returns></returns>
        public override StructureModel[] getStructures() //string domain
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = from str in db.content_org_structures
                            join site in db.cms_sitess on str.f_ord equals site.f_content
                            join dep in db.content_departmentss on str.id equals dep.f_structure
                            orderby str.n_sort
                            where site.c_alias.ToLower().Equals(domain)
                            select new { str, dep };

                var data = query.ToArray()
                    .GroupBy(p => new { p.str.id })
                    .Select(s => new StructureModel
                    {
                        Id = s.Key.id,
                        Title = s.First().str.c_title,
                        TitleShort = s.First().str.c_title_short,
                        Phone = s.First().str.c_phone,
                        PhoneReception = s.First().str.c_phone_reception,
                        Email = s.First().str.c_email,
                        Num = s.First().str.num,
                        GeopointX = s.First().str.n_geopoint_x,
                        GeopointY = s.First().str.n_geopoint_y,
                        Ovp = s.First().str.b_ovp,
                        Adress = s.First().str.c_adress,
                        Routes = s.First().str.c_routes,
                        DopAddres = getDopAddresStructur(s.First().str.id),
                        //DopAddres = (from ad in db.content_org_structure_adresss
                        //             where ad.f_org_structure==s.First().str.id
                        //             select new DopAddres
                        //             {
                        //                 Address = ad.c_adress,
                        //                 GeopointX = ad.n_geopoint_x,
                        //                 GeopointY = ad.n_geopoint_y
                        //             }).ToArray(),
                        Departments = s.OrderBy(o => o.dep.n_sort).Select(d => new Departments
                        {
                            Id = d.dep.id,
                            Title = d.dep.c_title,
                            Phones = (from p in db.content_departments_phones
                                      join dep in db.content_departmentss on p.f_department equals dep.id
                                      where p.f_department.Equals(d.dep.id)
                                      select new DepartmentsPhone
                                      {
                                          Label = p.c_key,
                                          Value = p.c_val
                                      }).ToArray()
                        }).ToArray()
                    });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }


        public override DopAddres[] getDopAddresStructur(Guid StrucId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_org_structure_adresss.Where(w => w.f_org_structure == StrucId);
                if (query.Any())
                {
                    query = query.OrderBy(o => new { o.c_title, o.c_adress });
                    return query.Select(s => new DopAddres()
                    {
                        Id = s.id,
                        Address = s.c_adress,
                        GeopointX = s.n_geopoint_x,
                        GeopointY = s.n_geopoint_y,
                        Title = s.c_title
                    }).ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// Отдельная структура
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public override StructureModel getStructureItem(int num) //string domain,
        {
            try
            {
                string domain = _domain;
                using (var db = new CMSdb(_context))
                {
                    var data = db.content_org_structures.Where(w => w.num == num)
                               .Join(db.cms_sitess.Where(o => o.c_alias.ToLower() == domain), o => o.f_ord, e => e.f_content, (e, o) => e)
                               .OrderBy(e => e.n_sort)
                               .Select(s => new StructureModel()
                               {
                                   Id = s.id,
                                   Num = s.num,
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
                                   Ovp = s.b_ovp
                               });

                    if (data.Any())
                        return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> getStructureItem for \"{0}\" num={}", _domain, num);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }

            return null;
        }

        /// <summary>
        /// Получаем список департаментов
        /// </summary>
        /// <param name="StructureId"></param>
        /// <returns></returns>
        public override Departments[] getDepartmentsList(Guid StructureId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_departmentss
                            .Where(w => w.f_structure == StructureId)
                            .OrderBy(o => o.n_sort)
                            .Select(s => new Departments()
                            {
                                Id = s.id,
                                Title = s.c_title
                            });
                if (query.Any())
                {
                    return query.ToArray();
                }

                return null;
            }
        }

        /// <summary>
        /// Получаем департамент
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public override Departments getDepartmentsItem(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_departmentss
                            .Where(w => w.id == Id)
                            .Select(s => new Departments()
                            {
                                Id = s.id,
                                Title = s.c_title,
                                Text = s.c_adress
                            });
                if (query.Any())
                {
                    var data = query.First();
                    var Phones = db.content_departments_phones
                                  .Where(w => w.f_department == data.Id)
                                  .OrderBy(o => o.n_sort)
                                  .Select(s => new DepartmentsPhone()
                                  {
                                      Label = s.c_key,
                                      Value = s.c_val
                                  });
                    if (Phones.Any())
                    {
                        data.Phones = Phones.ToArray();
                    }
                    var People = db.content_sv_employees_departments.Where(w => w.f_department == Id).OrderBy(o => new { o.c_surname, o.c_name, o.c_patronymic })
                                    .Select(s => new PeopleModel()
                                    {
                                        Id = s.id,
                                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                                        Post = s.c_post,
                                        Status = s.c_status,
                                        Photo = s.c_photo
                                    });

                    if (People.Any())
                    {
                        data.Boss = People.Where(w => w.Status == "boss").ToArray();
                        data.Sister = People.Where(w => w.Status == "sister").ToArray();
                    }
                    return data;
                }
                return null;
            }

        }

        /// <summary>
        /// Получаем ОВП
        /// </summary>
        /// <param name="Id">идентификатор струкутуры(родителя)</param>
        /// <returns></returns>
        public override Departments getOvpDepartaments(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_departmentss
                            .Where(w => w.f_structure == Id)
                            .OrderBy(o => o.c_title)
                            .Select(s => new Departments()
                            {
                                Id = s.id,
                                Title = s.c_title,
                                Text = s.c_adress
                            });
                if (query.Any())
                {
                    var data = query.First();
                    var Phones = db.content_departments_phones
                                  .Where(w => w.f_department == data.Id)
                                  .OrderBy(o => o.n_sort)
                                  .Select(s => new DepartmentsPhone()
                                  {
                                      Label = s.c_key,
                                      Value = s.c_val
                                  });
                    if (Phones.Any())
                    {
                        data.Phones = Phones.ToArray();
                    }


                    var People = db.content_sv_employees_departments.Where(w => w.f_department == data.Id).OrderBy(o => new { o.c_surname, o.c_name, o.c_patronymic })
                                .Select(s => new PeopleModel()
                                {
                                    Id = s.id,
                                    FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                                    Post = s.c_post,
                                    Status = s.c_status,
                                    Photo = s.c_photo
                                });

                    if (People.Any())
                    {
                        data.Boss = People.Where(w => w.Status == "boss").ToArray();
                        data.Sister = People.Where(w => w.Status == "sister").ToArray();
                    }
                    return data;
                }

                return null;
            }
        }

        /// <summary>
        /// Список врачей
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        public override PeopleModel[] getPeopleList(PeopleFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                string domain = filter.Domain; // домен

                Guid department = !string.IsNullOrWhiteSpace(filter.Group) // департамент
                    ? Guid.Parse(filter.Group) : Guid.Empty;

                var search = !string.IsNullOrWhiteSpace(filter.SearchText)
                    ? filter.SearchText.ToLower().Split(' ') : null; // поиск по человеку

                var people = (from p in db.content_peoples
                              join pol in db.content_org_employeess on p.id equals pol.f_people
                              where !pol.b_dismissed
                              join o in db.content_orgss on pol.f_org equals o.id
                              join s in db.cms_sitess on pol.f_org equals s.f_content into ps
                              from s in ps.DefaultIfEmpty()
                              select new { p, pol, s, o });

                if (filter.Id != null && filter.Id.Count() > 0)
                {
                    people = people.Where(w => w.p.employeespostspeoples.Any(q => filter.Id.Contains(q.f_people)));
                }

                if (search != null)
                {
                    foreach (string item in search)
                    {
                        people = people.Where(w => w.p.c_surname.Contains(item)
                                                || w.p.c_name.Contains(item)
                                                || w.p.c_patronymic.Contains(item));
                    }
                }

                int specialization = !string.IsNullOrWhiteSpace(filter.Type) ? Convert.ToInt32(filter.Type) : 0; // специализация

                var data = (from p in people
                            join pepl in db.content_org_employees_postss on p.p.id equals pepl.f_people
                            join ep in db.content_specializationss on pepl.f_post equals ep.id
                            join pdl in db.content_department_employeess on p.pol.id equals pdl.f_employee into ps
                            from pdl in ps.DefaultIfEmpty()
                                //where p.s.c_alias.Equals(domain) &&
                            where (department.Equals(Guid.Empty) || pdl.f_department.Equals(department))
                                    && ep.b_doctor
                                    && (specialization == 0 || pepl.f_post.Equals(specialization))
                            orderby ep.id, p.p.c_surname, p.p.c_name, p.p.c_patronymic, pepl.n_type
                            select new { p, ep, pepl });

                if (!string.IsNullOrEmpty(domain))
                {
                    data = data.Where(n => n.p.s.c_alias.ToLower() == domain);
                }

                if (filter.Specializations != null && filter.Specializations.Count() > 0)
                {
                    data = data.Where(n => filter.Specializations.Contains(n.ep.id));
                }

                var data2 = data.ToArray()
                    .GroupBy(g => new { g.p.p.id })
                    .Select(s => new PeopleModel
                    {
                        Id = s.Key.id,
                        FIO = s.First().p.p.c_surname + " " + s.First().p.p.c_name + " " + s.First().p.p.c_patronymic,
                        Photo = s.First().p.p.c_photo,
                        SNILS = s.First().p.p.c_snils,
                        Posts = s.Select(ep2 => new Specialisation
                        {
                            Id = ep2.ep.id,
                            Name = ep2.ep.c_name,
                            Org = (ep2.p.o.id != null) ? getOrgItemShort(ep2.p.o.id) : null,
                            Type = ep2.pepl.n_type
                        }).ToArray()
                    }).OrderBy(o => o.FIO);

                if (data2.Any())
                    return data2.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Список врачей Организации
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        public override PeopleList getOrgPeopleList(PeopleFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                string domain = filter.Domain; // домен

                #region comments
                //var people = db.content_peoples.AsQueryable();

                //if (!String.IsNullOrEmpty(domain))
                //{
                //    var contentId = db.cms_sitess
                //                        .Where(w => w.c_alias.Equals(domain))
                //                        .Select(s => s.f_content)
                //                        .SingleOrDefault();

                //    people = people.Where(w => w.contentpeopleorglinks.Any(a => a.contentorgpeoplelink.id.Equals(contentId)));

                //    var queryData = FindPeoplesQuery(people, filter);

                //    var result = queryData
                //             .Where(w => w.employeespostspeoples.Any(b => b.employeespostsspecializations.b_doctor))
                //             .Where(w => w.contentpeopleorglinks.Any(a => !a.b_dismissed))
                //             .OrderBy(o => new { o.c_surname, o.c_name, o.c_patronymic })
                //             .Select(s => new PeopleModel
                //             {
                //                 Id = s.id,
                //                 FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                //                 Photo = s.c_photo,
                //                 SNILS = s.c_snils,
                //                 Posts = (from sp in db.content_specializationss
                //                         join oep in db.content_org_employees_postss on sp.id equals oep.f_post
                //                         join oe in db.content_org_employeess on oep.f_employee equals oe.id
                //                         where oe.f_org.Equals(contentId) && oe.f_people.Equals(s.id)
                //                         select new Specialisation
                //                         {
                //                             Name = sp.c_name
                //                         }).GroupBy(g => g.Name).Select(t => t.First()).ToArray()
                //             });

                //    int itemCount = result.Count();
                //    result = result
                //                .Skip(filter.Size * (filter.Page - 1))
                //                .Take(filter.Size);

                //    if (result.Any())
                //    {
                //        return new PeopleList()
                //        {
                //            Doctors = result.ToArray(),
                //            Pager = new Pager
                //            {
                //                Page = filter.Page,
                //                Size = filter.Size,
                //                ItemsCount = itemCount
                //            }
                //        };
                //    }
                //    return null;
                //}
                #endregion

                if (!String.IsNullOrWhiteSpace(domain))
                {
                    Guid? siteId = db.cms_sitess
                                        .Where(w => w.c_alias.Equals(domain))
                                        .Select(s => s.f_content)
                                        .SingleOrDefault();

                    var links = db.test_people_post_linkss
                        .Where(w => w.f_org == siteId)
                        .Where(w => w.testpeoplepostlinkstestposts.b_doctor)
                        .Where(w => !w.b_deleted)
                        ;
                    
                    #region filter

                    if (!String.IsNullOrWhiteSpace(filter.SearchText))
                    {
                        var search = filter.SearchText.ToLower().Split(' ');

                        foreach (var s in search)
                        {
                            links = links
                                .Where(w => w.testpeoplepostlinkstestpeoples.c_surname.Contains(s)
                                        || w.testpeoplepostlinkstestpeoples.c_name.Contains(s)
                                        || w.testpeoplepostlinkstestpeoples.c_patronymic.Contains(s));
                        }
                    }
                    if (!String.IsNullOrEmpty(filter.Type))
                    {
                        links = links
                            .Where(w => w.testpeoplepostlinkstestposts.id == Int32.Parse(filter.Type));
                    }
                    if (!String.IsNullOrWhiteSpace(filter.Group))
                    {
                        links = links
                            .Where(w => w.testpeoplepostlinkstestpeoples.contentpeopleorglinks.Any(a => a.fkcontentdepartmentpeoplelinks.Any(b => b.f_department == Guid.Parse(filter.Group))));
                    }
                    if (filter.Specializations != null && filter.Specializations.Count() > 0)
                    {
                        links = links.Where(w => filter.Specializations.Contains(w.f_post));
                    }

                    #endregion

                    var people = links
                        .Select(s => new
                        {
                            Id = s.f_people,
                            FIO = $"{s.testpeoplepostlinkstestpeoples.c_surname} {s.testpeoplepostlinkstestpeoples.c_name} {s.testpeoplepostlinkstestpeoples.c_patronymic}",
                            Photo = s.testpeoplepostlinkstestpeoples.c_photo,
                            SNILS = s.testpeoplepostlinkstestpeoples.c_snils,
                            Post = s.testpeoplepostlinkstestposts.name
                        });

                    var p = people.ToArray()
                        .GroupBy(g => new { g.Id })
                        .Select(s => new PeopleModel
                        {
                            Id = s.Key.Id,
                            FIO = s.First().FIO,
                            Photo = s.First().Photo,
                            SNILS = s.First().SNILS,
                            Posts = s.Select(e => new Specialisation
                            {
                                Name = e.Post
                            }).ToArray()
                        }).OrderBy(o => o.FIO);


                    int itemCount = p.Count();
                    var tt = p.Skip(filter.Size * (filter.Page - 1))
                             .Take(filter.Size);

                    if (tt.Any())
                    {
                        return new PeopleList()
                        {
                            Doctors = tt.ToArray(),
                            Pager = new Pager
                            {
                                Page = filter.Page,
                                Size = filter.Size,
                                ItemsCount = itemCount
                            }
                        };
                    }
                    return null;
                }

                return null;
            }
        }

        /// <summary>
        /// Получает отдельного сотрудника
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override PeopleModel getPeopleItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_peoples
                    .Where(w => w.id.Equals(id))
                    .Where(w => w.testpeoplepostlinkstestpeopless.Any(a => !a.b_deleted))
                    .Select(s => new PeopleModel
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                        Photo = s.c_photo,
                        Posts = s.contentpeopleorglinks
                                    .Where(w => w.contentpeopleorglink.id.Equals(s.id))
                                    .Select(g => new Specialisation
                                    {
                                        Org = new OrgsShortModel
                                        {
                                            Title = g.contentorgpeoplelink.c_title,
                                            Url = db.cms_sites_domainss
                                                    .Where(w => w.fksitesdomains.f_content.Equals(g.f_org))
                                                    .Select(m => m.c_domain)
                                                    .FirstOrDefault()
                                        }
                                    }).ToArray(),
                        XmlInfo = s.contentpeopleinfocontentpeoples
                                        .Where(w => w.f_people.Equals(s.id))
                                        .Select(d => d.c_xml)
                                        .ToArray()
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Получаем СНИЛС для редиректа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override string getPeopleSnils(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_peoples
                    .Where(w => w.id.Equals(id))
                    .Select(s => s.c_snils);

                if (query.Any())
                    return query.SingleOrDefault();

                return null;
            }
        }

        /// <summary>
        /// сгруппированные по структурам департменты для выпадающего спика
        /// </summary>
        /// <returns></returns>
        public override StructureModel[] getDeparatamentsSelectList()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.c_alias.ToLower() == domain)
                           .Join(db.content_orgss, e => e.f_content, o => o.id, (e, o) => o)
                           .Join(db.content_org_structures, n => n.id, m => m.f_ord, (n, m) => m)
                           .Select(s => new StructureModel()
                           {
                               Title = s.c_title,
                               Departments = getDepartmentsList(s.id)
                           });



                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Список специализаций/должностей
        /// </summary>
        /// <returns></returns>
        public override Specialisation[] getSpecialisations()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = (from s in db.cms_sitess
                             join pol in db.content_org_employeess on s.f_content equals pol.f_org
                             join pepl in db.content_org_employees_postss on pol.f_people equals pepl.f_people
                             join ep in db.content_specializationss on pepl.f_post equals ep.id
                             where (domain.Equals("main") || s.c_alias.ToLower().Equals(domain)) && ep.b_doctor
                             select new Specialisation
                             {
                                 Id = ep.id,
                                 Parent = ep.n_parent,
                                 Name = ep.c_name
                             });

                var data = query.GroupBy(x => x.Id).Select(s => s.First());

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Список специализаций/должностей
        /// </summary>
        /// <returns></returns>
        public override Specialisation[] getSpecialisations(SpecialisationFilter filtr)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_specializationss.
                    Where(s => s.b_doctor);

                if (filtr.Specializations != null && filtr.Specializations.Count() > 0)
                {
                    query = query.Where(s => filtr.Specializations.Contains(s.id));

                }

                if (filtr.PeopleId.HasValue)
                {
                    query = query.Where(s => s.employeespostsspecializationss.Any(p => p.f_people == filtr.PeopleId.Value))
                        .Where(s => s.employeespostsspecializationss.Any(p => !p.b_dissmissed));

                }

                var data = query.Select(s => new Specialisation
                {
                    Id = s.id,
                    Parent = s.n_parent,
                    Name = s.c_name
                });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peopleId"></param>
        /// <returns></returns>
        public override Specialisation[] getPeopleSpecialisations(Guid peopleId)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_org_employees_postss
                    .Where(s => s.f_people == peopleId)
                    .Where(s => !s.b_dissmissed);

                var data = query.Select(s => new Specialisation
                {
                    Id = s.employeespostsspecializations.id,
                    Parent = s.employeespostsspecializations.n_parent,
                    Name = s.employeespostsspecializations.c_name,
                    Org = new OrgsShortModel()
                    {
                        Id = (Guid)s.f_org
                    }
                });

                if (data.Any())
                    return data.ToArray();

                return null;
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
                if (filtr == null)
                    throw new Exception("cmsRepository > queryByOrgFilter: Filter is null");

                var query = db.content_orgss
                            .Where(o => o.contentemployeespostsorgss.Any(p => !p.b_dissmissed));


                if (filtr.Disabled != null && (bool)filtr.Disabled)
                    query = query.Where(w => w.b_disabled);

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

                if (filtr.PeopleId.HasValue && filtr.PeopleId.Value != Guid.Empty)
                {
                    query = query.Where(p => p.contentorgpeoplelinks.Any(s => s.f_people == filtr.PeopleId.Value));
                }

                if (filtr.Except.HasValue && filtr.Except.Value != Guid.Empty)
                    query = query.Where(w => w.id != filtr.Except.Value);

                query = query.OrderBy(o => o.n_sort);

                var data = query.Select(s => new OrgsModel()
                {
                    Id = s.id,
                    Title = s.c_title,
                    ShortTitle = s.c_title_short,
                    Sort = s.n_sort,
                    Address = s.c_adress,
                    Phone = s.c_phone,
                    Fax = s.c_fax,
                    Url = getSiteDefaultDomainByContentId(s.id),
                    ExtUrl = s.c_www
                });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получем организацию
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override OrgsModel getOrgInfo(string siteId)
        {
            string domain = !string.IsNullOrEmpty(siteId) ? siteId : _domain;

            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.c_alias == domain)
                             .Join(db.content_orgss, e => e.f_content, o => o.id, (e, o) => o)
                             .Select(s => new OrgsModel()
                             {
                                 Title = !string.IsNullOrEmpty(s.c_title_short) ? s.c_title_short : s.c_title,
                                 Address = s.c_adress,
                                 Phone = s.c_phone,
                                 Fax = s.c_fax,
                                 Email = s.c_email,
                                 PhoneReception = s.c_phone_reception,
                                 GeopointX = s.n_geopoint_x,
                                 GeopointY = s.n_geopoint_y
                             });


                //if (data.Any())
                    return data.FirstOrDefault();

                //var data = from s in db.cms_sitess
                //           join o in db.content_orgss on s.f_content equals o.id //into ps from pdl in ps.DefaultIfEmpty()
                //           where s.c_alias == domain
                //           select new OrgsModel
                //           {
                //               Title = !string.IsNullOrEmpty(o.c_title_short) ? o.c_title_short : o.c_title,
                //               Address = s.c_adress,
                //               Phone = s.c_phone,
                //               Fax = s.c_fax,
                //               Email = s.c_email,
                //               PhoneReception = o.c_phone_reception,
                //               GeopointX = o.n_geopoint_x,
                //               GeopointY = o.n_geopoint_y
                //           };

                //if (data != null)
                //{
                //    var res = data.SingleOrDefault();
                //    return res;
                //}

                //return null;
            }
        }


        /// <summary>
        /// Получем список голосования
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public override IEnumerable<VoteModel> getVote(string Ip)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                //так добились  того чтобы голосования "тянулись" с главного сайта
                domain = "main";
                var query = db.content_votes
                            .Where(w => w.f_site == domain && w.b_disabled == false)
                            .OrderByDescending(o => o.d_date_start)
                            .Select(s => new VoteModel()
                            {
                                Id = s.id,
                                Header = s.c_header,
                                Text = s.c_text,
                                Type = s.b_type,
                                DateStart = s.d_date_start,
                                DateEnd = s.d_date_end,
                                Answer = getVoteAnswer(s.id, Ip),
                                ShowStatistic = ShowStatic(s.id, Ip)
                            });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// определяем показывать статистику или форму голосования
        /// </summary>
        /// <param name="VoteId"></param>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public bool ShowStatic(Guid VoteId, string Ip)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_votes.Where(w => w.id == VoteId);

                //если опрос завершен по дате
                if (data.Single().d_date_end <= DateTime.Now)
                    return true;

                //если пользователь уже принял участие в опросе
                var _count = db.content_vote_userss.Where(w => w.f_vote == VoteId && w.c_ip == Ip).Count();
                if (_count > 0)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Голосование
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public override VoteModel getVoteItem(Guid id, string Ip)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_votes
                            .Where(w => w.id == id)
                            .Select(s => new VoteModel
                            {
                                Id = s.id,
                                Header = s.c_header,
                                Text = s.c_text,
                                DateStart = s.d_date_start,
                                DateEnd = s.d_date_end,
                                Answer = getVoteAnswer(s.id, Ip),
                                ShowStatistic = ShowStatic(s.id, Ip)
                            });
                if (query.Any())
                    return query.Single();

                return null;
            }

        }

        /// <summary>
        /// Получаем ответы на голосование
        /// </summary>
        /// <param name="VoteId"></param>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public override VoteAnswer[] getVoteAnswer(Guid VoteId, string Ip)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_vote_answerss
                            .Where(w => w.f_vote == VoteId)
                            .OrderBy(o => o.n_sort)
                            .Select(s => new VoteAnswer()
                            {
                                Variant = s.c_variant,
                                id = s.id,
                                Statistic = getVoteStat(s.id, VoteId, Ip)
                            });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Статистика по голосованию
        /// </summary>
        /// <param name="AnswerId"></param>
        /// <param name="VoteId"></param>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public override VoteStat getVoteStat(Guid AnswerId, Guid VoteId, string Ip)
        {
            using (var db = new CMSdb(_context))
            {
                // проверяем даны ли ранее ответы этим пользователем
                VoteStat data = new VoteStat
                {
                    AllVoteCount = db.content_vote_userss.Where(w => w.f_vote == VoteId).Count(),
                    ThisVoteCount = db.content_vote_userss.Where(w => w.f_answer == AnswerId).Count()
                };

                return data;
            }
        }

        /// <summary>
        /// Записывает данные о факте голосования
        /// </summary>
        /// <param name="VoteId">Идентификатор вопроса</param>
        /// <param name="AnswerId">Индентифкатор ответа</param>
        /// <param name="Ip">IP адрес пользователя</param>
        /// <returns></returns>
        public override bool GiveVote(Guid VoteId, string[] AnswerId, string Ip)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    if (AnswerId.Length > 0)
                    {
                        foreach (var x in AnswerId)
                        {
                            Guid AnswerItemId = Guid.Parse(x);
                            db.content_vote_userss
                              .Value(v => v.c_ip, Ip)
                              .Value(v => v.f_vote, VoteId)
                              .Value(v => v.f_answer, AnswerItemId)
                              .Insert();
                        }
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Список админстративного персонала
        /// </summary>
        /// <returns></returns>
        public override OrgsAdministrative[] getAdministrative(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_sitess.Where(w => w.c_alias.ToLower() == domain);
                if (query.Any())
                {
                    var data = query.Single();
                    if (data.c_content_type == "org")
                    {
                        var adm = db.content_orgs_adminstrativs
                            .Where(w => w.f_org == data.f_content)
                            .OrderBy(o => o.n_sort)
                            .Select(s => new OrgsAdministrative()
                            {
                                id = s.id,
                                Surname = s.c_surname,
                                Name = s.c_name,
                                Patronymic = s.c_patronymic,
                                Phone = s.c_phone,
                                Photo = new Photo { Url = s.c_photo },
                                Post = s.c_post,
                                Text = s.c_text,
                                PeopleId = s.f_people
                            });

                        if (adm.Any())
                            return adm.ToArray();
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Получаем OID организации по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override string getOid()
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    return
                        (from s in db.cms_sitess
                         join o in db.content_orgss on s.f_content equals o.id
                         where s.c_alias.ToLower().Equals(_domain)
                         select o.f_oid
                        ).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> getOid for \"{0}\"", _domain);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }

            return null;
        }

        /// <summary>
        /// Получаем список типов ЛПУ
        /// </summary>
        /// <returns></returns>
        public override OrgType[] getOrgTypes()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_orgs_typess
                    .Where(w => w.n_sort != 0 && w.n_sort != 1000)
                    .OrderBy(o => o.n_sort)
                    .Select(s => new OrgType
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort
                    });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получаем список ЛПУ по типу
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override OrgsModel[] getOrgModels(Guid? type)
        {
            using (var db = new CMSdb(_context))
            {
                if (type != null)
                {
                    var query = (from o in db.content_orgss
                                 join t in db.content_orgs_types_links on o.id equals t.f_org
                                 join a in db.content_orgs_adminstrativs on o.id equals a.f_org into ps
                                 from a in ps.DefaultIfEmpty()
                                 where a.id == null || a.f_org.Equals(o.id) && a.b_leader
                                 join s in db.cms_sitess on o.id equals s.f_content into ss
                                 from s in ss.DefaultIfEmpty()
                                 where (s.id == null || s.f_content.Equals(o.id))//s.c_alias!="main" &&
                                 join p in db.content_org_employeess on a.f_people equals p.id into ts
                                 from p in ts.DefaultIfEmpty()
                                 where p.id == null || p.id == a.f_people
                                 where t.f_type.Equals(type) /*&& o.f_oid != null*/
                                 select new OrgsModel
                                 {
                                     Id = o.id,
                                     Title = o.c_title,
                                     Phone = o.c_phone,
                                     PhoneReception = o.c_phone_reception,
                                     Fax = o.c_fax,
                                     Email = o.c_email,
                                     ExtUrl = o.c_www,
                                     Address = o.c_adress,
                                     Logo = new Photo() { Url = o.c_logo },
                                     Link = (!string.IsNullOrEmpty(s.c_alias)) ? getSiteDefaultDomain(s.c_alias) : null,
                                     Leader = new OrgsAdministrative
                                     {
                                         id = p.f_people,
                                         Surname = a.c_surname,
                                         Name = a.c_name,
                                         Patronymic = a.c_patronymic,
                                         Post = a.c_post,
                                         PeopleId = a.f_people,
                                         Photo = new Photo { Url = a.c_photo }
                                     }
                                 });

                    if (query.Any())
                        return query.ToArray();

                    return null;
                }
                else
                {
                    var query = (from o in db.content_orgss
                                 join a in db.content_orgs_adminstrativs on o.id equals a.f_org into ps
                                 from a in ps.DefaultIfEmpty()
                                 where a.id == null || a.f_org.Equals(o.id) && a.b_leader
                                 join s in db.cms_sitess on o.id equals s.f_content into ss
                                 from s in ss.DefaultIfEmpty()
                                 join p in db.content_org_employeess on a.f_people equals p.id into ts
                                 from p in ts.DefaultIfEmpty()
                                 where p.id == null || p.id == a.f_people
                                 orderby o.n_sort
                                 where (s.id == null || s.f_content.Equals(o.id)) //&& s.c_alias != "main"// && o.f_oid != null
                                 select new OrgsModel
                                 {
                                     Id = o.id,
                                     Title = o.c_title,
                                     Phone = o.c_phone,
                                     PhoneReception = o.c_phone_reception,
                                     Fax = o.c_fax,
                                     Email = o.c_email,
                                     ExtUrl = o.c_www,
                                     Address = o.c_adress,
                                     Logo = new Photo() { Url = o.c_logo },
                                     Link = (!string.IsNullOrEmpty(s.c_alias)) ? getSiteDefaultDomain(s.c_alias) : null,
                                     Affiliation = o.f_department_affiliation,
                                     Leader = new OrgsAdministrative
                                     {
                                         id = p.f_people,
                                         Surname = a.c_surname,
                                         Name = a.c_name,
                                         Patronymic = a.c_patronymic,
                                         Post = a.c_post,
                                         PeopleId = a.f_people,
                                         Photo = new Photo { Url = a.c_photo }
                                     }
                                 });

                    if (query.Any())
                        return query.ToArray();

                    return null;
                }
            }
        }


        public override OrgsModel[] getOrgsModel(string tab, Guid? idtype = null)
        {
            using (var db = new CMSdb(_context))
            {
                //var query =db.content_orgss.Where(w=>w.b_disabled==false && w.id!=Guid.Parse("CC8442EF-CECE-4CD3-A544-DA319E03C981"));
                var query = db.get_lpu_list();

                switch (tab)
                {
                    case "typelist":
                        if (idtype.HasValue)
                        {
                            query = query
                              .Join(db.content_orgs_types_links
                                  .Where(w => w.f_type == idtype.Value),
                              n => n.id,
                              m => m.f_org,
                              (n, m) => n);
                        }
                        break;
                    case "affiliation":
                        if (idtype.HasValue)
                        {
                            query = query
                                .Where(w => w.f_department_affiliation == idtype);
                        }
                        break;
                    case "services":
                        if (idtype.HasValue)
                        {
                            query = query
                            .Join(db.content_orgs_medical_services_linkss
                                 .Where(w => w.f_medical_service == idtype),
                            n => n.id,
                            m => m.f_org,
                            (n, m) => n);
                        }
                        break;
                }

                //query = query.OrderBy(o => new { o.n_sort, o.c_title });
                var data = query.Select(o => new OrgsModel()
                {
                    //Id = o.id,
                    Title = o.c_title,
                    Phone = o.orgphone,
                    PhoneReception = o.c_phone_reception,
                    Fax = o.c_fax,
                    Email = o.c_email,
                    ExtUrl = o.c_www,
                    Address = o.c_adress,
                    Logo = new Photo() { Url = o.c_logo },
                    Link = o.c_domain,
                    Leader = new OrgsAdministrative
                    {
                        Surname = o.c_surname,
                        Name = o.c_name,
                        Patronymic = o.c_patronymic,
                        Post = o.c_post,
                        PeopleId = o.leader_id,
                        Photo = new Photo { Url = o.c_photo }
                    }
                    //spotDomainContent(o.id),
                    //Leader = getLeaderOrg(o.id)
                });
                if (data.Any()) return data.ToArray();
                else return null;
            }
        }

        public override OrgsAdministrative getLeaderOrg(Guid OrgId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_orgs_adminstrativs.Where(w => w.f_org == OrgId && w.b_leader == true);
                if (query.Any())
                {
                    return query.Select(s => new OrgsAdministrative
                    {
                        //id = s.f_people,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Post = s.c_post,
                        PeopleId = s.f_people,
                        Photo = new Photo { Url = s.c_photo }
                    }).Single();
                }
                return null;
            }
        }

        public override string spotDomainContent(Guid? ContentId)
        {
            using (var db = new CMSdb(_context))
            {
                var site = db.cms_sitess.Where(w => w.f_content == ContentId);
                if (site.Any())
                {
                    string SiteId = site.Single().c_alias.ToLower();
                    var domains = db.cms_sites_domainss.Where(w => w.f_site == SiteId).OrderByDescending(o => o.b_default);
                    if (domains.Any())
                    {
                        return domains.First().c_domain;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Получаем название типа ЛПУ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override string getOrgTypeName(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_orgs_typess
                    .Where(w => w.id.Equals(id))
                    .Select(s => s.c_title);
                if (data.Any())
                    return data.SingleOrDefault();

                return null;
            }
        }

        /// <summary>
        /// Получим список ведомственных принадлежностей
        /// </summary>
        /// <returns></returns>
        public override DepartmentAffiliationModel[] getDepartmentAffiliations()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_orgs_department_affiliations
                    .OrderBy(o => o.n_sort)
                    .Select(s => new DepartmentAffiliationModel
                    {
                        Key = s.id,
                        Value = s.c_title
                    });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получим название ведомственной принадлежности
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override string getAffiliationDepartment(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_orgs_department_affiliations
                    .Where(w => w.id.Equals(id))
                    .Select(s => s.c_title);

                if (query.Any())
                    return query.SingleOrDefault();

                return null;
            }
        }


        /// <summary>
        /// Получим список медицинских услуг для организации
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override MedServiceModel[] getMedicalServices(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrWhiteSpace(domain))
                {
                    var query = (from s in db.cms_sitess
                                 join o in db.content_orgss on s.f_content equals o.id
                                 join omsl in db.content_orgs_medical_services_linkss on o.id equals omsl.f_org
                                 join ms in db.content_medical_servicess on omsl.f_medical_service equals ms.id
                                 where s.c_alias.ToLower().Equals(domain)
                                 orderby ms.n_sort
                                 select new MedServiceModel
                                 {
                                     Id = ms.id,
                                     Title = ms.c_title,
                                     Sort = ms.n_sort
                                 });

                    if (query.Any())
                        return query.ToArray();

                    return null;
                }
                else
                {
                    var query = (from ms in db.content_medical_servicess
                                 orderby ms.n_sort
                                 select new MedServiceModel
                                 {
                                     Id = ms.id,
                                     Title = ms.c_title,
                                     Sort = ms.n_sort
                                 });

                    if (query.Any())
                        return query.ToArray();

                    return null;
                }
            }
        }



        public MedServiceModel[] getMedicalServicesOptim(Guid? IdOrg)
        {
            using (var db = new CMSdb(_context))
            {
                if (IdOrg!=null)
                {
                    //показываем все услуги
                    var query = db.content_sv_org_servicess
                               .Where(w=>w.f_org==IdOrg)
                               .OrderBy(o=>o.n_sort)
                               .Select(s=> new MedServiceModel {
                                   Id=s.id,
                                   Title=s.c_title,
                                   Sort=s.n_sort
                               });   
                    if (query.Any())
                        return query.ToArray();

                    return null;
                }
                else
                {
                    //показываем все услуги
                    var query = (from ms in db.content_medical_servicess
                                 orderby ms.n_sort
                                 select new MedServiceModel
                                 {
                                     Id = ms.id,
                                     Title = ms.c_title
                                     //,
                                     //Sort = ms.n_sort
                                 });

                    if (query.Any())
                        return query.ToArray();

                    return null;
                }
            }
        }

        /// <summary>
        /// Получим список организаций, представляющих медицинскую услугу
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override OrgsModel[] getOrgPortalModels(Guid service)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from o in db.content_orgss
                             join omsl in db.content_orgs_medical_services_linkss on o.id equals omsl.f_org
                             join a in db.content_orgs_adminstrativs on o.id equals a.f_org into ps
                             from a in ps.DefaultIfEmpty()
                             where a.id == null || (a.f_org.Equals(o.id) && a.b_leader)
                             join s in db.cms_sitess on o.id equals s.f_content into ss
                             from s in ss.DefaultIfEmpty()
                             join p in db.content_org_employeess on a.f_people equals p.id into ts
                             from p in ts.DefaultIfEmpty()
                             where p.id == null || p.id == a.f_people
                             where omsl.f_medical_service.Equals(service)
                             select new OrgsModel
                             {
                                 Id = o.id,
                                 Title = o.c_title,
                                 Phone = o.c_phone,
                                 PhoneReception = o.c_phone_reception,
                                 Fax = o.c_fax,
                                 Email = o.c_email,
                                 Address = o.c_adress,
                                 Logo = new Photo() { Url = o.c_logo },
                                 Link = (from d in db.cms_sites_domainss orderby d.b_default descending where d.f_site==s.c_alias select d.c_domain).FirstOrDefault(),
                                 //s.c_alias.ToLower(),
                                 Leader = new OrgsAdministrative
                                 {
                                     id = p.f_people,
                                     Surname = a.c_surname,
                                     Name = a.c_name,
                                     Patronymic = a.c_patronymic,
                                     Post = a.c_post,
                                     PeopleId = a.f_people,
                                     Photo = new Photo { Url = a.c_photo }
                                 }
                             });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получим название медицинской услуги
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override string getMedicalServiceTitle(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_medical_servicess
                    .Where(w => w.id.Equals(id))
                    .Select(s => s.c_title);

                if (query.Any())
                    return query.SingleOrDefault();

                return null;
            }
        }

        /// <summary>
        /// Поиск людей по фильтру
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<content_people> FindPeoplesQuery(IQueryable<content_people> query, PeopleFilter filter)
        {
            if (!String.IsNullOrWhiteSpace(filter.SearchText))
            {
                var search = filter.SearchText.ToLower().Split(' ');

                foreach (var s in search)
                {
                    query = query.Where(w => w.c_surname.Contains(s)
                                        || w.c_name.Contains(s)
                                        || w.c_patronymic.Contains(s));
                }
            }
            if (!String.IsNullOrEmpty(filter.Type))
            {
                query = query.Where(w => w.employeespostspeoples.Any(a => a.f_post.Equals(Int32.Parse(filter.Type))));
            }
            if (!String.IsNullOrWhiteSpace(filter.Group))
            {
                query = query.Where(w => w.contentpeopleorglinks.Any(a => a.fkcontentdepartmentpeoplelinks.Any(b => b.f_department.Equals(Guid.Parse(filter.Group)))));
            }
            if (filter.Specializations != null && filter.Specializations.Count() > 0)
            {
                query = query.Where(w => w.employeespostspeoples.Any(a => filter.Specializations.Contains(a.f_post)));
            }

            return query;
        }

        /// <summary>
        /// Получаем список врачей для портала
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <returns>Список врачей</returns>
        public override PeopleList getDoctorsList(PeopleFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                #region comments
                //var people = db.content_peoples
                //                    .Where(w => w.contentpeopleorglinks.Any(a => !a.b_dismissed))
                //                    .Where(w => w.employeespostspeoples.Any(b => b.employeespostsspecializations.b_doctor));

                //var queryData = FindPeoplesQuery(people, filter);

                //var result = queryData
                //    .OrderBy(o => o.c_surname)
                //    .Select(s => new PeopleModel
                //    {
                //        Id = s.id,
                //        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                //        Photo = s.c_photo,
                //        Posts = s.employeespostspeoples
                //                    .Where(a => !a.b_dissmissed)
                //                    .Select(p => new Specialisation
                //                    {
                //                        Id = p.f_post,
                //                        Name = p.employeespostsspecializations.c_name,
                //                        Type = p.n_type,
                //                        Org = new OrgsShortModel
                //                        {
                //                            Id = p.contentemployeespostsorgs.id,
                //                            Title = p.contentemployeespostsorgs.c_title,
                //                            Url = db.cms_sitess.Where(w => w.f_content.Equals(p.contentemployeespostsorgs.id))
                //                                                .Select(r => r.fksitesdomainss.FirstOrDefault().c_domain)
                //                                                .SingleOrDefault()
                //                        }
                //                    })
                //    });

                //int itemCount = result.Count();

                //result = result
                //            .Skip(filter.Size * (filter.Page - 1))
                //            .Take(filter.Size);

                //if (result.Any())
                //{
                //    return new PeopleList()
                //    {
                //        Doctors = result.ToArray(),
                //        Pager = new Pager()
                //        {
                //            Page = filter.Page,
                //            Size = filter.Size,
                //            ItemsCount = itemCount
                //        }
                //    };
                //}
                #endregion

                var people = db.content_peoples
                    .Where(w => w.testpeoplepostlinkstestpeopless.Any(a => !a.b_deleted))
                    .Where(w => w.testpeoplepostlinkstestpeopless.Any(a => a.testpeoplepostlinkstestposts.b_doctor));

                var queryData = FindPeoplesQuery(people, filter);

                var result = queryData
                    .OrderBy(o => o.c_surname)
                    .Select(s => new PeopleModel
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                        Photo = s.c_photo,
                        Posts = s.testpeoplepostlinkstestpeopless
                            .Where(w => !w.b_deleted)
                            .Select(p => new Specialisation
                            {
                                Id = p.f_post,
                                Name = p.testpeoplepostlinkstestposts.name,
                                Type = p.n_type,
                                Org = new OrgsShortModel
                                {
                                    Id = p.f_org,
                                    Title = p.testpeoplepostlinkstestorgs.c_name,
                                    Url = db.cms_sitess
                                        .Where(w => w.f_content == p.f_org)
                                        .Select(r => r.fksitesdomainss.FirstOrDefault().c_domain)
                                        .SingleOrDefault()
                                }
                            })
                    });

                int itemCount = result.Count();

                result = result
                            .Skip(filter.Size * (filter.Page - 1))
                            .Take(filter.Size);

                if (result.Any())
                {
                    return new PeopleList()
                    {
                        Doctors = result.ToArray(),
                        Pager = new Pager()
                        {
                            Page = filter.Page,
                            Size = filter.Size,
                            ItemsCount = itemCount
                        }
                    };
                }

                return null;
            }
        }

        /// <summary>
        /// Получаем список отзывов
        /// </summary>
        /// <param name="filtr"></param>
        /// <returns></returns>
        public override FeedbacksList getFeedbacksList(FilterParams filtr)
        {
            FeedbackModel[] feedbacks = null;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_feedbackss
                    .Where(w => w.f_site == _domain)
                    .Where(w => w.b_disabled == filtr.Disabled)
                    .Where(w => w.c_type == filtr.Type)
                    .OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Skip(filtr.Size * (filtr.Page - 1))
                        .Take(filtr.Size)
                        .Select(s => new FeedbackModel
                        {
                            Id = s.id,
                            Title = s.c_title,
                            Text = s.c_text,
                            Date = s.d_date,
                            SenderName = s.c_sender_name,
                            SenderEmail = s.c_sender_email,
                            //SenderContacts,
                            Answer = s.c_answer,
                            Answerer = s.c_answerer,
                            //AnswererCode
                            IsNew = s.b_new,
                            Disabled = s.b_disabled,
                            Anonymous = s.b_anonymous,
                            //FbType
                        });
                    feedbacks = List.ToArray();

                    return new FeedbacksList()
                    {
                        Data = feedbacks,
                        Pager = new Pager()
                        {
                            Page = filtr.Page,
                            Size = filtr.Size,
                            ItemsCount = ItemCount,
                            //PageCount = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                        }
                    };
                }
                return null;
            }
        }
        /// <summary>
        /// Отзыв
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override FeedbackModel getFeedbackItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_feedbackss
                    .Where(w => w.id == id)
                    .Select(s => new FeedbackModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        SenderName = s.c_sender_name,
                        SenderEmail = s.c_sender_email,
                        SenderContacts = s.c_sender_contacts,
                        Answer = s.c_answer,
                        Answerer = s.c_answerer,
                        IsNew = s.b_new,
                        Disabled = s.b_disabled,
                        AnswererCode = s.c_code,
                        Anonymous = s.b_anonymous,
                        FbType = (FeedbackType)Enum.Parse(typeof(FeedbackType), s.c_type)
                    });

                if (data.Any())
                    return data.First();

                return null;
            }
        }
        /// <summary>
        /// Сохранение сообщения  из обратной связи при отправке пользователем
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public override bool insertFeedbackItem(FeedbackModel feedback)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
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
                            c_sender_contacts = feedback.SenderContacts,
                            c_answer = feedback.Answer,
                            c_answerer = feedback.Answerer,
                            b_new = feedback.IsNew,
                            b_disabled = feedback.Disabled,
                            f_site = _domain,
                            c_code = (Guid)feedback.AnswererCode,
                            b_anonymous = feedback.Anonymous,
                            c_type = feedback.FbType.ToString()
                        };
                        db.Insert(cdFeedback);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> insertFeedbackItem for \"{0}\"", _domain);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }

            return false;
        }

        /// <summary>
        /// Записываем ответ на сообщение
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public override bool updateFeedbackItem(FeedbackModel feedback)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_feedbacks cdFeedback = db.content_feedbackss
                                                    .Where(p => p.id == feedback.Id)
                                                    .SingleOrDefault();
                        if (cdFeedback == null)
                        {
                            throw new Exception("Запись с таким Id не существует");
                        }

                        cdFeedback.c_answer = feedback.Answer;
                        cdFeedback.c_answerer = feedback.Answerer;
                        cdFeedback.b_disabled = feedback.Disabled;
                        cdFeedback.c_code = (Guid)feedback.AnswererCode;

                        db.Update(cdFeedback);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = String.Format("frontRepository=> updateFeedbackItem for \"{0}\"", _domain);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Debug, message, ex));
            }
            return false;
        }

        /// <summary>
        /// Анкета
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override AnketaModel getLastWorksheetItem()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_anketass.AsQueryable();

                query = query.Where(s => s.b_disabled == false)
                    .Where(s => s.d_date <= DateTime.Now);

                // Сделать привязку
                var data = query
                    .OrderByDescending(s => s.d_date)
                    .Select(s => new AnketaModel
                    {
                        Id = s.id,
                        Count = s.n_count,
                        Title = s.c_title,
                        Alias = s.c_alias.ToLower(),
                        Text = s.c_text,
                        Url = s.c_url,
                        DateBegin = s.d_date,
                        DateEnd = s.d_date_end,
                        Disabled = s.b_disabled,
                        //Links  заполняем в контроллере
                    });
                if (data.Any())
                    return data.First();

                else
                    return null;
            }
        }

        /// <summary>
        /// Получаем список событий
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override EventsList getEvents(FilterParams filter)
        {
            string domain = _domain;

            using (var db = new CMSdb(_context))
            {
                var contentType = ContentType.EVENT.ToString().ToLower();

                var eventFilter = (from s in db.cms_sitess
                                   join cct in db.content_content_links on s.f_content equals cct.f_link
                                   join e in db.content_eventss on cct.f_content equals e.id
                                   where s.c_alias.ToLower().Equals(domain) && cct.f_content_type.Equals(contentType)
                                   select e.id);

                if (!eventFilter.Any())
                    return null;
                var eventIds = eventFilter.ToArray();

                var query = db.content_eventss
                    .Where(w => eventIds.Contains(w.id))
                    .Where(w => !w.b_disabled);

                if (filter.Date != null)
                {
                    query = query.Where(w => (w.d_date >= filter.Date) || (w.d_date_end >= filter.Date)
                             || (w.b_annually && (w.d_date.DayOfYear >= ((DateTime)filter.Date).DayOfYear || w.d_date_end.DayOfYear >= ((DateTime)filter.Date).DayOfYear)));
                }
                if (filter.DateEnd != null)
                {
                    query = query.Where(w => (w.d_date <= filter.DateEnd) || (w.d_date_end <= filter.DateEnd)
                             || (w.b_annually && (w.d_date.DayOfYear <= ((DateTime)filter.DateEnd).DayOfYear || w.d_date_end.DayOfYear <= ((DateTime)filter.DateEnd).DayOfYear)));
                }


                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(w => w.c_title.ToLower().Contains(filter.SearchText.ToLower()));
                }

                query = query.OrderByDescending(o => o.d_date)
                             .ThenByDescending(o => o.d_date_end);

                int itemCount = query.Count();

                var eventList = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new EventsModel
                    {
                        Id = s.id,
                        Num = s.num,
                        Title = s.c_title,
                        Alias = s.c_alias.ToLower(),
                        DateBegin = s.d_date,
                        DateEnd = s.d_date_end,
                        PreviewImage = new Photo { Url = s.c_preview },
                        Place = s.c_place,
                        EventMaker = s.c_organizer,
                        Url = s.c_url,
                        UrlName = s.c_url_name
                    });

                if (eventList.Any())
                {
                    return new EventsList()
                    {
                        Data = eventList.ToArray(),
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

            return null;
        }

        /// <summary>
        /// Получаем единичную запись события
        /// </summary>
        /// <param name="num"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override EventsModel getEvent(int num, string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_eventss
                    .Where(w => w.num.Equals(num))
                    .Where(w => w.c_alias.ToLower().Equals(alias.ToLower()))
                    .Select(s => new EventsModel
                    {
                        Id = s.id,
                        Num = s.num,
                        Title = s.c_title,
                        Text = s.c_text,
                        Alias = s.c_alias.ToLower(),
                        DateBegin = s.d_date,
                        DateEnd = s.d_date_end,
                        PreviewImage = new Photo { Url = s.c_preview },
                        Place = s.c_place,
                        EventMaker = s.c_organizer,
                        Url = s.c_url,
                        UrlName = s.c_url_name
                    });

                if (!query.Any()) return null;
                return query.SingleOrDefault();
            }
        }

        /// <summary>
        /// Получаем список фотоматериалов
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override PhotoModel[] getPhotoList(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_photoss.Where(w => w.f_album == id);
                if (query.Any())
                {
                    var data = query.OrderBy(o => o.n_sort)
                                  .Select(s => new PhotoModel()
                                  {
                                      PreviewImage = new Photo { Url = s.c_preview },
                                      PhotoImage = new Photo { Url = s.c_photo },
                                      PhotoOriginal = s.c_photo,
                                      Id = s.id,
                                      Title = s.c_title
                                  }).ToArray();
                    return data;
                }
                return null;
            }
        }

        /// <summary>
        /// Получаем список главных специалистов
        /// </summary>
        /// <returns></returns>
        public override GSModel[] getGSList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from ms in db.content_gss
                             join s in db.cms_sitess on ms.id equals s.f_content into s2
                             from s in s2.DefaultIfEmpty()
                             select new GSModel
                             {
                                 Id = ms.id,
                                 Title = ms.c_name,
                                 Desc = ms.c_desc,
                                 Domain = (s != null) ? s.c_alias.ToLower() : null,
                                 Url = (s != null && !string.IsNullOrEmpty(s.c_alias)) ? getSiteDefaultDomain(s.c_alias) : null
                             }
                    );

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query
                        .Where(w => w.Title.ToLower().Contains(filter.SearchText.ToLower()));
                }

                query = query.OrderBy(p => p.Title);

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Список врачей в гс по типу
        /// </summary>
        /// <param name="mainSpecialistId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override GSMemberModel[] getGSMembers(Guid mainSpecialistId, GSMemberType type)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_gs_memberss
                            .Where(p => p.f_gs == mainSpecialistId)
                            .Where(p => p.f_type == type.ToString().ToLower())
                            .Select(p => new GSMemberModel()
                            {
                                Id = p.id,
                                GSId = p.f_gs,
                                People = new PeopleModel()
                                {
                                    Id = p.f_people,
                                    FIO = String.Format("{0} {1} {2}", p.gsmemberspeople.c_surname, p.gsmemberspeople.c_name, p.gsmemberspeople.c_patronymic),
                                    Photo = p.gsmemberspeople.c_photo,
                                },
                                Posts = getPeopleSpecialisations(p.f_people)
                                //Orgs = getOrgs(new OrgFilter() { PeopleId = p.f_people})
                            });
                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получение контактов для члена гс
        /// </summary>
        /// <param name="id">GSMember Id</param>
        /// <returns></returns>
        public override OrgsModel[] getGsMemberContacts(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data1 = db.content_gs_members_contactss
                                   .Where(p => p.f_gs_member == id)
                                   .Where(p => p.f_org == null)
                                   .Select(s => new OrgsModel()
                                   {
                                       Title = s.c_org_title,
                                       Address = s.c_org_address,
                                       Phone = s.c_org_phone,
                                       Fax = s.c_org_fax,
                                       ExtUrl = s.c_org_site
                                   });

                var query = db.content_gs_members_contactss
                                   .Where(p => p.f_gs_member == id)
                                   .Where(p => p.f_org != null)
                                   .Select(p => p.f_org);

                if (query.Any())
                {
                    var data2 = db.content_orgss
                        .Where(s => query.Contains(s.id))
                        .Select(s => new OrgsModel()
                        {
                            Id = s.id,
                            Title = s.c_title,
                            ShortTitle = s.c_title_short,
                            Sort = s.n_sort,
                            Address = s.c_adress,
                            Phone = s.c_phone,
                            Fax = s.c_fax,
                            Url = getSiteDefaultDomainByContentId(s.id),
                            ExtUrl = s.c_www,
                            Logo = new Photo()
                            {
                                Url = s.c_logo
                            }
                        });

                    if (data1.Any() && data2.Any())
                    {
                        var dataArr1 = data1.ToArray();
                        var dataArr2 = data2.ToArray();
                        return dataArr1.Union(dataArr2).ToArray();
                    }

                    if (data2.Any())
                        return data2.ToArray();
                }

                if (data1.Any())
                    return data1.ToArray();

                return null;
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
                    .Select(s => new GSModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Desc = s.c_desc,
                        //Во внешней части не нужно
                        //SiteId = db.cms_sitess.Where(p => p.f_content == s.id).Select(p => p.id).SingleOrDefault(),
                        //(from site in db.cms_sitess
                        //  join cms in db.content_gss on site.f_content equals cms.id
                        //  where cms.id.Equals(s.id)
                        //  select site.id).SingleOrDefault(),
                        Specialisations = s.gsspecialisationsgss.Where(sp => sp.f_gs == id).Any() ?
                                                s.gsspecialisationsgss
                                                        .Select(sp => sp.f_specialisation)
                                                        .ToArray()
                                                : null,
                        //(from l in db.content_gs_specialisationss
                        //join m in db.content_gss
                        //on l.f_gs equals m.id
                        //where l.f_gs.Equals(s.id)
                        //select l.f_specialisation).ToArray(),

                    });

                if (!query.Any()) return null;
                else return query.SingleOrDefault();
            }
        }

        /// <summary>
        /// Список врачей, входящих в Главных специалистов 
        /// </summary>
        /// <param name="filtr"></param>
        /// <returns></returns>
        public override PeopleModel[] getGSMembers(PeopleFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var type = "spec";
                var people = (from s in db.content_gs_memberss

                              join spec in db.content_gss on s.f_gs equals spec.id

                              join site in db.cms_sitess on s.f_gs equals site.f_content into ss
                              from site in ss.DefaultIfEmpty()

                              join p in db.content_peoples on s.f_people equals p.id

                              join org in db.content_org_employeess on p.id equals org.f_people into ps
                              from org in ps.DefaultIfEmpty()

                              where (s.f_type == type)
                              orderby p.c_surname

                              select new { s, p, org, spec, site }
                              );

                if (filtr.Orgs != null && filtr.Orgs.Count() > 0)
                {
                    people = people.Where(w => filtr.Orgs.Contains(w.org.f_org));
                }

                var data = people.Select(s =>
                    new PeopleModel()
                    {
                        Id = s.p.id,
                        FIO = s.p.c_surname + " " + s.p.c_name + " " + s.p.c_patronymic,
                        GS = new GSModel()
                        {
                            Id = s.spec.id,
                            Title = s.spec.c_name,
                            Url = (!string.IsNullOrEmpty(s.site.c_alias)) ? getSiteDefaultDomain(s.site.c_alias) : null
                        },
                        Photo = s.p.c_photo
                    });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Возвращает кол-во специалистов
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override int getCountGSBySite(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                return (from ms in db.content_gss
                        join s in db.cms_sitess on ms.id equals s.f_content
                        join l in db.content_gs_memberss on ms.id equals l.f_gs
                        where l.f_type.Equals("spec") && s.c_alias.Equals(domain)
                        select ms.c_name).Count();
            }
        }

        /// <summary>
        /// Получаем организацию
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override OrgsShortModel getOrgItemShort(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_orgss.Where(w => w.id == id)
                    .Select(s => new OrgsShortModel
                    {
                        Id = s.id,
                        Title = !string.IsNullOrEmpty(s.c_title_short) ? s.c_title_short : s.c_title,
                        Url = getSiteDefaultDomainByContentId(s.id),
                        Logo = new Photo
                        {
                            Url = s.c_logo
                        }
                    });

                if (!data.Any()) return null;
                else return data.FirstOrDefault();
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
                var data = db.content_orgss.Where(w => w.id == id)
                    .Select(s => new OrgsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        ShortTitle = s.c_title_short,
                        Phone = s.c_phone,
                        Url = getSiteDefaultDomainByContentId(s.id),
                        PhoneReception = s.c_phone_reception,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        ExtUrl = s.c_www,
                        Contacts = s.c_contacts,
                        Address = s.c_adress,
                        GeopointX = s.n_geopoint_x,
                        GeopointY = s.n_geopoint_y,
                        Oid = s.f_oid,
                        Logo = new Photo
                        {
                            Url = s.c_logo
                        }
                    });

                if (!data.Any()) return null;
                else return data.FirstOrDefault();
            }
        }

        /// <summary>
        /// Возвращает идентификатор импорта по организации
        /// </summary>
        /// <returns></returns>
        public override OrgsShortModel getCurrentOrgImportGuid()
        {
            using (var db = new CMSdb(_context))
            {
                var result = (from o in db.content_orgss
                              join s in db.cms_sitess on o.id equals s.f_content
                              where s.c_alias.Equals(_domain)
                              select new OrgsShortModel
                              {
                                  Id = (Guid)o.f_guid,
                                  Title = o.c_title
                              });

                if (result.Any())
                    return result.SingleOrDefault();
                return null;
            }
        }

        /// <summary>
        /// Возвращает организацию по названию
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public override OrgsModel getOrgItem()
        {
            string domain = _domain;

            using (var db = new CMSdb(_context))
            {
                return (from o in db.content_orgss
                        join s in db.cms_sitess on o.id equals s.f_content
                        where s.c_alias.Equals(domain)
                        select new OrgsModel
                        {
                            Id = o.id,
                            Title = o.c_title
                        }).SingleOrDefault();
            }
        }


        public override VacanciesList getVacancy(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_vacanciess
                     .Where(w => w.b_disabled == false);

                if (!string.IsNullOrEmpty(filter.Domain))
                {
                    query = query.Where(w => w.f_site == filter.Domain);
                }

                if (filter.Date != null)
                {
                    query = query.Where(w => w.d_date >= filter.Date);
                }

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(w =>
                                (
                                w.c_profession.Contains(filter.SearchText) ||
                                w.c_post.Contains(filter.SearchText) ||
                                w.c_desc.Contains(filter.SearchText)
                                ));
                }

                query = query.OrderByDescending(o => o.d_date);

                var vacancyList = query
                            .Skip(filter.Size * (filter.Page - 1))
                            .Take(filter.Size)
                            .Select(s => new VacancyModel
                            {
                                Id = s.id,
                                Profession = s.c_profession,
                                Post = s.c_post,
                                Desc = s.c_desc,
                                Date = s.d_date,
                                Salary = s.c_salary,
                                OrgName = getOrgInfo(s.f_site).Title,
                                OrgUrl = getSiteDefaultDomain(s.f_site),
                                Temporarily = s.b_temporarily
                            });

                if (vacancyList.Any())
                {
                    int itemCount = query.Count();
                    return new VacanciesList()
                    {
                        Data = vacancyList.ToArray(),
                        Pager = new Pager()
                        {
                            Page = filter.Page,
                            Size = filter.Size,
                            ItemsCount = itemCount,
                            //PageCount = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                        }
                    };
                }
                return null;

            }
        }

        public override VacancyModel getVacancyItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_vacanciess.Where(w => (w.id == id && w.b_disabled == false));
                if (query.Any())
                {
                    var data = query.Select(s => new VacancyModel()
                    {
                        Id = s.id,
                        Profession = s.c_profession,
                        Post = s.c_post,
                        Desc = s.c_desc,
                        Experience = s.с_experience,
                        Сonditions = s.с_conditions,
                        Temporarily = s.b_temporarily,
                        Date = s.d_date,
                        Salary = s.c_salary,
                        OrgName = getOrgInfo(s.f_site).Title,
                        OrgUrl = getSiteDefaultDomain(s.f_site)
                    }).Single();
                    return data;
                }
                return null;

            }
        }
    }
}
