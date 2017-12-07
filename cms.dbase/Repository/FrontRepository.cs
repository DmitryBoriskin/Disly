using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using cms.dbModel.entity.cms;

namespace cms.dbase
{
    public class FrontRepository : abstract_FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        private string _domain = string.Empty;

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
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

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
                    throw new Exception("FrontRepository: getSideId Domain is empty!");

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

                    throw new Exception("FrontRepository: getSideId Domain '" + domain + "' was not found!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("FrontRepository: getSideId Domain '" + domain + "' непредвиденная ошибка!" + ex.Message);
            }
        }

        /// <summary>
        /// Получение вьюхи
        /// </summary>
        /// <param name="siteSection">Секция</param>
        /// <returns></returns>
        public override string getView(string siteSection) //string siteId,
        {
            string siteId = _domain;
            using (var db = new CMSdb(_context))
            {
                string ViewPath = "~/Error/404/";
                
                var query = (from s in db.front_site_sections
                             join v in db.front_page_viewss
                             on s.f_page_view equals v.id
                             where (s.f_site.Equals(siteId) && s.f_front_section.Equals(siteSection))
                             select v.c_url);
                if (query.Any())
                    ViewPath = query.SingleOrDefault();
                
                return ViewPath;
            }
        }

        /// <summary>
        /// Получение информации по сайту
        /// </summary>
        /// <returns></returns>
        public override SitesModel getSiteInfo()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess
                    .Where(w => w.c_alias.Equals(domain))
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
                        Twitter = s.c_twitter,
                        Theme = s.c_theme,
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
        /// Получение информации по сайту
        /// </summary>
        /// <returns></returns>
        public override UsersModel[] getSiteAdmins()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_userss.Where(w => w.f_group == "admin");
                if (query.Any())
                {
                    var data = query.Select(s => new UsersModel
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                        EMail = s.c_email
                    }).ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Получим список элементов карты сайта для контроллера
        /// </summary>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapListShort()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => !w.b_disabled)
                    .Select(s => new SiteMapModel
                    {
                        Title = s.c_title,
                        Path = s.c_path,
                        Alias = s.c_alias
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
        public override SiteMapModel[] getSiteMapList() //string domain
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_sitemap_menus
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => !w.b_disabled)
                    .OrderBy(o => o.n_sort)
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
                        MenuAlias = s.menu_alias,
                        MenuSort = s.menu_sort,
                        MenuGroups = getSiteMapGroupMenu(s.id),
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
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_banners_sections
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => !w.b_disabled)
                    .OrderBy(o => o.n_sort)
                    .Select(s => new BannersModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Photo = new Photo { Url = s.c_photo },
                        Url = s.c_url,
                        Text = s.c_text,
                        Date = s.d_date,
                        Sort = s.n_sort,
                        SectionAlias = s.c_alias
                    });

                if (data.Any())
                    return data.ToArray();

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
                var query = db.content_sitemaps.Where(w => w.c_path == path && w.c_alias == alias && w.f_site == domain);
                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias,
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
        /// Получим эл-т карты сайта
        /// </summary>
        /// <param name="frontSection"></param>
        /// <returns></returns>
        public override SiteMapModel getSiteMap(string frontSection)
        {
            string domain = _domain;

            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => w.f_front_section.Equals(frontSection))
                    .Select(s => new SiteMapModel
                    {
                        Title = s.c_title,
                        Text = s.c_text,
                        Alias = s.c_alias,
                        Path = s.c_path,
                        Id = s.id,
                        FrontSection = s.f_front_section
                    });

                if (!query.Any()) return null;
                return query.SingleOrDefault();
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
                    .Select(s => s.c_alias);

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
                    .Where(w => w.f_site.Equals(_domain))
                    .Where(w => w.c_path.Equals(path))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Path = s.c_path,
                        FrontSection = s.f_front_section,
                        Url = s.c_url
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
                                 .Where(w => w.uui_parent.Equals(ParentId))
                                 .OrderBy(o => o.n_sort)
                                 .Select(c => new SiteMapModel
                                 {
                                     Title = c.c_title,
                                     Alias = c.c_alias,
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
                var data = db.content_documentss.Where(w => w.id_page == id)
                 .OrderBy(o => o.n_sort)
                 .Select(s => new DocumentsModel
                 {
                     id = s.id,
                     Title = s.c_title,
                     FilePath = s.c_file_path,
                     idPage = s.id_page
                 });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получаем хленые крошки
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

                    var item_data = db.content_sitemaps
                                    .Where(w => w.f_site == domain && w.c_path == _path && w.c_alias == _alias).Take(1)
                                    .Select(s => new Breadcrumbs
                                    {
                                        Title = s.c_title,
                                        Url = s.c_path + s.c_alias
                                    }).SingleOrDefault();

                    if (item_data != null)
                    {
                        data.Add(item_data);
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

                return null;
            }
        }

        /// <summary>
        /// Получаем новости для модуля на главной странице
        /// </summary>
        /// <returns></returns>
        public override List<MaterialFrontModule> getMaterialsModule()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var contentType = ContentType.MATERIAL.ToString().ToLower();

                // список id-новостей для данного сайта
                var materialIds = db.content_content_links.Where(e => e.f_content_type == contentType)
                    .Join(db.cms_sitess.Where(o => o.c_alias == domain),
                            e => e.f_link,
                            o => o.f_content,
                            (e, o) => e.f_content
                            );

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
                        .Where(w => w.group_id.Equals(g))
                        .Where(w => w.b_disabled == false)
                        .OrderByDescending(o => o.d_date)
                        .Select(s => new MaterialFrontModule
                        {
                            Title = s.c_title,
                            Alias = s.c_alias,
                            Date = s.d_date,
                            GroupName = s.group_title,
                            GroupAlias = s.group_alias,
                            Photo = s.c_preview
                        });

                    // берём последние 3 новости данной группы
                    if (query.Any())
                        list.AddRange(query.Take(2));
                }

                if (list.Any())
                    return list;

                return null;
            }
        }

        /// <summary>
        /// Получим список новостей для определенной сущности
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(FilterParams filter)
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
                        .Join(db.cms_sitess.Where(o => o.c_alias == filter.Domain),
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

                    if (!String.IsNullOrEmpty(filter.Category))
                    {
                        var category = db.content_materials_groupss.Where(w => w.c_alias == filter.Category).First().id;
                        query = query
                                    .Join(
                                            db.content_materials_groups_links
                                            .Where(o => o.f_group == category),
                                            e => e.id, o => o.f_material, (o, e) => o
                                         );
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
                                Alias = s.c_alias,
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
                        return new MaterialsList
                        {
                            Data = materialsList.ToArray(),
                            Pager = new Pager
                            {
                                page = filter.Page,
                                size = filter.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
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
                                                        .Join(db.cms_sitess.Where(o => o.c_alias == domain),
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
                var data = db.content_materials_groupss;
                if (data.Any())
                {
                    return data.OrderBy(o => o.n_sort)
                               .Select(s => new MaterialsGroup
                               {
                                   Alias = s.c_alias,
                                   Title = s.c_title
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
                            where site.c_alias.Equals(domain)
                            select new { str, dep };

                var data = query.ToArray()
                    .GroupBy(p => new { p.str.id })
                    .Select(s => new StructureModel
                    {
                        Id = s.Key.id,
                        Title = s.First().str.c_title,
                        Phone = s.First().str.c_phone,
                        PhoneReception = s.First().str.c_phone_reception,
                        Email = s.First().str.c_email,
                        Num = s.First().str.num,
                        GeopointX = s.First().str.n_geopoint_x,
                        GeopointY = s.First().str.n_geopoint_y,
                        Ovp = s.First().str.b_ovp,
                        Adress = s.First().str.c_adress,
                        Routes = s.First().str.c_routes,
                        Departments = s.Select(d => new Departments
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

        /// <summary>
        /// Отдельная структура
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public override StructureModel getStructureItem(int num) //string domain,
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.content_org_structures.Where(w => w.num == num)
                           .Join(db.cms_sitess.Where(o => o.c_alias == domain), o => o.f_ord, e => e.f_content, (e, o) => e)
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
                    return data.First();

                return null;
            }
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
                    var People = db.content_sv_people_departments.Where(w => w.f_department == Id).OrderBy(o=>new {o.c_surname,o.c_name,o.c_patronymic })
                                    .Select(s => new People()
                                    {
                                        Id = s.id,
                                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                                        Post = s.c_post,
                                        Status = s.c_status
                                    });

                    if (People.Any())
                    {
                        data.Boss = People.Where(w => w.Status == "boss").ToArray();
                        data.Sister = People.Where(w=>w.Status== "sister").ToArray();                        
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
                    var People = db.content_sv_people_departments.Where(w => w.f_department == Id)
                                    .Select(s => new People()
                                    {
                                        Id = s.id,
                                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                                        Post = s.c_post,
                                        Status = s.c_status
                                    });

                    if (People.Any())
                    {
                        data.Peoples = People.ToArray();
                    }

                    var Boss = db.content_peoples.Where(w => w.id == data.DirectorF)
                                                 .Select(s => new People()
                                                 {
                                                     Id = s.id,
                                                     FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                                                     Post = data.DirecorPost
                                                 });

                    if (Boss.Any())
                    {
                        data.Boss = Boss.ToArray();
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
        public override People[] getPeopleList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                string domain = filter.Domain; // домен

                Guid department = !string.IsNullOrWhiteSpace(filter.Group) // департамент
                    ? Guid.Parse(filter.Group) : Guid.Empty;

                var search = !string.IsNullOrWhiteSpace(filter.SearchText) 
                    ? filter.SearchText.ToLower().Split(' ') : null; // поиск по человеку


                var people = (from s in db.cms_sitess
                              join pol in db.content_people_org_links on s.f_content equals pol.f_org
                              join p in db.content_peoples on pol.f_people equals p.id
                              select new { p, pol, s });

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
                            join pepl in db.content_people_employee_posts_links on p.p.id equals pepl.f_people
                            join ep in db.content_employee_postss on pepl.f_post equals ep.id
                            join pdl in db.content_people_department_links on p.pol.id equals pdl.f_people into ps
                            from pdl in ps.DefaultIfEmpty()
                            where p.s.c_alias.Equals(domain)
                                    && (department.Equals(Guid.Empty) || pdl.f_department.Equals(department))
                                    && ep.b_doctor
                                    && (specialization == 0 || pepl.f_post.Equals(specialization))
                            orderby ep.id, p.p.c_surname, p.p.c_name, p.p.c_patronymic, pepl.n_type
                            select new { p, ep });

                var data2 = data.ToArray()
                    .GroupBy(g => new { g.p.p.id })
                    .Select(s => new People
                    {
                        Id = s.Key.id,
                        FIO = s.First().p.p.c_surname + " " + s.First().p.p.c_name + " " + s.First().p.p.c_patronymic,
                        Photo = s.First().p.p.c_photo,
                        SNILS = s.First().p.p.c_snils,
                        Posts = s.Select(ep2 => new PeoplePost
                        {
                            Id = ep2.ep.id,
                            Name = ep2.ep.c_name
                        }).ToArray()
                    });

                if (data2.Any())
                    return data2.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получает отдельного сотрудника
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override People getPeopleItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from p in db.content_peoples
                             join pol in db.content_people_org_links on p.id equals pol.f_people into pol2
                             from pol in pol2.DefaultIfEmpty()
                             join pdl in db.content_people_department_links on pol.id equals pdl.f_people into pdl2
                             from pdl in pdl2.DefaultIfEmpty()
                             join d in db.content_departmentss on pdl.f_department equals d.id into d2
                             from d in d2.DefaultIfEmpty()
                             join os in db.content_org_structures on d.f_structure equals os.id into os2
                             from os in os2.DefaultIfEmpty()
                             join msel in db.content_main_specialist_employees_links on p.id equals msel.f_employee into msel2
                             from msel in msel2.DefaultIfEmpty()
                             join s in db.cms_sitess on msel.f_main_specialist equals s.f_content into s2
                             from s in s2.DefaultIfEmpty()
                             join ms in db.content_main_specialistss on msel.f_main_specialist equals ms.id into ms2
                             from ms in ms2.DefaultIfEmpty()
                             where p.id.Equals(id)
                             select new People
                             {
                                 Id = p.id,
                                 SNILS = p.c_snils,
                                 FIO = p.c_surname + " " + p.c_name + " " + p.c_patronymic,
                                 XmlInfo = p.xml_info,
                                 Photo = p.c_photo,
                                 StructureId = os.num,
                                 DepartmentId = d.id,
                                 DepartmentTitle = d.c_title,
                                 Domain = s.c_alias,
                                 MainSpec = ms.c_name
                             });
                
                if (query.Any())
                    return query.SingleOrDefault();

                return null;
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
                var data = db.cms_sitess.Where(w => w.c_alias == domain)
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
        /// Список должностей
        /// </summary>
        /// <returns></returns>
        public override PeoplePost[] getPeoplePosts()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = (from s in db.cms_sitess
                             join pol in db.content_people_org_links on s.f_content equals pol.f_org
                             join pepl in db.content_people_employee_posts_links on pol.f_people equals pepl.f_people
                             join ep in db.content_employee_postss on pepl.f_post equals ep.id
                             where s.c_alias.Equals(domain) && ep.b_doctor
                             select new PeoplePost
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
        /// Получем организацию
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override OrgsModel getOrgInfo()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess.Where(w => w.c_alias == domain)
                             .Join(db.content_orgss, e => e.f_content, o => o.id, (e, o) => o)
                             .Select(s => new OrgsModel
                             {
                                 Address = s.c_adress,
                                 Phone = s.c_phone,
                                 Fax = s.c_fax,
                                 Email = s.c_email,
                                 PhoneReception = s.c_phone_reception,
                                 GeopointX = s.n_geopoint_x,
                                 GeopointY = s.n_geopoint_y
                             });

                if (data.Any())
                    return data.First();

                return null;
            }
        }

        /// <summary>
        /// Получем список голосования
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public override IEnumerable<VoteModel> getVote( string Ip)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
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
                var query = db.cms_sitess.Where(w => w.c_alias == domain);
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
                                Surname = s.c_surname,
                                Name = s.c_name,
                                Patronymic = s.c_patronymic,
                                Phone = s.c_phone,
                                Photo = new Photo { Url = s.c_photo },
                                Post = s.c_post,
                                Text = s.c_text,
                                PeopleF = s.f_people
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
            using (var db = new CMSdb(_context))
            {
                return
                    (from s in db.cms_sitess
                     join o in db.content_orgss on s.f_content equals o.id
                     where s.c_alias.Equals(_domain)
                     select o.f_oid
                    ).SingleOrDefault();
            }
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
        public override OrgFrontModel[] getOrgModels(Guid? type)
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
                                 where s.id == null || s.f_content.Equals(o.id)
                                 join p in db.content_people_org_links on a.f_people equals p.id into ts
                                 from p in ts.DefaultIfEmpty()
                                 where p.id == null || p.id == a.f_people
                                 where t.f_type.Equals(type) && o.f_oid != null
                                 select new OrgFrontModel
                                 {
                                     Id = o.id,
                                     Title = o.c_title,
                                     Phone = o.c_phone,
                                     PhoneReception = o.c_phone_reception,
                                     Fax = o.c_fax,
                                     Email = o.c_email,
                                     Address = o.c_adress,
                                     Logo = o.c_logo,
                                     Link = s.c_alias,
                                     Leader = new OrgsAdministrative
                                     {
                                         id = p.f_people,
                                         Surname = a.c_surname,
                                         Name = a.c_name,
                                         Patronymic = a.c_patronymic,
                                         Post = a.c_post,
                                         PeopleF = a.f_people,
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
                                 join p in db.content_people_org_links on a.f_people equals p.id into ts
                                 from p in ts.DefaultIfEmpty()
                                 where p.id == null || p.id == a.f_people
                                 orderby o.n_sort
                                 where (s.id == null || s.f_content.Equals(o.id)) && o.f_oid != null
                                 select new OrgFrontModel
                                 {
                                     Id = o.id,
                                     Title = o.c_title,
                                     Phone = o.c_phone,
                                     PhoneReception = o.c_phone_reception,
                                     Fax = o.c_fax,
                                     Email = o.c_email,
                                     Address = o.c_adress,
                                     Logo = o.c_logo,
                                     Link = s.c_alias,
                                     Affiliation = o.f_department_affiliation,
                                     Leader = new OrgsAdministrative
                                     {
                                         id = p.f_people,
                                         Surname = a.c_surname,
                                         Name = a.c_name,
                                         Patronymic = a.c_patronymic,
                                         Post = a.c_post,
                                         PeopleF = a.f_people,
                                         Photo = new Photo { Url = a.c_photo }
                                     }
                                 });

                    if (query.Any())
                        return query.ToArray();

                    return null;
                }
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
        public override MedicalService[] getMedicalServices(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrWhiteSpace(domain))
                {
                    var query = (from s in db.cms_sitess
                                 join o in db.content_orgss on s.f_content equals o.id
                                 join omsl in db.content_orgs_medical_services_linkss on o.id equals omsl.f_org
                                 join ms in db.content_medical_servicess on omsl.f_medical_service equals ms.id
                                 where s.c_alias.Equals(domain)
                                 orderby ms.n_sort
                                 select new MedicalService
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
                                 select new MedicalService
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

        /// <summary>
        /// Получим список организаций, представляющих медицинскую услугу
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override OrgFrontModel[] getOrgPortalModels(Guid service)
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
                             join p in db.content_people_org_links on a.f_people equals p.id into ts
                             from p in ts.DefaultIfEmpty()
                             where p.id == null || p.id == a.f_people
                             where omsl.f_medical_service.Equals(service)
                             select new OrgFrontModel
                             {
                                 Id = o.id,
                                 Title = o.c_title,
                                 Phone = o.c_phone,
                                 PhoneReception = o.c_phone_reception,
                                 Fax = o.c_fax,
                                 Email = o.c_email,
                                 Address = o.c_adress,
                                 Logo = o.c_logo,
                                 Link = s.c_alias,
                                 Leader = new OrgsAdministrative
                                 {
                                     id = p.f_people,
                                     Surname = a.c_surname,
                                     Name = a.c_name,
                                     Patronymic = a.c_patronymic,
                                     Post = a.c_post,
                                     PeopleF = a.f_people,
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
        /// Получаем список врачей для портала
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <returns>Список врачей</returns>
        public override DoctorList getDoctorsList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var search = !string.IsNullOrWhiteSpace(filter.SearchText)
                    ? filter.SearchText.ToLower().Split(' ') : null; // поиск по человеку


                var people = (from  p in db.content_peoples
                              select  p);

                if (search != null)
                {
                    foreach (string item in search)
                    {
                        people = people.Where(w => w.c_surname.Contains(item)
                                                || w.c_name.Contains(item)
                                                || w.c_patronymic.Contains(item));
                    }
                }
                string post = !string.IsNullOrWhiteSpace(filter.Type)
                    ? filter.Type : null;

                var doctors = (from p in people
                               join pol in db.content_people_org_links on p.id equals pol.f_people
                               join o in db.content_orgss on pol.f_org equals o.id
                               join s in db.cms_sitess on o.id equals s.f_content into ss
                               from s in ss.DefaultIfEmpty()
                               where s.f_content == null || s.f_content == o.id
                               join pepl in db.content_people_employee_posts_links on p.id equals pepl.f_people
                               join ep in db.content_employee_postss on pepl.f_post equals ep.id
                               where post == null || pepl.f_post.ToString().Equals(post)
                               orderby p.c_surname, p.c_name, p.c_patronymic, ep.c_name
                               select new
                               {
                                   p = new
                                   {
                                       id = p.id,
                                       surname = p.c_surname,
                                       name = p.c_name,
                                       patronymic = p.c_patronymic,
                                       photo = p.c_photo
                                   },
                                   ep = new
                                   {
                                       id = ep.id,
                                       name = ep.c_name,
                                       org = pepl.f_org_guid,
                                       title = o.c_title,
                                       domain = s.c_alias
                                   }
                               });

                // кол-во докторов
                int itemCount = doctors.Count();

                doctors = doctors
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => s);

                var doctors2 = doctors
                    .ToArray()
                    .GroupBy(g => g.p.id)
                    .Select(s => new People
                    {
                        Id = s.First().p.id,
                        FIO = s.First().p.surname + " " + s.First().p.name + " " + s.First().p.patronymic,
                        Photo = s.First().p.photo,
                        Posts = s.Select(ep2 => new PeoplePost
                        {
                            Id = ep2.ep.id,
                            Name = ep2.ep.name,
                            OrgId = ep2.ep.org,
                            OrgAlias = ep2.ep.domain,
                            OrgTitle = ep2.ep.title
                        }).ToArray()
                    });

                if (doctors2.Any())
                {
                    return new DoctorList
                    {
                        Doctors = doctors2.ToArray(),
                        Pager = new Pager
                        {
                            page = filter.Page,
                            size = filter.Size,
                            items_count = itemCount,
                            page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
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

                    return new FeedbacksList
                    {
                        Data = feedbacks,
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
                            c_code = feedback.AnswererCode,
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
                //write to log ex
                return false;
            }
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
                        //c_code = feedback.AnswererCode Возможно можно будет удалить

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

        /// <summary>
        /// Получаем список событий
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override EventsList getEvents(FilterParams filter)
        {
            string domain = _domain;
            DateTime now = DateTime.Now;

            using (var db = new CMSdb(_context))
            {
                var contentType = ContentType.EVENT.ToString().ToLower();

                var eventIds = (from s in db.cms_sitess
                             join cct in db.content_content_links on s.f_content equals cct.f_link
                             join e in db.content_eventss on cct.f_content equals e.id
                             where s.c_alias.Equals(domain) && cct.f_content_type.Equals(contentType)
                             select e.id);

                if (!eventIds.Any()) return null;

                var query = db.content_eventss
                    .Where(w => eventIds.Contains(w.id))
                    .Where(w => !w.b_disabled)
                    .Where(w => (w.d_date >= now) || (w.d_date_end >= now) 
                            || (w.b_annually && (w.d_date.DayOfYear >= now.DayOfYear || w.d_date_end.DayOfYear >= now.DayOfYear)));

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
                        Alias = s.c_alias,
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
                    return new EventsList
                    {
                        Data = eventList.ToArray(),
                        Pager = new Pager
                        {
                            page = filter.Page,
                            size = filter.Size,
                            items_count = itemCount,
                            page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
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
                        Alias = s.c_alias,
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
                     var data=query.OrderBy(o => o.n_sort)
                                   .Select(s => new PhotoModel()
                                   {
                                       PreviewImage = new Photo { Url = s.c_preview },
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
        public override MainSpecialistFrontModel[] getMainSpecialistList()
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from ms in db.content_main_specialistss
                             join s in db.cms_sitess on ms.id equals s.f_content into s2
                             from s in s2.DefaultIfEmpty()
                             select new MainSpecialistFrontModel
                             {
                                 Id = ms.id,
                                 Name = ms.c_name,
                                 Domain = s.c_alias
                             });

                if (!query.Any()) return null;
                return query.ToArray();
            }
        }
    }
}
