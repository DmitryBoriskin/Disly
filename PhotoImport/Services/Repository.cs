using cms.dbase.models;
using System.Linq;
using LinqToDB;
using PhotoImport.Models;
using System;

namespace PhotoImport.Services
{
    /// <summary>
    /// Репозиторий для работы с БД
    /// </summary>
    public class Repository
    {
        // контекст подключения
        private string _context = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Repository()
        {
            _context = "dbConnection";
        }

        /// <summary>
        /// Возвращает список альбомов организации
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public PhotoAlbumOld[] GetAlbums(int org, int? link)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from g in db._importCD_Gallerys
                             join jp in db.__importMD_JUR_PERSONs on g.F_ORGS equals jp.LINK
                             join o in db.content_orgss on jp.F_FRMP_OID equals o.f_oid
                             join s in db.cms_sitess on o.id equals s.f_content
                             where g.F_ORGS.Equals(org) && (link == null || g.LINK.Equals(link))
                             select new PhotoAlbumOld
                             {
                                 Link = g.LINK,
                                 Org = g.F_ORGS,
                                 Name = g.C_NAME,
                                 Description = g.C_DESCRIPTION,
                                 Date = g.D_DATE,
                                 Width = g.N_WIDTH,
                                 Height = g.N_HEIGHT,
                                 Folder = g.C_GALLERY_FOLDER,
                                 Time = g.D_TIME,
                                 Domain = s.c_alias
                             });

                return query.ToArray();
            }
        }

        /// <summary>
        /// Записывает инфу по новому фотоальбому
        /// </summary>
        /// <param name="album"></param>
        public void InsertPhotoAlbum(PhotoAlbumNew album)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_photoalbums
                    .Value(v => v.id, album.Id)
                    .Value(v => v.c_title, album.Title)
                    .Value(v => v.c_text, album.Text)
                    .Value(v => v.d_date, album.Date)
                    .Value(v => v.f_site, album.Domain)
                    .Value(v => v.c_path, album.Path)
                    .Value(v => v.c_disabled, album.Disabled)
                    .Value(v => v.n_old_id, album.OldId)
                    .Insert();
            }
        }

        /// <summary>
        /// Обновляет превьюшку для альбома
        /// </summary>
        /// <param name="preview"></param>
        public void UpdatePreviewAlbum(Guid id, string preview)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_photoalbums
                    .Where(w => w.id.Equals(id))
                    .Set(u => u.c_preview, preview)
                    .Update();
            }
        }

        /// <summary>
        /// Записывает инфу по фотографиям
        /// </summary>
        /// <param name="photo"></param>
        public void InsertPhotoItem(Photo photo)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_photoss
                    .Value(v => v.id, photo.Id)
                    .Value(v => v.f_album, photo.AlbumId)
                    .Value(v => v.c_title, photo.Title)
                    .Value(v => v.d_date, photo.Date)
                    .Value(v => v.c_preview, photo.Preview)
                    .Value(v => v.c_photo, photo.Original)
                    .Value(v => v.n_sort, photo.Sort)
                    .Insert();
            }
        }

        /// <summary>
        /// Возвращает список идентификаторов организаций
        /// </summary>
        /// <returns></returns>
        public int[] GetOrgIds()
        {
            using (var db = new CMSdb(_context))
            {
                return db.__importMD_JUR_PERSONs
                    .Select(s => s.LINK)
                    .ToArray();
            }
        }
    }
}
