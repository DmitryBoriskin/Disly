using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с баннерами
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
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
                    //insertLog(userId, IP, "insert", id, String.Empty, "Banners", item.Title);
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Banners,
                        Action = LogAction.insert,
                        PageId = id,
                        PageName = item.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

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

                    string img = (item.Photo != null) ? item.Photo.Url: null;

                    query
                        .Set(s => s.c_title, item.Title)
                        .Set(s => s.c_photo, item.Photo.Url)
                        .Set(s => s.c_url, item.Url)
                        .Set(s => s.c_text, item.Text)
                        .Set(s => s.d_date, item.Date)
                        .Set(s => s.b_disabled, item.Disabled)
                        .Set(s => s.f_section, item.Section)
                        .Update();

                    // логирование
                    //insertLog(userId, IP, "update", id, string.Empty, "Banners", item.Title);
                    var log = new LogModel()
                    {
                        Site =_domain,
                        Section = LogSection.Banners,
                        Action = LogAction.update,
                        PageId = id,
                        PageName = item.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

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
                    //insertLog(userId, IP, "delete", id, string.Empty, "Banners", title);
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Banners,
                        Action = LogAction.delete,
                        PageId = id,
                        PageName = title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

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
    }
}
