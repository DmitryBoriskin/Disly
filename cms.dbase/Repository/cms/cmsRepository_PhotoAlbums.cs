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
        public override PhotoAlbumList getPhotoAlbum(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrEmpty(filter.Domain))
                {                    
                    var query = db.content_photoalbums.Where(w=>w.f_site==filter.Domain).AsQueryable();
                    if (query.Any())
                    {
                        query = query.OrderBy(o => o.d_date);
                        int itemCount = query.Count();
                        var photoalbumsList = query.Skip(filter.Size * (filter.Page - 1))
                                                   .Take(filter.Size)
                                                   .Select(s => new PhotoAlbum
                                                   {
                                                       Id = s.id,
                                                       Title = s.c_title,
                                                       Date = s.d_date,
                                                       PreviewImage = new Photo() { Url = s.c_preview }
                                                   });

                        return new PhotoAlbumList
                        {
                            Data = photoalbumsList.ToArray(),
                            Pager = new Pager
                            {
                                page = filter.Page,
                                size = filter.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                            }
                        };
                    }
                }
                return null;
            }
        }
        public override PhotoAlbum getPhotoAlbumItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_photoalbums
                           .Where(w => w.id == id)
                           .Select(s => new PhotoAlbum {
                               Id=s.id,
                               Title=s.c_title,
                               Date=s.d_date,
                               Path=s.c_path,
                               PreviewImage = new Photo() { Url = s.c_preview },
                               Text=s.c_text                               
                           });                
                if (query.Any())
                {
                    var data = query.Single();
                    //цепляем к альбому фотографии
                    data.Photos = db.content_photoss
                                    .Where(w => w.f_album == data.Id)
                                    .Select(s=>new PhotoModel() {
                                        Id=s.id,
                                        Title=s.c_title,
                                        PreviewImage=new Photo { Url=s.c_preview}
                                    }).ToArray();  
                    return data;
                }
                return null;
            }
        }
        public override bool insPhotoAlbum(Guid id, PhotoAlbum ins)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_photoalbum cdPhotoAlbum = db.content_photoalbums
                                                .Where(p => p.id == ins.Id)
                                                .SingleOrDefault();
                        if (cdPhotoAlbum != null)
                        {
                            throw new Exception("Запись с таким Id уже существует");
                        }
                        
                        cdPhotoAlbum = new content_photoalbum
                        {
                            id = ins.Id,
                            f_site=_domain,
                            c_path=ins.Path,
                            c_title = ins.Title,
                            c_preview = (ins.PreviewImage != null) ? ins.PreviewImage.Url : null,
                            c_text = ins.Text,
                            d_date = ins.Date                            
                        };
                    
                        db.Insert(cdPhotoAlbum);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.PhotoAlbums,
                            Action = LogAction.insert,
                            PageId = ins.Id,
                            PageName = ins.Title,
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
        public override bool updPhotoAlbum(Guid id, PhotoAlbum upd)
        {            
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_photoalbum cdPhoto = db.content_photoalbums
                                                .Where(p => p.id == upd.Id)
                                                .SingleOrDefault();
                        if (cdPhoto == null)
                            throw new Exception("Запись с таким Id не найдена");

                        cdPhoto.c_title = upd.Title;                        
                        cdPhoto.c_text = upd.Text;
                        cdPhoto.d_date = upd.Date;
                        cdPhoto.c_preview =(upd.PreviewImage == null) ? cdPhoto.c_preview : upd.PreviewImage.Url;
                        db.Update(cdPhoto);                        

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.PhotoAlbums,
                            Action = LogAction.update,
                            PageId = upd.Id,
                            PageName = upd.Title,
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
        public override bool delPhotoAlbum(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_photoalbums
                           .Where(w => w.id == id);
                    if (!data.Any())
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }
                    var cdPhotoAlbum = data.SingleOrDefault();
                    //удаление привязки
                    var q1 = db.content_content_links
                             .Where(s => s.f_content == id)
                             .Delete();
                    //удадение фотогаллереи
                    db.Delete(cdPhotoAlbum);
                }                
                return false;
            }
        }
        
        public override bool insertPhotos(Guid Id, PhotoModel[] photo)
        {
            using (var db = new CMSdb(_context))
            {
                var queryMaxSort = db.content_photoss
                                     .Where(w => w.f_album==Id)                
                                     .Select(s => s.n_sort);
                int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 0;
                if (photo != null)
                {
                    if (photo.Length > 0)
                    {
                        foreach (var item in photo)
                        {
                            maxSort++;
                            db.content_photoss
                              .Value(v => v.f_album, Id)
                              .Value(v => v.c_title, item.Title)
                              .Value(v => v.d_date, item.Date)
                              .Value(v => v.c_preview, item.PreviewImage.Url)
                              .Value(v => v.c_photo, item.PhotoImage.Url)
                              .Value(v => v.n_sort, maxSort)
                              .Insert();
                        }
                    }                    
                    return true;
                }
                return false;
            }
        }

    }
}
