using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
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
                        Logo = s.c_logo,
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
                    .Select(s => new BannersModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Photo = new Photo { Url = s.c_photo},
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
        /// <summary>
        /// Получим список новостей для определенной сущности
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
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


    }
}
