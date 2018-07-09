using ImportOldInfo.Models;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportOldInfo
{
    /// <summary>
    /// Репозиторий для работы с БД
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Контекст для подключения
        /// </summary>
        private string context = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Repository()
        {
            context = "dbConnection";
        }

        /// <summary>
        /// Возвращает идентификаторы 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Org GetOrg(int id)
        {
            using (var db = new DbModel(context))
            {
                var res = db.get_org_ids(id).FirstOrDefault();
                return new Org
                {
                    Id = id,
                    UUID = (Guid)res.uuid,
                    Alias = res.alias
                };
            }
        }

        /// <summary>
        /// Возвращает список идентификаторов организаций
        /// </summary>
        /// <returns></returns>
        public int[] GetOrgsIds()
        {
            using (var db = new DbModel(context))
            {
                return db.import_get_old_orgs_ids()
                    .Select(s => s.LINK)
                    .ToArray();
            }
        }

        /// <summary>
        /// Возвращает список идентификаторов в нашей бд
        /// </summary>
        /// <returns></returns>
        public int[] GetSitesIds()
        {
            using (var db = new DbModel(context))
            {
                return db.cms_sitess
                    .Where(w => w.n_old_id != null)
                    .Select(s => (int)s.n_old_id)
                    .ToArray();
            }
        }

        /// <summary>
        /// Удаляет новости
        /// </summary>
        /// <param name="org"></param>
        public void DeleteMaterials(Org org)
        {
            using (var db = new DbModel(context))
            {
                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        int res = (int)db.delete_materials(org.UUID).Select(s => s.result).SingleOrDefault();
                        ServiceLogger.Info("{work}", $"новости для организации {org.Alias} удалены");
                        ServiceLogger.Info("{work}", $"кол-во удалённых новостей: {res}");
                        tr.Commit();
                    }
                    catch (Exception e)
                    {
                        ServiceLogger.Error("{error}", e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет новости
        /// </summary>
        /// <returns></returns>
        public bool ImportMaterials(Org org)
        {
            using (var db = new DbModel(context))
            {
                db.CommandTimeout = 1200000;
                if (db.Command != null)
                {
                    db.Command.CommandTimeout = 1200000;
                }
                try
                {
                    using (var tr = db.BeginTransaction())
                    {
                        var res = db.import_materials(org.UUID, org.Id).FirstOrDefault();
                        ServiceLogger.Info("{work}", $"материалы для организации {org.Alias} добавлены");
                        ServiceLogger.Info("{work}", $"кол-во новостей: {res.news}");
                        ServiceLogger.Info("{work}", $"кол-во публикаций: {res.publications}");
                        ServiceLogger.Info("{work}", $"кол-во анонсов: {res.anonses}");
                        tr.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    ServiceLogger.Error("{error}", e.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаляет обратную связь
        /// </summary>
        /// <param name="org"></param>
        public void DeleteFeedbacks(Org org)
        {
            using (var db = new DbModel(context))
            {
                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        int res = db.content_feedbackss.Where(w => w.f_site == org.Alias).Delete();
                        ServiceLogger.Info("{work}", $"обратная связь для организации {org.Alias} удалена");
                        ServiceLogger.Info("{work}", $"кол-во удалённой обратной связи: {res}");
                        tr.Commit();
                    }
                    catch (Exception e)
                    {
                        ServiceLogger.Error("{error}", e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет обратную связь
        /// </summary>
        /// <returns></returns>
        public bool ImportFeedbacks(Org org)
        {
            using (var db = new DbModel(context))
            {
                try
                {
                    using (var tr = db.BeginTransaction())
                    {
                        var res = db.import_feedback(org.Id, org.Alias).SingleOrDefault();
                        ServiceLogger.Info("{work}", $"обратная связь для организации {org.Alias} добавлена");
                        ServiceLogger.Info("{work}", $"кол-во отзывов {res.reviews}");
                        ServiceLogger.Info("{work}", $"кол-во вопросов и ответов: {res.appeals}");
                        tr.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    ServiceLogger.Error("{error}", e.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаляет вакансии
        /// </summary>
        /// <param name="org"></param>
        public void DeleteVacancies(Org org)
        {
            using (var db = new DbModel(context))
            {
                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        int res = db.content_vacanciess.Where(w => w.f_site == org.Alias).Delete();
                        ServiceLogger.Info("{work}", $"вакансии для организации {org.Alias} удалены");
                        ServiceLogger.Info("{work}", $"кол-во удалённых вакансий: {res}");
                        tr.Commit();
                    }
                    catch (Exception e)
                    {
                        ServiceLogger.Error("{error}", e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет вакансии
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public int ImportVacancies(Org org)
        {
            using (var db = new DbModel(context))
            {
                int result = 0;
                try
                {
                    using (var tr = db.BeginTransaction())
                    {
                        result = (int)db.import_vacancies(org.Id, org.Alias)
                                            .Select(s => s.result)
                                            .SingleOrDefault();
                        ServiceLogger.Info("{work}", $"вакансии для организации {org.Alias} добавлены");
                        ServiceLogger.Info("{work}", $"кол-во: {result}");
                        tr.Commit();

                    }
                }
                catch (Exception e)
                {
                    ServiceLogger.Error("{error}", e.ToString());
                }
                return result;
            }
        }

        /// <summary>
        /// Возвращает список фотоальбомов
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public PhotoAlbumOld[] GetAlbums(Org org)
        {
            using (var db = new DbModel(context))
            {
                return db.import_get_old_albums(org.Id)
                    .Select(s => new PhotoAlbumOld
                    {
                        Link = s.LINK,
                        Org = s.F_ORGS,
                        Name = s.C_NAME,
                        Description = s.C_DESCRIPTION,
                        Date = s.D_DATE,
                        Width = s.N_WIDTH,
                        Height = s.N_HEIGHT,
                        Folder = s.C_GALLERY_FOLDER,
                        Time = s.D_TIME,
                        Domain = org.Alias
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает список новых фотоальбомов
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public PhotoAlbumNew[] GetNewAlbums(Org org)
        {
            using (var db = new DbModel(context))
            {
                return db.content_photoalbums
                    .Where(w => w.f_site == org.Alias)
                    .Select(s => new PhotoAlbumNew
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        Domain = s.f_site,
                        Path = s.c_path,
                        Disabled = s.c_disabled,
                        OldId = s.n_old_id
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает список новых фотоальбомов для исправления косяков по изображениям
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public PhotoAlbumNew[] GetNewAlbumsForUpdate(Org org)
        {
            using (var db = new DbModel(context))
            {
                return db.content_photoalbums
                    .Where(w => w.f_site == org.Alias)
                    .Where(w => w.n_old_id != null)
                    .Select(s => new PhotoAlbumNew
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        Domain = s.f_site,
                        Path = s.c_path,
                        Disabled = s.c_disabled,
                        OldId = s.n_old_id
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает список новых фотоальбомов, у которых есть папка со старым путём
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public PhotoAlbumNew[] GetNewAlbumsWithPath(Org org)
        {
            using (var db = new DbModel(context))
            {
                return db.content_photoalbums
                    .Where(w => w.f_site == org.Alias)
                    .Where(w => w.c_old_path != null)
                    .Select(s => new PhotoAlbumNew
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        Domain = s.f_site,
                        Path = s.c_path,
                        Disabled = s.c_disabled,
                        OldId = s.n_old_id,
                        OldPath = s.c_old_path,
                        Org = s.f_org
                    }).ToArray();
            }
        }

        /// <summary>
        /// Удаляет фотки для альбома
        /// </summary>
        /// <param name="album"></param>
        public bool DropPhotos(Guid album)
        {
            using (var db = new DbModel(context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool res = db.content_photoss
                        .Where(w => w.f_album == album)
                        .Delete() > 0;

                    tr.Commit();
                    return res;
                }
            }
        }

        /// <summary>
        /// Удаляет импортированные фотоальбомы 
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public bool DeletePhotoAlbums(Org org)
        {
            using (var db = new DbModel(context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = db.content_photoalbums
                        .Where(w => w.f_site == org.Alias)
                        .Where(w => w.n_old_id != null)
                        .Delete() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Записывает инфу по новому фотоальбому
        /// </summary>
        /// <param name="album"></param>
        public bool InsertPhotoAlbum(PhotoAlbumNew album)
        {
            using (var db = new DbModel(context))
            {
                if (db.content_photoalbums.Where(w => w.n_old_id == album.OldId).Any())
                {
                    return false;
                }

                return db.content_photoalbums
                    .Value(v => v.id, album.Id)
                    .Value(v => v.c_title, album.Title)
                    .Value(v => v.c_text, album.Text)
                    .Value(v => v.d_date, album.Date)
                    .Value(v => v.f_site, album.Domain)
                    .Value(v => v.c_path, album.Path)
                    .Value(v => v.c_disabled, album.Disabled)
                    .Value(v => v.n_old_id, album.OldId)
                    .Insert() > 0;
            }
        }

        /// <summary>
        /// Обновляет превьюшку для альбома
        /// </summary>
        /// <param name="preview"></param>
        public bool UpdatePreviewAlbum(Guid id, string preview)
        {
            using (var db = new DbModel(context))
            {
                return db.content_photoalbums
                    .Where(w => w.id.Equals(id))
                    .Set(u => u.c_preview, preview)
                    .Update() > 0;
            }
        }

        /// <summary>
        /// Добавляет фотографии к альбому
        /// </summary>
        /// <param name="photos"></param>
        /// <returns></returns>
        public bool InsertPhotos(List<content_photos> photos)
        {
            using (var db = new DbModel(context))
            {
                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        db.BulkCopy(photos);
                        tr.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        ServiceLogger.Error("{error}", e.ToString());
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет фотогаллерии к новостям и структуре
        /// </summary>
        /// <param name="org"></param>
        public bool UpdateContentWithGallery(Org org)
        {
            using (var db = new DbModel(context))
            {
                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        var res = db.import_photos(org.Id).SingleOrDefault();
                        ServiceLogger.Info("{work}", $"фотогаллереи прикреплены к {org.Alias}");
                        ServiceLogger.Info("{work}", $"кол-во фото, прикреплённых к новостям: {res.news}");
                        ServiceLogger.Info("{work}", $"кол-во фото, прикреплённых к структуре: {res.structures}");
                        tr.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        ServiceLogger.Error("{error}", e.ToString());
                        return false;
                    }
                }
            }
        }
    }
}
