using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;
using System.Collections.Generic;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Получим список новостей
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
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

        /// <summary>
        /// Получим единичную запись новости
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="substance">Сущность</param>
        /// <returns></returns>
        public override MaterialsModel getMaterial(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
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
                        DefaultSite = (Guid)s.uui_origin,
                        DefaultSiteType = s.c_origin_type,
                        Group = (Guid)s.fkcontentmaterialslinks
                            .Where(w => w.f_link_id.Equals(s.uui_origin))
                            .Where(w => w.f_link_type.Equals(s.c_origin_type))
                            .Select(t => t.f_group)
                            .SingleOrDefault(),
                        Event = (Guid?)s.fkcontentmaterialslinks
                            //(Guid?)db.content_materials_links
                            //.Where(w => w.f_material.Equals(s.id))
                            .Where(w => w.f_link_type.Equals("event"))
                            .Select(t => t.f_link_id).SingleOrDefault()
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
                        c_preview = (material.PreviewImage != null)? material.PreviewImage.Url : null,
                        c_url = material.Url,
                        c_url_name = material.UrlName,
                        c_desc = material.Desc,
                        c_keyw = material.Keyw,
                        b_important = material.Important,
                        b_disabled = material.Disabled,
                        n_day = material.Date.Day,
                        n_month = material.Date.Month,
                        n_year = material.Date.Year,
                        uui_origin = material.DefaultSite,
                        c_origin_type = material.DefaultSiteType
                    };

                    // добавляем группу
                    var materialLink = new content_materials_link
                    {
                        f_material = material.Id,
                        f_link_id = material.DefaultSite,
                        f_link_type = material.DefaultSiteType,
                        f_group = material.Group
                    };

                    using (var tran = db.BeginTransaction())
                    {
                        db.Insert(cdMaterial);
                        db.Insert(materialLink);
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
                    cdMaterial.c_preview = (material.PreviewImage != null)? material.PreviewImage.Url : null ;
                    cdMaterial.c_url = material.Url;
                    cdMaterial.c_url_name = material.UrlName;
                    cdMaterial.c_desc = material.Desc;
                    cdMaterial.c_keyw = material.Keyw;
                    cdMaterial.b_important = material.Important;
                    cdMaterial.b_disabled = material.Disabled;
                    cdMaterial.n_day = material.Date.Day;
                    cdMaterial.n_month = material.Date.Month;
                    cdMaterial.n_year = material.Date.Year;

                    // обновляем группу
                    db.content_materials_links
                         .Where(w => w.f_material.Equals(material.Id))
                         .Where(w => w.f_link_id.Equals(material.DefaultSite))
                         .Set(u => u.f_group, material.Group)
                         .Update();

                    // обновляем событие
                    if (material.Event != null)
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
                                .Set(u => u.f_group, material.Group)
                                .Update();
                        }
                        else
                        {
                            db.content_materials_links
                                .Value(u => u.f_material, material.Id)
                                .Value(u => u.f_link_id, material.Event)
                                .Value(u => u.f_link_type, "event")
                                .Value(u => u.f_group, material.Group)
                                .Insert();
                        }
                    }

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
                        .Where(w => !w.f_link_id.Equals(model.Material.DefaultSite))
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
                                            .Value(v => v.f_group, model.Material.Group)
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
