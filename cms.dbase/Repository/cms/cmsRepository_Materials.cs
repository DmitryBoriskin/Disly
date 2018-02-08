using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;
using System.Web;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        #region private methods of class

        // Получение групп к которым относится новость
        private MaterialsGroup[] db_getMaterialGroups(CMSdb db, Guid materialId)
        {
            var query = db.content_materials_groups_links.AsQueryable();

            if (materialId != Guid.Empty)
            {
                query = query.Where(w => w.f_material == materialId);
            }

            var data = query
                 .Select(s => new MaterialsGroup
                 {
                     Id = s.f_group,
                     Title = s.fkcontentmaterialsgroups.c_title
                 });

            if (!data.Any())
                return null;

            return data.ToArray();
        }

        //Данная функция должна вызываться в рамках транзакции
        private void db_updateMaterialGroups(CMSdb db, Guid materialId, Guid[] groups)
        {
            //Удаляем привязанные группы
            db.content_materials_groups_links
                    .Where(w => w.f_material == materialId)
                    .Delete();

            // привязываем новые группы
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    if (group != Guid.Empty)
                    {
                        var materialGroup = new content_materials_groups_link
                        {
                            id = Guid.NewGuid(),
                            f_material = materialId,
                            f_group = group
                        };
                        db.Insert(materialGroup);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Получение групп к которым относится новость
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public override MaterialsGroup[] getMaterialGroups(Guid materialId)
        {
            using (var db = new CMSdb(_context))
            {
                return db_getMaterialGroups(db, materialId);
            }
        }

        public override MaterialsGroup[] getAllMaterialGroups()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_materials_groupss
                    .OrderBy(o => o.n_sort)
                     .Select(s => new MaterialsGroup
                     {
                         Id = s.id,
                         Title = s.c_title,
                         Alias = s.c_alias.ToLower()
                     });

                if (!data.Any())
                    return null;

                return data.ToArray();
            }
        }



        public override GroupModel getMaterialGroup(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_materials_groupss
                    .Where(p => p.c_alias.ToLower() == alias)
                    .OrderBy(o => o.n_sort)
                    .Select(s => new GroupModel
                    {
                        Id = s.id,
                        GroupName = s.c_title,
                        Alias = s.c_alias.ToLower()
                    });

                if (data.Any())
                    return data.SingleOrDefault();

                return null;
            }
        }

        /// <summary>
        /// Изменение группы, только название группе меняем
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public override bool updateMaterialGroup(GroupModel group)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var getGroup = db.content_materials_groupss
                        .Where(g => g.c_alias.ToLower() == group.Alias.ToLower());

                    if (getGroup.Any())
                    {
                        var cdGroup = getGroup.SingleOrDefault();

                        cdGroup.c_title = group.GroupName;
                        db.Update(cdGroup);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.MaterialGroup,
                            Action = LogAction.update,
                            PageId = cdGroup.id,
                            PageName = group.GroupName,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);
                    }
                    else
                    {
                        var cdGroup = new content_materials_groups()
                        {
                            id = Guid.NewGuid(),
                            c_alias = group.Alias.ToLower(),
                            c_title = group.GroupName
                        };
                        db.Insert(cdGroup);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.MaterialGroup,
                            Action = LogAction.insert,
                            PageId = cdGroup.id,
                            PageName = group.GroupName,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override bool deleteMaterialGroup(string alias)
        {
            if (string.IsNullOrEmpty(alias))
                return false;

            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var getGroup = db.content_materials_groupss.
                        Where(g => g.c_alias.ToLower() == alias.ToLower());

                    if (!getGroup.Any())
                        return false;

                    var cdGroup = getGroup.SingleOrDefault();
                    var groupId = cdGroup.id;
                    var groupName = cdGroup.c_title;

                    db.cms_resolutions_templatess
                        .Where(g => g.f_user_group == alias)
                        .Delete();
                    getGroup.Delete();

                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.UserGroup,
                        Action = LogAction.delete,
                        PageId = groupId,
                        PageName = groupName,
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
        /// Получим список новостей
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(MaterialFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                //var query = db.content_materialss.AsQueryable();

                //if (!string.IsNullOrEmpty(filtr.Domain))
                //{
                //    var contentType = ContentType.MATERIAL.ToString().ToLower();

                //    var materials = db.content_content_links.Where(e => e.f_content_type == contentType)
                //        .Join(db.cms_sitess.Where(o => o.c_alias.ToLower() == filtr.Domain),
                //                e => e.f_link,
                //                o => o.f_content,
                //                (e, o) => e.f_content
                //                );

                //    if (!materials.Any())
                //        return null;

                //    query = query
                //        .Where(w => materials.Contains(w.id));
                //}

                var query = db.content_sv_materials_links_sitess.AsQueryable();

                #region Filter
                //Фильтр по домену
                if (!string.IsNullOrEmpty(filtr.Domain))
                {
                    query = query.Where(p => p.site_alias == filtr.Domain);
                }

                //Фильтр по привязке к др. контенту (организация и тп)
                //if (filtr.RelId.HasValue && filtr.RelId.Value != Guid.Empty)
                //{
                //    query = query.Where(s => s.f_content == filtr.RelId.Value)
                //                    .Where(s => s.f_content_type == filtr.RelType.ToString().ToLower());
                //}

                //Фильтр по тексту

                if (!string.IsNullOrEmpty(filtr.Group))
                {
                    //query = query.Where(s => s.fkcontentmaterialsgroupslinkmaterials.Any(g => g.fkcontentmaterialsgroups.c_alias.ToLower() == filtr.Group));
                    query = query.Where(g => db.content_materials_groups_links
                                                .Any(t => t.fkcontentmaterialsgroups.c_alias == filtr.Group && t.f_material == g.material_id));

                }

                if (!string.IsNullOrEmpty(filtr.SearchText))
                    query = query.Where(s => s.c_title.ToLower().Contains(filtr.SearchText));


                if (filtr.Date.HasValue)
                    query = query.Where(s => s.d_date > filtr.Date.Value);

                if (filtr.DateEnd.HasValue)
                    query = query.Where(s => s.d_date < filtr.DateEnd.Value.AddDays(1));

                if (filtr.Disabled.HasValue)
                    query = query.Where(s => s.b_disabled == filtr.Disabled.Value);
                
                #endregion


                query = query.OrderByDescending(w => w.d_date);

                int itemCount = query.Count();

                var materialsList = query
                        .Skip(filtr.Size * (filtr.Page - 1))
                        .Take(filtr.Size)
                        .Select(s => new MaterialsModel
                        {
                            Id = s.material_id,
                            Title = s.c_title,
                            Alias = s.c_alias.ToLower(),
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
                            Important = s.material_important,
                            Locked = s.b_locked,
                            CountSee = s.n_count_see,
                                //Links  заполняем в контроллере
                            });

                if (materialsList.Any())
                    return new MaterialsList()
                    {
                        Data = materialsList.ToArray(),
                        Pager = new Pager()
                        {
                            Page = filtr.Page,
                            Size = filtr.Size,
                            ItemsCount = itemCount,
                            //PageCount = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                        }
                    };

                return null;
            }
        }

        /// <summary>
        /// Получим единичную запись новости
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override MaterialsModel getMaterial(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                //Проверка на домен???

                var data = db.content_materialss
                    .Where(w => w.id == id)
                    .Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias.ToLower(),
                        PreviewImage = new Photo()
                        {
                            Url = s.c_preview,
                            Source = s.c_preview_source
                        },
                        Text = s.c_text,
                        Url = s.c_url,
                        UrlName = s.c_url_name,
                        Date = s.d_date,
                        Keyw = s.c_keyw,
                        Desc = s.c_desc,
                        Disabled = s.b_disabled,
                        Important = s.b_important,
                        SmiType = s.c_smi_type,
                        CountSee = s.n_count_see,
                        Locked = s.b_locked,
                        ContentLink = (Guid)s.f_content_origin,
                        ContentLinkType = s.c_content_type_origin,
                        GroupsId = s.fkcontentmaterialsgroupslinkmaterials
                                    .Select(g =>
                                    g.f_group).ToArray(),
                        Groups = s.fkcontentmaterialsgroupslinkmaterials
                                    .Select(g => new MaterialsGroup()
                                    {
                                        Id = g.f_group,
                                        Title = g.fkcontentmaterialsgroups.c_title
                                    }).ToArray()
                    });

                if (data.Any())
                {
                    var data1 = data.Single();
                    data1.Important = db.content_content_links.Where(w => w.f_content == id && w.f_link == data.First().ContentLink && w.b_important == true).Any();
                    return data1;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Добавляем запись
        /// </summary>
        /// <param name="material">Новость</param>
        /// <returns></returns>
        public override bool insertCmsMaterial(MaterialsModel material)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
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
                            c_alias = material.Alias.ToLower(),
                            c_text = material.Text,
                            d_date = material.Date,
                            c_preview = (material.PreviewImage != null) ? material.PreviewImage.Url : null,
                            c_preview_source = (material.PreviewImage != null) ? material.PreviewImage.Source : null,
                            c_url = material.Url,
                            c_url_name = material.UrlName,
                            c_desc = material.Desc,
                            c_keyw = material.Keyw,
                            b_important = material.Important,
                            b_disabled = material.Disabled,
                            n_day = material.Date.Day,
                            n_month = material.Date.Month,
                            n_year = material.Date.Year,
                            f_content_origin = material.ContentLink,
                            c_content_type_origin = material.ContentLinkType,
                            b_locked = material.Locked,
                            c_smi_type = material.SmiType
                        };

                        // добавляем принадлежность к сущности(ссылку на организацию/событие/персону)
                        var cdMaterialLink = new content_content_link
                        {
                            id = Guid.NewGuid(),
                            f_content = material.Id,
                            f_content_type = ContentType.MATERIAL.ToString().ToLower(),
                            f_link = material.ContentLink,
                            f_link_type = material.ContentLinkType,
                            b_origin = true,
                            b_important = material.Important
                        };

                        #region удаляем признак важности в остальных новостях
                        if (material.Important)
                        {

                            var q1 = db.content_content_links
                                .Where(w => w.f_link == material.ContentLink && w.f_content_type == "material" && w.b_important == true);

                            q1.Set(w => w.b_important, false)
                                .Update();
                        }
                        #endregion

                        db.Insert(cdMaterial);
                        db.Insert(cdMaterialLink);





                        db_updateMaterialGroups(db, material.Id, material.GroupsId);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.insert,
                            PageId = material.Id,
                            PageName = material.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

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
        /// Обновляем запись
        /// </summary>
        /// <param name="material">Новость</param>
        /// <returns></returns>
        public override bool updateCmsMaterial(MaterialsModel material)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_materials cdMaterial = db.content_materialss
                                                .Where(p => p.id == material.Id)
                                                .SingleOrDefault();
                        if (cdMaterial == null)
                            throw new Exception("Запись с таким Id не найдена");

                        cdMaterial.c_title = material.Title;
                        cdMaterial.c_alias = material.Alias.ToLower();
                        cdMaterial.c_text = material.Text;
                        cdMaterial.d_date = material.Date;

                        if (material.PreviewImage != null)
                        {
                            cdMaterial.c_preview = material.PreviewImage.Url;
                            cdMaterial.c_preview_source = material.PreviewImage.Source;
                        }
                        else
                        {
                            cdMaterial.c_preview = null;
                            cdMaterial.c_preview_source = null;
                        }

                        cdMaterial.c_url = material.Url;
                        cdMaterial.c_url_name = material.UrlName;
                        cdMaterial.c_desc = material.Desc;
                        cdMaterial.c_keyw = material.Keyw;
                        //cdMaterial.b_important = material.Important;
                        cdMaterial.b_disabled = material.Disabled;
                        cdMaterial.n_day = material.Date.Day;
                        cdMaterial.n_month = material.Date.Month;
                        cdMaterial.n_year = material.Date.Year;
                        cdMaterial.b_locked = material.Locked;
                        cdMaterial.c_smi_type = material.SmiType;

                        db.Update(cdMaterial);
                        db_updateMaterialGroups(db, material.Id, material.GroupsId);

                        #region главная новость
                        if (material.Important)
                        {
                            //удаляем признак важности в остальных новостях
                            db.content_content_links
                                .Where(w => w.f_link == material.ContentLink && w.f_content_type == "material" && w.b_important == true)
                                .Set(w => w.b_important, false)
                                .Update();
                        }
                        //признак важности
                        db.content_content_links
                            .Where(w => w.f_content == material.Id && w.f_link == material.ContentLink && w.f_content_type == "material")
                            .Set(s => s.b_important, material.Important)
                            .Update();

                        #endregion


                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.update,
                            PageId = material.Id,
                            PageName = material.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

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
        /// Удаляем новость
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override bool deleteCmsMaterial(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        var data = db.content_materialss
                                               .Where(p => p.id == id);
                        if (!data.Any())
                        {
                            throw new Exception("Запись с таким Id не найдена");
                        }

                        var cdMaterial = data.SingleOrDefault();

                        //Delete news_group_links
                        var q1 = db.content_materials_groups_links
                             .Where(s => s.f_material == id)
                             .Delete();
                        //Delete links to other objects
                        var q2 = db.content_content_links
                             .Where(s => s.f_content == id)
                             .Delete();

                        db.Delete(cdMaterial);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.delete,
                            PageId = cdMaterial.id,
                            PageName = cdMaterial.c_title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                var message = String.Format("cmsRepository: deleteCmsMaterial; id={0}", id);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Error, message, ex));

                return false;
            }
        }

        /// <summary>
        /// Получаем список групп для новостей
        /// </summary>
        /// <returns></returns>
        public override MaterialsGroup[] getMaterialsGroups()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_materials_groupss
                    .OrderBy(o => o.n_sort)
                    .Select(s => new MaterialsGroup
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
        /// добавляем ссылку на рсс ленту
        /// </summary>
        /// <param name="rsslink"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public override bool insertRssLink(string rsslink, string title)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_rss_links.Where(w => w.f_site == _domain && w.c_url.ToLower() == rsslink);
                if (query.Any())
                {
                    return false;//случай когда эта рсс лента уже подключена
                }


                db.content_rss_links
                    .Value(v => v.c_url, rsslink)
                    .Value(v => v.c_title, title)
                    .Value(v => v.f_site, _domain)
                    .Insert();
                return true;//успешное добалените рсс ленты
            }
        }


        public override bool delRssLink(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_rss_links.Where(w => w.id == id);
                if (data.Any())
                {
                    data.Delete();
                    return true;
                }
                return false;
            }
        }

        public override RssChannel[] getRssChannelMiniList()
        {
            using (var db = new CMSdb(_context))
            {
                //удаляем все rss объекты прошлой выборки
                db.content_rss_materialss.Where(w => w.f_site == _domain).Delete();

                var query = db.content_rss_links.Where(w => w.f_site == _domain);
                if (query.Any())
                {
                    return query.Select(s => new RssChannel()
                    {
                        Title = s.c_title,
                        RssLink = s.c_url,
                        id = s.id
                    }).ToArray();
                }
                return null;
            }
        }


        public override bool insertRssObject(MaterialsModel ins)
        {
            using (var db = new CMSdb(_context))
            {
                string Prev = (ins.PreviewImage != null) ? ins.PreviewImage.Url : null;
                db.content_rss_materialss
                   .Value(v => v.f_site, _domain)
                   .Value(v => v.d_date, ins.Date)
                   .Value(v => v.c_title, ins.Title)
                   .Value(v => v.c_preview, Prev)
                   .Value(v => v.c_text, ins.Text + "<p>Источник: " + ins.Url + "</p>")
                   .Value(v => v.c_desc, ins.Desc)
                   .Value(v => v.c_keyw, ins.Keyw)
                   .Value(v => v.c_url, ins.Url)
                   .Value(v => v.c_url_name, ins.UrlName)
                   .Insert();
                return true;


            }

        }

        public override RssItem[] getRssObjects()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_rss_materialss.Where(w => w.f_site == _domain);
                if (query.Any())
                {
                    query = query.OrderBy(o => o.d_date);
                    return query.Select(s => new RssItem()
                    {
                        title = s.c_title,
                        pubDate = s.d_date,
                        id = s.id,
                        link = s.c_url,
                        enclosure = s.c_preview,
                        New = true,
                        RssGuid = getSpotRssMaterial(s.c_url)
                    }).ToArray();
                }
                return null;
            }
        }

        public override MaterialsModel getRssMaterial(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_rss_materialss.Where(w => w.id == id);
                if (data.Any())
                {
                    return data.Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Date = s.d_date,

                        Year = Convert.ToInt32(s.d_date.ToString("yyyy")),
                        Month = Convert.ToInt32(s.d_date.ToString("MM")),
                        Day = Convert.ToInt32(s.d_date.ToString("dd")),
                        PreviewImage = (s.c_preview != null) ? new Photo { Url = s.c_preview } : null,
                        Text = s.c_text,
                        Url = s.c_url,
                        ImportRss = true
                        //ImportRss
                    }).Single();
                }
                return null;
            }
        }
        /// <summary>
        /// опрделяем была ли импотрирована э
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override string getSpotRssMaterial(string link)
        {
            using (var db = new CMSdb(_context))
            {
                var contentType = ContentType.MATERIAL.ToString().ToLower();

                var materials = db.content_content_links.Where(e => e.f_content_type == contentType)
                    .Join(db.cms_sitess.Where(o => o.c_alias.ToLower() == _domain),
                            e => e.f_link,
                            o => o.f_content,
                            (e, o) => e.f_content
                            );

                if (!materials.Any())
                    return null;

                var query = db.content_materialss.Where(w => materials.Contains(w.id) && w.c_url.ToLower() == link.ToLower());
                if (query.Count() > 0)
                {
                    return query.Single().id.ToString();
                }
                return null;
            }
        }


        /// <summary>
        /// запись материала из рсс
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public override bool insertCmsMaterialRss(MaterialsModel material)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_materials cdMaterial = db.content_materialss
                                                .Where(p => p.id == material.Id)
                                                .SingleOrDefault();
                        if (cdMaterial != null)
                        {
                            throw new Exception("Запись с таким Id уже существует");
                        }
                        Guid idMaterial = Guid.NewGuid();
                        cdMaterial = new content_materials
                        {
                            id = idMaterial,
                            c_title = material.Title,
                            c_alias = material.Alias.ToLower(),
                            c_text = material.Text,
                            d_date = material.Date,
                            c_preview = (material.PreviewImage != null) ? material.PreviewImage.Url : null,
                            c_preview_source = (material.PreviewImage != null) ? material.PreviewImage.Source : null,
                            c_url = material.Url,
                            c_url_name = material.UrlName,
                            c_desc = material.Desc,
                            c_keyw = material.Keyw,
                            b_important = material.Important,
                            b_disabled = material.Disabled,
                            n_day = material.Date.Day,
                            n_month = material.Date.Month,
                            n_year = material.Date.Year,
                            f_content_origin = material.ContentLink,
                            c_content_type_origin = material.ContentLinkType,
                            b_rss_import = true,
                            b_rss_guid = material.Id//записываем его старый id 

                        };

                        // добавляем принадлежность к сущности(ссылку на организацию/событие/персону)
                        var cdMaterialLink = new content_content_link
                        {
                            id = Guid.NewGuid(),
                            f_content = idMaterial,
                            f_content_type = ContentType.MATERIAL.ToString().ToLower(),
                            f_link = material.ContentLink,
                            f_link_type = material.ContentLinkType,
                            b_origin = true
                        };

                        db.Insert(cdMaterial);
                        db.Insert(cdMaterialLink);

                        db_updateMaterialGroups(db, material.Id, material.GroupsId);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.insert,
                            PageId = material.Id,
                            PageName = material.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

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

    }
}
