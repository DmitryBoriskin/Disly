using cms.dbase.models;
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

                //var query = db.front_site_sections.Where(w => w.f_site == siteId && w.f_front_section == siteSection)
                //               .Join(db.front_page_viewss,e=>e.f_page_view,o=>o.id,(e,o)=>o);

                var query = (from s in db.front_site_sections
                             join v in db.front_page_viewss
                             on s.f_page_view equals v.id
                             where (s.f_site.Equals(siteId) && s.f_front_section.Equals(siteSection))
                             select v.c_url);
                if (query.Any())
                    ViewPath = query.SingleOrDefault();

                //if (query.Any()) {
                //    ViewPath = query.SingleOrDefault().c_url;
                //}

                //var data = db.front_sv_page_veiws.Where(w => w.f_site == siteId && w.f_pege_type == siteSection).FirstOrDefault();
                //if (data != null) {
                //    ViewPath = data.c_url;
                //}
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


        public override SiteMapModel getSiteMap(string path, string alias, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps.Where(w => w.c_path == path && w.c_alias == alias && w.f_site == domain);
                if (query.Any())
                {
                    var data = query.Select(s => new SiteMapModel
                    {
                        Title = s.c_title,
                        Text = s.c_text,
                        Alias = s.c_alias,
                        Path = s.c_path,
                        Id = s.id,
                        FrontSection = s.f_front_section
                    }).First();

                    return data;
                }
                return null;
            }

        }

        /// <summary>
        /// Получим текст для 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="frontSection"></param>
        /// <returns></returns>
        public override string getContactsText(string domain, string frontSection)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => w.f_front_section.Equals(frontSection))
                    .Select(s => s.c_text);

                if (!query.Any()) return null;
                return query.SingleOrDefault();
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
                                     Path = c.c_path,
                                     FrontSection = c.f_front_section,
                                     Url = c.c_url
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
        public override List<Breadcrumbs> getBreadCrumbCollection(string Url, string domain)
        {
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
                        //.ToList().Add(item_data);
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
                else
                {
                    return null;
                }
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



                query = query.Where(w => (w.n_year == _year) && (w.n_month == _month) && (w.n_day == _day) && (w.c_alias.ToLower() == alias.ToLower()));
                if (query.Any())
                {
                    return query.Select(s => new MaterialsModel
                    {
                        Title = s.c_title,
                        Text = s.c_text
                    }).First();
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
        /// <param name="domain"></param>
        /// <returns></returns>
        public override StructureModel[] getStructures(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                #region comments
                //var query = db.content_org_structures
                //            .Join(db.cms_sitess.Where(o => o.c_alias == domain), o => o.f_ord, e => e.f_content, (e, o) => e)
                //            .OrderBy(o => o.n_sort)
                //            .Select(s => new StructureModel()
                //            {
                //                Id = s.id,
                //                Title = s.c_title,
                //                Phone = s.c_phone,
                //                Num = s.num,
                //                GeopointX = s.n_geopoint_x,
                //                GeopointY = s.n_geopoint_y,
                //                // список департаментов
                //                Departments = (from st in db.content_org_structures
                //                               join d in db.content_departmentss
                //                               on st.id equals d.f_structure
                //                               where d.f_structure.Equals(s.id)
                //                               select new Departments
                //                               {
                //                                   Title = d.c_title,
                //                                   // список телефонов
                //                                   Phones = db.content_departments_phones
                //                                                .Where(w => w.f_department.Equals(d.id))
                //                                                .Select(dp => new DepartmentsPhone
                //                                                {
                //                                                    Label = dp.c_key,
                //                                                    Value = dp.c_val
                //                                                }).ToArray()
                //                               }).ToArray()
                //            });
                #endregion

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

                if (!data.Any()) return null;
                return data.ToArray();
            }
        }

        /// <summary>
        /// Отдельная структура
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public override StructureModel getStructureItem(string domain, int num)
        {
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
                {
                    return data.First();
                }
                return null;
            }
        }
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
                                Text = s.c_adress,
                                DirecorPost = s.c_director_post,
                                DirectorF = s.f_director

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
                        data.Boss = Boss.First();
                    }

                    return data;
                }
                return null;
            }

        }
        /// <summary>
        /// 
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
                                Text = s.c_adress,
                                DirecorPost = s.c_director_post,
                                DirectorF = s.f_director

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
                        data.Boss = Boss.Single();
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

                string search = filter.SearchText; // поиск по человеку

                int specialization = !string.IsNullOrWhiteSpace(filter.Type) ? Convert.ToInt32(filter.Type) : 0; // специализация

                var data = (from s in db.cms_sitess
                            join pol in db.content_people_org_links on s.f_content equals pol.f_org
                            join p in db.content_peoples on pol.f_people equals p.id
                            join pepl in db.content_people_employee_posts_links on p.id equals pepl.f_people
                            join ep in db.content_employee_postss on pepl.f_post equals ep.id
                            join pdl in db.content_people_department_links on pol.id equals pdl.f_people into ps
                            from pdl in ps.DefaultIfEmpty()
                            where s.c_alias.Equals(domain)
                                    && (department.Equals(Guid.Empty) || pdl.f_department.Equals(department))
                                    && ep.b_doctor
                                    && (string.IsNullOrWhiteSpace(search) || (p.c_surname.Contains(search)
                                                                                || p.c_name.Contains(search)
                                                                                || p.c_patronymic.Contains(search)))
                                    && (specialization == 0 || pepl.f_post.Equals(specialization))
                            orderby ep.id, p.c_surname, p.c_name, p.c_patronymic, pepl.n_type
                            select new { p, ep });


                var data2 = data.ToArray()
                    .GroupBy(g => new { g.p.id })
                    .Select(s => new People
                    {
                        Id = s.Key.id,
                        FIO = s.First().p.c_surname + " " + s.First().p.c_name + " " + s.First().p.c_patronymic,
                        Photo = s.First().p.c_photo,
                        Posts = s.Select(ep2 => new PeoplePost
                        {
                            Id = ep2.ep.id,
                            Name = ep2.ep.c_name
                        }).ToArray()
                    });

                if (!data2.Any()) return null;
                return data2.ToArray();

                #region comment
                //var query = db.cms_sitess
                //            .Where(w => w.c_alias == domain)
                //            .Join(db.content_people_org_links, e => e.f_content, o => o.f_org, (e, o) => o)
                //            .Join(db.content_peoples, m => m.f_people, n => n.id, (m, n) => n);

                //if (filter.SearchText != null)
                //{
                //    query = query.Where(w => (w.c_name.Contains(filter.SearchText) || w.c_surname.Contains(filter.SearchText) || w.c_patronymic.Contains(filter.SearchText)));
                //}
                //if (!String.IsNullOrEmpty(filter.Group))
                //{
                //    query = query
                //            .Join(db.content_people_org_links, e => e.id, o => o.f_people, (o, e) => new { e, o })
                //            .Join(db.content_people_department_links
                //                .Where(w => string.IsNullOrWhiteSpace(filter.Group) || w.f_department == Guid.Parse(filter.Group))
                //                , m => m.e.id, n => n.f_people, (m, n) => m.o);
                //}

                //#region временное решение
                //int post = !string.IsNullOrWhiteSpace(filter.Type) ? Int32.Parse(filter.Type) : 0;
                //if (post != 0)
                //{
                //    var queryWithPost = query
                //    .Join(db.content_people_employee_posts_links, p => p.id, pepl => pepl.f_people, (p, pepl) => new { p, pepl.f_post })
                //    .Join(db.content_employee_postss.Where(w => w.id.Equals(post)), pp => pp.f_post, ep => ep.id, (pp, ep) => new { pp, ep })
                //    .Select(s => new
                //    {
                //        people = s.pp.p,
                //        post = new PeoplePost
                //        {
                //            Id = s.ep.id,
                //            Name = s.ep.c_name
                //        }
                //    });

                //    if (queryWithPost.Any())
                //    {
                //        LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

                //        var result = queryWithPost.Select(s => new People
                //        {
                //            Id = s.people.id,
                //            FIO = s.people.c_surname + " " + s.people.c_name + " " + s.people.c_patronymic,
                //            Photo = s.people.c_photo,
                //            Posts = (from pep in db.content_people_employee_posts_links
                //                     join ep in db.content_employee_postss on pep.f_post equals ep.id
                //                     where pep.f_people.Equals(s.people.id)
                //                     select new PeoplePost
                //                     {
                //                         Id = ep.id,
                //                         Name = ep.c_name,
                //                         Type = pep.n_type
                //                     }).OrderBy(o => o.Type).ToArray()
                //        });

                //        return result.ToArray();
                //    }
                //    return null;
                //}
                //#endregion

                //var query1 = query.OrderBy(o => o.c_surname);
                //if (query1.Any())
                //{
                //    LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

                //    var result = query1.Select(s => new People()
                //    {
                //        Id = s.id,
                //        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                //        Photo = s.c_photo,
                //        Posts = (from pep in db.content_people_employee_posts_links
                //                 join ep in db.content_employee_postss on pep.f_post equals ep.id
                //                 where pep.f_people.Equals(s.id)
                //                 select new PeoplePost
                //                 {
                //                     Id = ep.id,
                //                     Name = ep.c_name,
                //                     Type = pep.n_type
                //                 }).OrderBy(o => o.Type).ToArray()
                //    });

                //    return result.ToArray();
                //}
                //return null;
                #endregion
            }
        }

        /// <summary>
        /// Получает отдельного сотрудника
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override People getPeopleItem(Guid id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                #region comments
                //var query = db.cms_sitess
                //              .Join(db.content_people_org_links, e => e.f_content, o => o.f_org, (e, o) => new { o, e })
                //              .Join(db.content_peoples, m => m.o.f_people, n => n.id, (m, n) => new { m, n });

                //if (!String.IsNullOrEmpty(domain))
                //{
                //    query = query.Where(w => w.m.e.c_alias == domain);
                //}

                //if (query.Any())
                //{
                //    return query.Select(s => new People
                //    {
                //        Id = s.n.id,
                //        FIO = s.n.c_surname + " " + s.n.c_name + " " + s.n.c_patronymic
                //    }).First();                    
                //}
                //return null;
                #endregion

                var query = db.content_peoples
                    .Where(w => w.id.Equals(id))
                    .Select(s => new People
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                        XmlInfo = s.xml_info,
                        Photo = s.c_photo
                    });

                if (!query.Any()) return null;
                return query.SingleOrDefault();
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

                if (!query.Any()) return null;
                return query.SingleOrDefault();
            }
        }

        /// <summary>
        /// сгруппированные по структурам департменты для выпадающего спика
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public override StructureModel[] getDeparatamentsSelectList(string domain)
        {
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



                if (data.Any()) return data.ToArray();
                return null;
            }
        }

        /// <summary>
        /// Список должностей
        /// </summary>
        /// <returns></returns>
        public override PeoplePost[] getPeoplePosts(string domain)
        {
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

                if (!data.Any()) return null;
                return data.ToArray();
            }
        }

        public override OrgsModel getOrgInfo(string domain)
        {
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
                                 GeopointY = s.n_geopoint_y,
                                 //Text = text
                             });
                if (data.Any())
                {
                    return data.First();
                }
                return null;
            }
        }


        public override IEnumerable<VoteModel> getVote(string domain, string Ip)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_votes
                            .Where(w => w.f_site == domain && w.b_disabled == false)
                            .OrderBy(o => o.d_date_start)
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
                {
                    return query.ToArray();
                }
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
                if (data.Single().d_date_end <= DateTime.Now) return true;//если опрос завершен по дате

                var _count = db.content_vote_userss.Where(w => w.f_vote == VoteId && w.c_ip == Ip).Count();
                if (_count > 0) return true;//если пользователь уже принял участие в опросе
                return false;
            }
        }
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
                {
                    return query.Single();
                }
                return null;
            }

        }

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
                {
                    return query.ToArray();
                }
                return null;
            }
        }
        public override VoteStat getVoteStat(Guid AnswerId, Guid VoteId, string Ip)
        {
            using (var db = new CMSdb(_context))
            {
                ////проверяем даны ли ранее ответы этим пользователем
                //var spot = db.content_vote_userss.Where(w =>(w.f_vote == VoteId && w.c_ip==Ip)).FirstOrDefault();
                //if (spot == null) return null;


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
        /// 
        /// </summary>
        /// <returns></returns>
        public override OrgsAdministrativ[] getAdministrativ(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var query= db.cms_sitess.Where(w => w.c_alias == domain);
                if (query.Any())
                {
                    var data = query.Single();
                    if (data.c_content_type == "org")
                    {
                        var adm = db.content_orgs_adminstrativs.Where(w => w.f_org == data.f_content);
                        if (adm.Any())
                        {
                            return adm
                                .OrderBy(o=>o.n_sort)
                                .Select(s => new OrgsAdministrativ() {
                                        Surname=s.c_surname,
                                        Name=s.c_name,
                                        Patronymic=s.c_patronymic,
                                        Phone=s.c_phone,
                                        Photo=new Photo {Url= s.c_photo}
                                    }).ToArray();
                        }
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
        public override string getOid(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                return (from s in db.cms_sitess
                        join o in db.content_orgss on s.f_content equals o.id
                        where s.c_alias.Equals(domain)
                        select o.f_oid).SingleOrDefault();
            }
        }
    }
}
