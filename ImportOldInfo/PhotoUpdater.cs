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
    /// Обновляем фотки
    /// </summary>
    public static class PhotoUpdater
    {
        /// <summary>
        /// Разрешённые расширения
        /// </summary>
        private static string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        /// <summary>
        /// Фиксим фотки
        /// </summary>
        /// <param name="org"></param>
        /// <param name="repository"></param>
        /// <param name="helper"></param>
        public static void Fix(Org org, Repository repository, ParamsHelper helper)
        {
            // старые альбомы
            IEnumerable<PhotoAlbumOld> oldAlbums = repository.GetAlbums(org);
            // новые альбомы
            IEnumerable<PhotoAlbumNew> newAlbums = repository.GetNewAlbumsForUpdate(org);

            // текущий порядковый номер альбома
            int currentAlbumNumber = 0;

            // общее кол-во альбомов
            int countAlbums = newAlbums.Count();

            foreach (var album in newAlbums)
            {
                currentAlbumNumber++;
                try
                {
                    var oldAlbum = oldAlbums.Where(w => w.Link == album.OldId).FirstOrDefault();
                    // путь до старой директории
                    string oldDirectory = ($"{helper.OldDirectory}{oldAlbum.Org}{oldAlbum.Folder.Replace(".", "")}").Replace("/", "\\");

                    if (Directory.Exists(oldDirectory))
                    {
                        // директория старого альбома
                        DirectoryInfo di = new DirectoryInfo(oldDirectory);
                        FileInfo[] fi = di.GetFiles();
                        if (fi != null && fi.Count() > 0)
                        {
                            string datePath = $"/{oldAlbum.Time.ToString("yyyy_MM")}/{oldAlbum.Time.ToString("dd")}/";

                            // путь в приложении
                            string localPath = $"{helper.UserFiles}{album.Domain}/Photo{datePath}{album.Id}/";

                            // путь для сохранения альбома
                            string savePath = helper.NewDirectory + localPath;

                            // удаляем фотки из альбома
                            repository.DropPhotos(album.Id);

                            if (Directory.Exists(savePath))
                            {
                                Directory.Delete(savePath, true);
                            }

                            DirectoryInfo _di = Directory.CreateDirectory(savePath);

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
                                    if (img != null && allowedExtensions.Contains(img.Extension.ToLower()))
                                    {
                                        count++;

                                        // превьюшка для фотки
                                        using (Bitmap imgPrev = (Bitmap)Bitmap.FromFile(img.FullName))
                                        {
                                            var tmpImg = Imaging.Resize(imgPrev, 120, 120, "center", "center");
                                            try
                                            {
                                                tmpImg.Save($"{savePath}prev_{count}.jpg", myImageCodecInfo, myEncoderParameters);
                                            }
                                            finally
                                            {
                                                tmpImg.Dispose();
                                            }
                                            imgPrev.Dispose();
                                        }
                                        
                                        // основное изображение
                                        using (Bitmap imgReal = (Bitmap)Bitmap.FromFile(img.FullName))
                                        {
                                            var tmpImgReal = Imaging.Resize(imgReal, 2000, "width");
                                            try
                                            {
                                                tmpImgReal.Save($"{savePath}{count}.jpg", myImageCodecInfo, myEncoderParameters);
                                            }
                                            finally
                                            {
                                                tmpImgReal.Dispose();
                                            }
                                            imgReal.Dispose();
                                        }
                                        
                                        // сохраняем превьюшку для альбома
                                        if (count == 1)
                                        {
                                            using (Bitmap albumPrev = (Bitmap)Bitmap.FromFile(img.FullName))
                                            {
                                                var tmpAlbumPrev = Imaging.Resize(albumPrev, 540, 360, "center", "center");
                                                try
                                                {
                                                    tmpAlbumPrev.Save($"{savePath}albumprev.jpg", myImageCodecInfo, myEncoderParameters);
                                                }
                                                finally
                                                {
                                                    tmpAlbumPrev.Dispose();
                                                }
                                                albumPrev.Dispose();
                                            }

                                            _previewAlbum = $"{localPath}albumprev.jpg";
                                            repository.UpdatePreviewAlbum(album.Id, _previewAlbum);
                                        }

                                        content_photos photo = new content_photos
                                        {
                                            id = Guid.NewGuid(),
                                            f_album = album.Id,
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
                    }

                    // сколько альбомов обработанно
                    if (currentAlbumNumber % 10 == 0)
                    {
                        ServiceLogger.Info("{work}", $"обработанно альбомов {currentAlbumNumber} из {countAlbums}");
                    }
                }
                catch (Exception e)
                {
                    ServiceLogger.Error("{error}", e.ToString());
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
