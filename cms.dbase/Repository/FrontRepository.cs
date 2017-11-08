﻿using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cms.dbase
{
    public class FrontRepository : abstract_FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
        }

        public FrontRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }

        /// <summary>
        /// Получение идентификатора сайта
        /// </summary>
        /// <param name="Domain">Домен</param>
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
        /// Получение вьюхи
        /// </summary>
        /// <param name="siteId">Домен</param>
        /// <param name="siteSection">Секция</param>
        /// <returns></returns>
        public override string getView(string siteId, string siteSection)
        {
            using (var db = new CMSdb(_context))
            {
                string ViewPath = "~/Error/404/";

                var data = db.front_sv_page_veiws.Where(w => w.f_site == siteId && w.f_pege_type == siteSection).FirstOrDefault();

                ViewPath = data.c_url;

                return ViewPath;
            }
        }

        /// <summary>
        /// Получение информации по сайту
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override SitesModel getSiteInfo(string domain)
        {
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

                if (!data.Any()) return null;
                else return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Получение меню из карты сайта
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapList(string domain)
        {
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
                        MenuGroups = getSiteMapGroupMenu(s.id)
                    });

                if (!data.Any()) return null;
                else return data.ToArray();
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

                if (!data.Any()) return null;
                else { return data.ToArray(); }
            }
        }

        /// <summary>
        /// Получение списка баннеров
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override BannersModel[] getBanners(string domain)
        {
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

                if (!data.Any()) return null;
                else return data.ToArray();
            }
        }





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


        public override SiteMapModel getSiteMap(string path,string alias,string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps.Where(w => w.c_path == path && w.c_alias == alias && w.f_site == domain);
                if (query.Any())
                {
                    var data= query.Select(s => new SiteMapModel {
                        Title=s.c_title,
                        Text=s.c_text,
                        Alias=s.c_alias,
                        Path=s.c_path,
                        Id=s.id
                        }).First();

                    
                    return data;
                }
                return null;
            }
            
        }
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
                                     Path = c.c_path
                                 }).ToArray();
                if (data.Any())
                {
                    return data;
                }
                return null;
            }
        }
        /// <summary>
        /// Получаем хленые крошки
        /// </summary>
        /// <param name="Url">относительная ссылка на страницу</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override Breadcrumbs[] getBreadCrumb(string Url, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                int _len = Url.Count();
                int _lastIndex = Url.LastIndexOf("/");
                var data = new List<Breadcrumbs>();
                while (_lastIndex > -1)
                {
                    string _path = Url.Substring(0, _lastIndex + 1).ToString();
                    string _alias = Url.Substring(_lastIndex + 1).ToString();
                    if (_alias == String.Empty) _alias = " ";

                    var item_data = db.content_sitemaps
                                    .Where(w => w.f_site == domain && w.c_path == _path && w.c_alias == _alias).Take(1)
                                    .Select(s=>new Breadcrumbs {
                                        Title=s.c_title,
                                        Url=s.c_path+s.c_alias
                                    }).SingleOrDefault();
                    if (item_data == null)
                    {
                    }
                    else data.Add(item_data);

                    Url = Url.Substring(0, _lastIndex);
                    _len = Url.Count();
                    _lastIndex = Url.LastIndexOf("/");

                }
                if (data.Any())
                {
                    data.Reverse();
                    return data.ToArray();
                }
                else
                {
            return null;
        }
            }
        }

        /// <summary>
        /// Получим список новостей для определенной сущности
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrEmpty(filtr.Domain))
                {
                    var contentType = ContentType.MATERIAL.ToString().ToLower();

                    //Запрос типа:
                    //Select t.*, s.* from[dbo].[content_content_link] t left join[dbo].[cms_sites] s
                    //on t.f_link = s.f_content Where s.c_alias = 'main'
                    var materials = db.content_content_links.Where(e => e.f_content_type == contentType)
                        .Join(db.cms_sitess.Where(o => o.c_alias == filtr.Domain),
                                e => e.f_link,
                                o => o.f_content,
                                (e, o) => e.f_content
                                );

                    if (!materials.Any())
                        return null;

                    var query = db.content_materialss
                                .Where(w => materials.Contains(w.id));

                    if (filtr.Disabled != null)
                    {
                        query = query.Where(w => w.b_disabled == filtr.Disabled);
                    }
                    if (!String.IsNullOrEmpty(filtr.Category))
                    {
                        var category = db.content_materials_groupss.Where(w => w.c_alias == filtr.Category).First().id;
                        query =query
                                    .Join(
                                            db.content_materials_groups_links
                                            .Where(o => o.f_group == category), 
                                            e => e.id, o => o.f_material, (o, e) => o
                                         );
                    }

                    query=query.OrderByDescending(w => w.d_date);

                    int itemCount = query.Count();

                    var materialsList = query
                            .Skip(filtr.Size * (filtr.Page - 1))
                            .Take(filtr.Size)
                            .Select(s => new MaterialsModel
                            {
                                Id = s.id,
                                Title = s.c_title,
                                Alias = s.c_alias,
                                Year=s.n_year,
                                Month=s.n_month,
                                Day=s.n_day,
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
                                page = filtr.Page,
                                size = filtr.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                            }
                        };
                }
                return null;
            }
        }

        /// <summary>
        /// Получаем новости для модуля на главной странице
        /// </summary>
        /// <returns></returns>
        public override List<MaterialFrontModule> getMaterialsModule(string domain)
        {
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
                    if (query.Any()) list.AddRange(query.Take(3));
                }

                if (list.Count() > 0) return list;
                else return null;
            }
        }
        public override MaterialsModel getMaterialsItem(string year, string month, string day, string alias, string domain)
        {
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



                query=query.Where(w => (w.n_year == _year) && (w.n_month == _month) && (w.n_day == _day) && (w.c_alias.ToLower()==alias.ToLower()));
                if (query.Any())
                {
                    return query.Select(s => new MaterialsModel {
                        Title=s.c_title,
                        Text=s.c_text
                            }).First();
                }
                return null;

            }
        }

        /// <summary>
        /// Выдает группы преесс-центра
        /// </summary>
        /// <returns></returns>
        public override MaterialsGroup[] getMaterialsGroup() {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_materials_groupss;
                if (data.Any())
                {
                    return data.OrderBy(o => o.n_sort)
                               .Select(s => new MaterialsGroup
                               {
                                     Alias=s.c_alias,
                                     Title=s.c_title                                 
                               }).ToArray();
                }
                return null;
            }
        }

    }
}
