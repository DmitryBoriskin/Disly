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
                 .Select(s => new MaterialsGroup
                 {
                     Id = s.id,
                     Title = s.c_title
                 });

                if (!data.Any())
                    return null;

                return data.ToArray();
            }
        }

        /// <summary>
        /// Получим список новостей для определенной сущности
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(MaterialFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
#warning content_sv_materials_sitess - можно грохнуть эту вьюху в базе
                #region old by view
                //var query = db.content_sv_materials_sitess
                //    .Where(w => w.id != null);
                //if(!string.IsNullOrEmpty(filtr.Domain))
                //    query = query.Where(w => w.domain.Equals(filtr.Domain));

                //query = query.OrderByDescending(o => o.d_date);
                //if (query.Any())
                //{
                //    int ItemCount = query.Count();
                //    var List = query
                //        .Select(s => new MaterialsModel
                //        {
                //            Id = s.id,
                //            Title = s.c_title,
                //            Alias = s.c_alias,
                //            PreviewImage = new Photo()
                //            {
                //                Url = s.c_preview
                //            },
                //            Text = s.c_text,
                //            Url = s.c_url,
                //            UrlName = s.c_url_name,
                //            Date = s.d_date,
                //            Keyw = s.c_keyw,
                //            Desc = s.c_desc,
                //            Disabled = s.b_disabled,
                //            Important = s.b_important
                //        }).
                //        Skip(filtr.Size * (filtr.Page - 1)).
                //        Take(filtr.Size);
                //    MaterialsModel[] materialsInfo = List.ToArray();
                //    return new MaterialsList
                //    {
                //        Data = materialsInfo,
                //        Pager = new Pager
                //        {
                //            page = filtr.Page,
                //            size = filtr.Size,
                //            items_count = ItemCount,
                //            page_count = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                //        }
                //    };
                //}
                #endregion

                var query = db.content_materials_links
                    .AsQueryable();

                if (!string.IsNullOrEmpty(filtr.Domain))
                {
                    var content = db_getDomainContentTypeId(db, filtr.Domain);
                    if (content != null && content.Id.HasValue)
                        query = query.Where(w => w.f_link_id == content.Id);
                }

                query = query.OrderByDescending(w => w.fkcontentmaterials.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var materialsList = query
                        .Select(s => new MaterialsModel
                        {
                            Id = s.fkcontentmaterials.id,
                            Title = s.fkcontentmaterials.c_title,
                            Alias = s.fkcontentmaterials.c_alias,
                            PreviewImage = new Photo()
                            {
                                Url = s.fkcontentmaterials.c_preview
                            },
                            Text = s.fkcontentmaterials.c_text,
                            Url = s.fkcontentmaterials.c_url,
                            UrlName = s.fkcontentmaterials.c_url_name,
                            Date = s.fkcontentmaterials.d_date,
                            Keyw = s.fkcontentmaterials.c_keyw,
                            Desc = s.fkcontentmaterials.c_desc,
                            Disabled = s.fkcontentmaterials.b_disabled,
                            Important = s.fkcontentmaterials.b_important
                        })
                        .Skip(filtr.Size * (filtr.Page - 1))
                        .Take(filtr.Size)
                        .ToArray();

                    return new MaterialsList
                    {
                        Data = materialsList,
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
        /// Получим единичную запись новости
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="substance">Сущность</param>
        /// <returns></returns>
        public override MaterialsModel getMaterial(Guid id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                //MaterialGroup[] groups = null;
                //if (!string.IsNullOrEmpty(domain))
                //{
                //    var content = db_getDomainContentTypeId(db, domain);
                //    if (content != null && content.Id.HasValue)
                //    {
                //        groups = db_getMaterialGroups(db, id, content.Id.Value);
                //    }
                //}

                var data = db.content_materialss
                    .Where(w => w.id == id)
                    .Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias,
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
                        Important = s.b_important,
                        ContentLink = (Guid)s.f_content_origin,
                        ContentLinkType = s.c_content_type_origin,

                        GroupsId = s.fkcontentmaterialsgroupslinkmaterials
                                    .Select(g =>
                                    g.f_group).ToArray(),

                        //GroupsId = s.fkcontentmaterialsgroupslinkmaterials
                        //            .Select(g => new SelectListItem()
                        //            {
                        //                Value = g.f_group.ToString(),
                        //                Text = g.fkcontentmaterialsgroups.c_title,
                        //                Selected = true
                        //            }).ToArray(),

                        Groups = s.fkcontentmaterialsgroupslinkmaterials
                                    .Select(g => new MaterialsGroup()
                                    {
                                        Id = g.f_group,
                                        Title = g.fkcontentmaterialsgroups.c_title
                                    }).ToArray()
                        //Groups = (Guid)s.fkcontentmaterialslinks
                        //    .Where(w => w.f_link_id.Equals(s.uui_origin))
                        //    .Where(w => w.f_link_type.Equals(s.c_origin_type))
                        //    .Select(t => t.f_group)
                        //    .SingleOrDefault(),
                        //Event = (Guid?)s.fkcontentmaterialslinks
                        //    //(Guid?)db.content_materials_links
                        //    //.Where(w => w.f_material.Equals(s.id))
                        //    .Where(w => w.f_link_type.Equals("event"))
                        //    .Select(t => t.f_link_id).SingleOrDefault()
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
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
                            c_alias = material.Alias,
                            c_text = material.Text,
                            d_date = material.Date,
                            c_preview = (material.PreviewImage != null) ? material.PreviewImage.Url : null,
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
                            c_content_type_origin = material.ContentLinkType
                        };

                        // добавляем принадлежность к сущности(ссылку на организацию/событие/персону)
                        var cdMaterialLink = new content_materials_link
                        {
                            id = Guid.NewGuid(),
                            f_material = material.Id,
                            f_link_id = material.ContentLink,
                            f_link_type = material.ContentLinkType,
                        };

                        db.Insert(cdMaterial);
                        db.Insert(cdMaterialLink);

                        db_updateMaterialGroups(db, material.Id, material.GroupsId);

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
                        cdMaterial.c_alias = material.Alias;
                        cdMaterial.c_text = material.Text;
                        cdMaterial.d_date = material.Date;
                        cdMaterial.c_preview = (material.PreviewImage == null) ? cdMaterial.c_preview : material.PreviewImage.Url;
                        cdMaterial.c_url = material.Url;
                        cdMaterial.c_url_name = material.UrlName;
                        cdMaterial.c_desc = material.Desc;
                        cdMaterial.c_keyw = material.Keyw;
                        cdMaterial.b_important = material.Important;
                        cdMaterial.b_disabled = material.Disabled;
                        cdMaterial.n_day = material.Date.Day;
                        cdMaterial.n_month = material.Date.Month;
                        cdMaterial.n_year = material.Date.Year;


                        // обновляем событие
                        /* if (material.Event != null)
                         {
                             var e = db.content_materials_links
                                 .Where(w => w.f_material.Equals(material.Id))
                                 .Where(w => w.f_link_type.Equals("event"));

                             if (e.Any())
                             {
                                 db.content_materials_links
                                     .Where(w => w.f_material.Equals(material.Id))
                                     .Where(w => w.f_link_type.Equals("event"))
                                     .Set(u => u.f_link_id, material.Event)
                                     .Set(u => u.f_group, material.Groups)
                                     .Update();
                             }
                             else
                             {
                                 db.content_materials_links
                                     .Value(u => u.f_material, material.Id)
                                     .Value(u => u.f_link_id, material.Event)
                                     .Value(u => u.f_link_type, "event")
                                     .Value(u => u.f_group, material.Groups)
                                     .Insert();
                             }
                         }*/

                        db.Update(cdMaterial);
                        db_updateMaterialGroups(db, material.Id, material.GroupsId);
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


        /// Добавляем связи новостей и организаций
        public override bool insertMaterialsOrgsLink(MaterialOrgsLink data)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {

                    //Удаляем существующие связи, кроме той организации, которой новость принадлежит.
                    db.content_materials_links
                                               .Where(w => w.f_material.Equals(data.MaterialId))
                                               .Where(w => !w.f_link_id.Equals(data.ContentLink))
                                               .Delete();

                    if (data.MaterialOrgs != null && data.MaterialOrgs.Count()>0)
                    {
                        foreach (var org in data.MaterialOrgs)
                        {
                            if (org != data.ContentLink)
                            {
                                db.content_materials_links
                                               .Value(v => v.f_material, data.MaterialId)
                                               .Value(v => v.f_link_id, org)
                                               .Value(v => v.f_link_type, "org")
                                               .Insert();
                            }

                        }

                    }
                    tran.Commit();
                }
                return true;
            }
        }

        /// <summary>
        /// Добавляем связи новостей и организаций
        /// </summary>
        /// <param name="material">Запись новости</param>
        /// <param name="orgTypes">Типы организаций</param>
        /// <returns></returns>
        public override bool insertMaterialsLinksToOrgs(MaterialOrgType model)
        {
            using (var db = new CMSdb(_context))
            {
                if (model.OrgTypes != null && model.OrgTypes.Count() > 0)
                {
                    db.content_materials_links
                        .Where(w => w.f_material.Equals(model.Material.Id))
                        .Where(w => !w.f_link_id.Equals(model.Material.ContentLink))
                        .Delete();

                    foreach (var t in model.OrgTypes)
                    {
                        if (t.Orgs != null)
                        {
                            foreach (var o in t.Orgs)
                            {
                                if (o.Check)
                                {
                                    bool isExist = db.content_materials_links
                                        .Where(w => w.f_material.Equals(model.Material.Id))
                                        .Where(w => w.f_link_id.Equals(o.Id))
                                        .Any();

                                    if (!isExist)
                                    {
                                        db.content_materials_links
                                            .Value(v => v.f_material, model.Material.Id)
                                            .Value(v => v.f_link_id, o.Id)
                                            .Value(v => v.f_link_type, "org")
                                            //.Value(v => v.f_group, model.Material.Groups)
                                            .Insert();
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Получаем список событий для новостей
        /// </summary>
        /// <returns></returns>
        public override MaterialsEvents[] getMaterialsEvents()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_eventss
                    .Where(w => !w.b_disabled)
                    .OrderByDescending(o => o.d_date)
                    .Select(s => new MaterialsEvents
                    {
                        Id = s.id,
                        Title = s.c_title
                    });

                if (!data.Any()) return null;
                else return data.ToArray();
            }
        }
    }
}
