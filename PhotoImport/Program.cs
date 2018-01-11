using PhotoImport.Models;
using PhotoImport.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PhotoImport
{
    class Program
    {
        // разрешённые расширения
        static string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        // параметры из конфига
        static ImportParams _params = new ImportParams();

        // репозиторий
        private static Repository repository = new Repository();

        static void Main(string[] args)
        {
            Console.WriteLine("Import photogallery app");

            // список идентификаторов организаций
            int[] orgIds = repository.GetOrgIds();

            int id = 0;

            do
            {
                Console.WriteLine("Enter organisation id: ");
                try
                {
                    id = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Not corrected input!");
                    continue;
                }
            } while (!orgIds.Contains(id));

            Execute(id);

            Console.WriteLine("Import completed");
            Console.ReadKey();
        }

        /// <summary>
        /// Основная логика
        /// </summary>
        /// <param name="org">Идентификатор организации</param>
        private static void Execute(int org)
        {
            // список фотоальбомов
            IEnumerable<PhotoAlbumOld> albums = repository.GetAlbums(org, null);

            // текущий порядковый номер альбома
            int currentAlbumNumber = 0;

            // общее кол-во альбомов
            int countAlbums = albums.Count();

            foreach (var album in albums)
            {
                currentAlbumNumber++;

                // идентификатор нового альбома
                Guid id = Guid.NewGuid();

                // путь до старой директории
                string oldDirectory = (_params.OldDirectory + album.Org + album.Folder.Replace(".", "")).Replace("/", "\\");

                if (Directory.Exists(oldDirectory))
                {
                    // информация по полученной директории
                    DirectoryInfo di = new DirectoryInfo(oldDirectory);

                    // список файлов в директории
                    FileInfo[] fi = di.GetFiles();

                    string datePath = "/" + album.Time.ToString("yyyy_MM") + "/" + album.Time.ToString("dd") + "/";

                    // путь в приложении
                    string localPath = _params.UserFiles + album.Domain + "/Photo" + datePath + id + "/";

                    // путь для сохранения альбома
                    string savePath = _params.NewDirectory + localPath;

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

                    repository.InsertPhotoAlbum(newAlbum);

                    // номер фотки
                    int count = 0;

                    // превьюшка для альбома
                    string _previewAlbum = null;

                    foreach (var img in fi)
                    {
                        if (allowedExtensions.Contains(img.Extension.ToLower()))
                        {
                            count++;

                            // превьюшка для фотки
                            Bitmap imgPrev = (Bitmap)Bitmap.FromFile(img.FullName);
                            imgPrev = Imaging.Resize(imgPrev, 120, 120, "center", "center");
                            imgPrev.Save(savePath + "prev_" + count + img.Extension);

                            // основное изображение
                            Bitmap imgReal = (Bitmap)Bitmap.FromFile(img.FullName);
                            imgReal = Imaging.Resize(imgReal, 2000, "width");
                            imgReal.Save(savePath + count + img.Extension);

                            // сохраняем превьюшку для альбома
                            if (count == 1)
                            {
                                Bitmap albumPrev = (Bitmap)Bitmap.FromFile(img.FullName);
                                albumPrev = Imaging.Resize(albumPrev, 540, 360, "center", "center");
                                albumPrev.Save(savePath + "albumprev" + img.Extension);

                                _previewAlbum = localPath + "albumprev" + img.Extension;
                                repository.UpdatePreviewAlbum(id, _previewAlbum);
                            }

                            // новая фотка
                            Photo photo = new Photo
                            {
                                Id = Guid.NewGuid(),
                                AlbumId = id,
                                Title = count + img.Extension,
                                Date = img.LastWriteTime,
                                Preview = localPath + "prev_" + count + img.Extension,
                                Original = localPath + count + img.Extension,
                                Sort = count
                            };

                            repository.InsertPhotoItem(photo);
                        }
                    }
                }

                // сколько альбомов обработанно
                if (currentAlbumNumber % 10 == 0)
                    Console.WriteLine("Processed " + currentAlbumNumber + " of " + countAlbums);
            }
        }
    }
}
