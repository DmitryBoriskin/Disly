using System;
using System.Collections.Generic;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с картой сайта
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
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
        /// Получаем единичную запись группы меню
        /// </summary>
        /// <returns></returns>
        public override SiteMapMenu getSiteMapMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemap_menuss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new SiteMapMenu
                    {
                        Id = s.id,
                        Text = s.c_title,
                        Sort = s.n_sort
                    });

                if (!query.Any()) return null;
                else return query.FirstOrDefault();
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
        public override bool createOrUpdateSiteMapMenu(SiteMapMenu item)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemap_menuss.Where(w => w.id.Equals(item.Id));
                if (!query.Any())
                {
                    var sortMax = db.content_sitemap_menuss.Select(s => s.n_sort);

                    int max = sortMax.Any() ? sortMax.Max() + 1 : 1;

                    db.content_sitemap_menuss
                        .Value(v => v.c_title, item.Text)
                        .Value(v => v.n_sort, max)
                        .Insert();

                    return true;
                }
                else
                {
                    db.content_sitemap_menuss
                        .Where(w => w.id.Equals(item.Id))
                        .Set(u => u.c_title, item.Text)
                        .Update();
                    return true;
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
    }
}
