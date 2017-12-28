using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {

        #region Anketa
 
        public override AnketaModel getAnketaItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {

                var data = db.content_anketass
                    .Where(w => w.id == id)
                    .Select(s => new AnketaModel
                    {
                        Id = s.id,
                        Count = s.n_count,
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Text = s.c_text,
                        Url = s.c_url,
                        DateBegin = s.d_date,
                        DateEnd = s.d_date_end,
                        Disabled = s.b_disabled,
                        //Links  заполняем в контроллере
                    });
                if (!data.Any())
                    return null;

                else
                    return data.Single();
            }
        }

        public override AnketasList getAnketasList(AnketaFilter filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_anketass.AsQueryable();
                int itemCount = query.Count();

                var List = query
                    .OrderByDescending(s => s.d_date)
                    .Skip(filtr.Size * (filtr.Page - 1))
                    .Take(filtr.Size)
                    .Select(s => new AnketaModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Text = s.c_text,
                        Url = s.c_url,
                        DateBegin = s.d_date,
                        DateEnd = s.d_date_end,
                        Disabled = s.b_disabled,
                        Count = s.n_count,
                    });

                if (!List.Any())
                    return null;

                return new AnketasList
                {
                    Data = List.ToArray(),
                    Pager = new Pager
                    {
                        page = filtr.Page,
                        size = filtr.Size,
                        items_count = itemCount,
                        page_count = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                    }
                };
            }
        }

        //public override AnketaModel[] getLastAnketasListWithCheckedFor(EventFilter filtr)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        var query = db.content_eventss
        //                        .Where(s => s.id != filtr.RelId); // Само на себя событие не может ссылаться - это зацикливание



        //        if (!string.IsNullOrEmpty(filtr.Domain))
        //        {
        //            var contentType = ContentType.EVENT.ToString().ToLower();
        //            var events = db.content_content_links.Where(e => e.f_content_type == contentType)
        //                    .Join(db.cms_sitess.Where(o => o.c_alias == filtr.Domain),
        //                            e => e.f_link,
        //                            o => o.f_content,
        //                            (e, o) => e.f_content
        //                            );

        //            if (!events.Any())
        //                return null;
        //            query = query.Where(w => events.Contains(w.id));
        //        }

        //        if (filtr.RelId.HasValue && filtr.RelId.Value != Guid.Empty)
        //        {
        //            var List = query
        //            .OrderByDescending(s => s.d_date)
        //            .Take(filtr.Size)
        //            .Select(s => new EventsShortModel()
        //            {
        //                Id = s.id,
        //                Title = s.c_title,
        //                DateBegin = s.d_date,
        //                Text = s.c_text,
        //                Checked = ContentLinkExists(filtr.RelId.Value, filtr.RelType, s.id, ContentLinkType.EVENT)
        //            });

        //            if (List.Any())
        //                return List.ToArray();
        //        }

        //        return null;

        //    }
        //}

        public override bool insertCmsAnketa(AnketaModel anketa)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_anketas cdAnketa = db.content_anketass
                                                        .Where(p => p.id == anketa.Id)
                                                        .SingleOrDefault();
                        if (cdAnketa != null)
                        {
                            throw new Exception("Запись с таким Id уже существует");
                        }

                        cdAnketa = new content_anketas
                        {
                            id = anketa.Id,
                            c_alias = anketa.Alias,
                            c_title = anketa.Title,
                            c_text = anketa.Text,
                            b_disabled = anketa.Disabled,
                            d_date = anketa.DateBegin,
                            d_date_end = (anketa.DateEnd.HasValue) ? anketa.DateEnd.Value : DateTime.Now.AddYears(1),
                            c_url = anketa.Url,
                        };

                        var cdContentLink = new content_content_link()
                        {
                            id = Guid.NewGuid(),
                            f_content = anketa.Id,
                            f_content_type = ContentType.ANKETA.ToString().ToLower(),
                            f_link = anketa.ContentLink,
                            f_link_type = anketa.ContentLinkType,
                            b_origin = true
                        };

                        db.Insert(cdAnketa);
                        db.Insert(cdContentLink);

                        //логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Anketa,
                            Action = LogAction.insert,
                            PageId = anketa.Id,
                            PageName = anketa.Title,
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
        public override bool updateCmsAnketa(AnketaModel anketa)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_anketas cdAnketa = db.content_anketass
                                                .Where(p => p.id == anketa.Id)
                                                .SingleOrDefault();
                        if (cdAnketa == null)
                        {
                            throw new Exception("Запись с таким Id не найдена");
                        }

                        var EndDate = (anketa.DateEnd.HasValue) ? anketa.DateEnd.Value : anketa.DateBegin;

                        cdAnketa.c_alias = anketa.Alias;
                        cdAnketa.c_title = anketa.Title;
                        cdAnketa.c_text = anketa.Text;
                        cdAnketa.b_disabled = anketa.Disabled;
                        cdAnketa.d_date = anketa.DateBegin;
                        cdAnketa.d_date_end = EndDate;
                        cdAnketa.c_url = anketa.Url;
                     

                        db.Update(cdAnketa);

                        //логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Anketa,
                            Action = LogAction.update,
                            PageId = anketa.Id,
                            PageName = anketa.Title,
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
        public override bool deleteCmsAnketa(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        var data = db.content_anketass
                                                .Where(p => p.id == id);

                        if (!data.Any())
                        {
                            throw new Exception("Запись с таким Id не найдена");
                        }

                        var cdAnketa = data.SingleOrDefault();

                        //Delete links to other objects
                        var q2 = db.content_content_links
                             .Where(s => s.f_content == id)
                             .Delete();

                        db.Delete(cdAnketa);

                        //логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Anketa,
                            Action = LogAction.delete,
                            PageId = cdAnketa.id,
                            PageName = cdAnketa.c_title,
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
        #endregion

    }
}
