using ImportOldInfo.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ImportOldInfo
{
    /// <summary>
    /// Обработка изображений
    /// </summary>
    public static class PhotoImport
    {
        /// <summary>
        /// Разрешённые расширения
        /// </summary>
        private static string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        /// <summary>
        /// Создаёт фотоальбомы
        /// </summary>
        /// <param name="org"></param>
        /// <param name="repository"></param>
        /// <param name="helper"></param>
        public static void CreateAlbums(Org org, Repository repository, ParamsHelper helper)
        {
            // список фотоальбомов
            IEnumerable<PhotoAlbumOld> albums = repository.GetAlbums(org);
            int count = 1;
            foreach (var album in albums)
            {
                try
                {
                    // идентификатор нового альбома
                    Guid id = Guid.NewGuid();

                    string datePath = $"/{album.Time.ToString("yyyy_MM")}/{album.Time.ToString("dd")}/";
                    // путь в приложении
                    string localPath = $"{helper.UserFiles}{album.Domain}/Photo{datePath}{id}/";

                    // добавим в бд запись по фотоальбому
                    string _title = album.Name.Length > 512 ?
                        album.Name.Substring(0, 512) : album.Name;

                    PhotoAlbumNew newAlbum = new PhotoAlbumNew
                    {
                        Id = id,
                        Title = _title,
                        Text = album.Description,
                        Date = album.Time,
                        Domain = album.Domain,
                        Path = localPath,
                        Disabled = false,
                        OldId = album.Link
                    };

                    // добавление альбома
                    repository.InsertPhotoAlbum(newAlbum);
                    count++;
                }
                catch (Exception e)
                {
                    ServiceLogger.Error("{error}", e.ToString());
                }
            }

            ServiceLogger.Info("{work}", $"для организации {org.Alias} добавлено {count} альбомов");
        }

        /// <summary>
        /// Основная логика
        /// </summary>
        /// <param name="org">Идентификатор организации</param>
        public static void Execute(Org org, Repository repository, ParamsHelper helper)
        {
            // список фотоальбомов
            IEnumerable<PhotoAlbumOld> albums = repository.GetAlbums(org);

            DropAlreadyImportedAlbums(org, repository, helper);

            // текущий порядковый номер альбома
            int currentAlbumNumber = 0;

            // общее кол-во альбомов
            int countAlbums = albums.Count();

            foreach (var album in albums)
            {
                currentAlbumNumber++;
                try
                {
                    // идентификатор нового альбома
                    Guid id = Guid.NewGuid();

                    if (!String.IsNullOrWhiteSpace(album.Folder))
                    {
                        // путь до старой директории
                        string oldDirectory = ($"{helper.OldDirectory}{album.Org}{album.Folder.Replace(".", "")}").Replace("/", "\\");
                        
                        if (Directory.Exists(oldDirectory))
                        {
                            // информация по полученной директории
                            DirectoryInfo di = new DirectoryInfo(oldDirectory);

                            // список файлов в директории
                            FileInfo[] fi = di.GetFiles();

                            string datePath = $"/{album.Time.ToString("yyyy_MM")}/{album.Time.ToString("dd")}/";

                            // путь в приложении
                            string localPath = $"{helper.UserFiles}{album.Domain}/Photo{datePath}{id}/";

                            // путь для сохранения альбома
                            string savePath = helper.NewDirectory + localPath;

                            // создадим папку для нового альбома
                            if (!Directory.Exists(savePath))
                            {
                                DirectoryInfo _di = Directory.CreateDirectory(savePath);
                            }

                            // добавим в бд запись по фотоальбому
                            string _title = album.Name.Length > 512 ?
                                album.Name.Substring(0, 512) : album.Name;

                            PhotoAlbumNew newAlbum = new PhotoAlbumNew
                            {
                                Id = id,
                                Title = _title,
                                Text = album.Description,
                                Date = album.Time,
                                Domain = album.Domain,
                                Path = localPath,
                                Disabled = false,
                                OldId = album.Link
                            };

                            // добавление альбома
                            repository.InsertPhotoAlbum(newAlbum);

                            // номер фотки
                            int count = 0;

                            // превьюшка для альбома
                            string _previewAlbum = null;

                            List<content_photos> photos = new List<content_photos>();

                            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 70L);

                            foreach (var img in fi)
                            {
                                try
                                {
                                    if (allowedExtensions.Contains(img.Extension.ToLower()))
                                    {
                                        count++;

                                        // превьюшка для фотки
                                        using (Bitmap imgPrev = (Bitmap)Bitmap.FromFile(img.FullName))
                                        {
                                            var tempImgPrev = Imaging.Resize(imgPrev, 120, 120, "center", "center");
                                            tempImgPrev.Save($"{savePath}prev_{count}.jpg", myImageCodecInfo, myEncoderParameters);
                                            tempImgPrev.Dispose();
                                        }

                                        // основное изображение
                                        using (Bitmap imgReal = (Bitmap)Bitmap.FromFile(img.FullName))
                                        {
                                            var tmpImgReal = Imaging.Resize(imgReal, 2000, "width");
                                            tmpImgReal.Save($"{savePath}{count}.jpg", myImageCodecInfo, myEncoderParameters);
                                            tmpImgReal.Dispose();
                                        }

                                        // сохраняем превьюшку для альбома
                                        if (count == 1)
                                        {
                                            using (Bitmap albumPrev = (Bitmap)Bitmap.FromFile(img.FullName))
                                            {
                                                var tmpAlbumPrev = Imaging.Resize(albumPrev, 540, 360, "center", "center");
                                                tmpAlbumPrev.Save($"{savePath}albumprev.jpg", myImageCodecInfo, myEncoderParameters);
                                                tmpAlbumPrev.Dispose();
                                            }

                                            _previewAlbum = $"{localPath}albumprev.jpg";
                                            repository.UpdatePreviewAlbum(id, _previewAlbum);
                                        }

                                        content_photos photo = new content_photos
                                        {
                                            id = Guid.NewGuid(),
                                            f_album = id,
                                            c_title = $"{count}{img.Extension}",
                                            d_date = img.LastWriteTime,
                                            c_preview = $"{localPath}prev_{count}.jpg",
                                            c_photo = $"{localPath}{count}.jpg",
                                            n_sort = count
                                        };

                                        // добавление фото
                                        photos.Add(photo);
                                    }
                                }
                                catch (Exception e)
                                {
                                    ServiceLogger.Error("{error}", e.ToString());
                                }
                            }
                            if (photos != null && photos.Count() > 0)
                            {
                                repository.InsertPhotos(photos);
                            }
                        }

                        // сколько альбомов обработанно
                        if (currentAlbumNumber % 10 == 0)
                        {
                            ServiceLogger.Info("{work}", $"обработанно альбомов {currentAlbumNumber} из {countAlbums}");
                        }
                    }
                }
                catch (Exception e)
                {
                    ServiceLogger.Error("{error}", $"ошибка на шаге: {currentAlbumNumber} of {countAlbums}");
                    ServiceLogger.Error("{error}", e.ToString());
                }
            }
        }

        /// <summary>
        /// Удаляет уже импортированные альбомы
        /// </summary>
        private static void DropAlreadyImportedAlbums(Org org, Repository repository, ParamsHelper helper)
        {
            // новые импортированные альбомы
            IEnumerable<PhotoAlbumNew> newAlbums = repository.GetNewAlbumsForUpdate(org);
            // удаляем ранее импортированные альбомы 
            repository.DeletePhotoAlbums(org);

            foreach (var album in newAlbums)
            {
                string path = $"{helper.NewDirectory}{album.Path}";
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
        }

        /// <summary>
        /// Получает инфу для кодирования
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                    return enc;
            return null;
        }
    }
}
