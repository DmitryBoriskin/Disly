using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class PhotoAlbumsController : CoreController
    {
        string savePath = "/Userfiles/Photo/";
        string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".gif", ".png" };

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            #region Filter
            FilterViewModel filter = new FilterViewModel()
            {
                Sections = _repository.getSectionsGroup(ViewBag.ControllerName)
            };

            ViewBag.Filter = filter;
            #endregion

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Photoalbums
        public ActionResult Index()
        {
            PhotoAlbumsViewModel model = new PhotoAlbumsViewModel()
            {
                List = _repository.getPhotoAlbums(Domain),

                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo                
            };

            //перезаписываем обложку фотоальбома, если при редактировании была удалена прошлая обложка
            foreach(PhotoAlbumsModel Item in model.List)
            {
                if (!System.IO.File.Exists(Server.MapPath(Item.Preview)))
                {
                    try
                    {
                        Item.Preview = _repository.getPhotos(Item.Id)[0].Preview;
                        _repository.updatePhotoAlbumPreview(Item.Id, Item.Preview);
                    }
                    catch { ViewBag.PrevExists = false; }
                }

            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Create(Guid id)
        {
            PhotoAlbumsViewModel model = new PhotoAlbumsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo
            };
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd"); // Convert.ToDateTime(Model.Item.Date).ToString("yyyy-MM-dd")
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn")]
        public ActionResult Create(Guid id, PhotoAlbumsViewModel model, IEnumerable<HttpPostedFileBase> uploadPhoto)
        {
            ViewBag.BtnName = "Создать";
            
            if (ModelState.IsValid)
            {
                #region photos
                int counter = 0;
                savePath += DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM_dd") + "/";
                string serverPath = savePath + id.ToString();

                PhotosModel[] photoList = new PhotosModel[uploadPhoto.Count()];
                
                foreach (HttpPostedFileBase photos in uploadPhoto)
                {
                    if (photos != null && photos.ContentLength > 0)
                    {
                        try
                        {
                            if (!allowedExtensions.Contains(Path.GetExtension(photos.FileName)))
                            {
                                Exception ex = new Exception("неверный формат файла \"" + Path.GetFileName(photos.FileName) + "\". Доступные расширения: .jpg, .jpeg, .png, .gif");
                                throw ex;
                            }
                            if (!Directory.Exists(Server.MapPath(serverPath))) { DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(serverPath)); }

                            double filesCount = Directory.EnumerateFiles(Server.MapPath(serverPath)).Count();
                            double newFilenameInt = Math.Ceiling(filesCount / 2) + 1;
                            string newFilename = newFilenameInt.ToString() + Path.GetExtension(photos.FileName);

                            while (System.IO.File.Exists(Server.MapPath(Path.Combine(serverPath, newFilename))))
                            {
                                newFilenameInt++;
                                newFilename = newFilenameInt.ToString() + Path.GetExtension(photos.FileName);
                            }
                            //сохраняем оригинал
                            photos.SaveAs(Server.MapPath(Path.Combine(serverPath, newFilename)));
                            //сохраняем превью
                            Bitmap _File = (Bitmap)Bitmap.FromStream(photos.InputStream);
                            _File = Imaging.Resize(_File, 120, 120, "center", "center");
                            _File.Save(Server.MapPath(serverPath + "/prev_" + newFilename));

                            photoList[counter] = new PhotosModel()
                            {
                                Id = Guid.NewGuid(),
                                Album_Id = id,
                                Title = Path.GetFileName(photos.FileName),
                                Date = DateTime.Now,
                                Preview = serverPath + "/prev_" + newFilename,
                                Photo = serverPath + "/" + newFilename
                            };
                            counter++;

                            //записываем обложку фотоальбома
                            model.Item.Preview = photoList[0].Preview;

                            ViewBag.Message = "Запись сохранена";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                            break;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Фотоальбом должен содержать хотя бы одну фотографию.");
                        break;
                    }
                }
                #endregion

                if (ModelState.IsValid)
                {
                    model.Item.SiteId = Domain;

                    _repository.insertPhotoAlbums(id, model.Item);
                    _repository.insertPhotos(id, photoList); //photos
                    _repository.insertLog(id, AccountInfo.id, "insert", RequestUserInfo.IP);

                    ViewBag.Message = "Запись добавлена";
                    ViewBag.backurl = id;
                }
            }

            model = new PhotoAlbumsViewModel()
            {
                Item = _repository.getPhotoAlbums(id, Domain),
                PhotoList = _repository.getPhotos(id), //photos

                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo
            };

            if (model.Item != null)
            {
                ViewBag.Date = model.Item.Date.ToString("yyyy-MM-dd");
            }
            else ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");

            return View("Item", model);
        }               

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhotoAlbumsViewModel model = new PhotoAlbumsViewModel()
            {
                Item = _repository.getPhotoAlbums(id, Domain),
                PhotoList = _repository.getPhotos(id), //photos

                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo
            };
            ViewBag.Date = Convert.ToDateTime(model.Item.Date).ToString("yyyy-MM-dd");
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "update-btn")]
        public ActionResult Edit(Guid id, PhotoAlbumsViewModel model, IEnumerable<HttpPostedFileBase> uploadPhoto)
        {
            ViewBag.BtnName = "Сохранить";

            if (ModelState.IsValid)
            {
                #region photos
                int counter = 0;
                string serverPath = "";

                PhotoAlbumsViewModel photo = new PhotoAlbumsViewModel()
                {
                    PhotoList = _repository.getPhotos(id)
                };

                if (photo.PhotoList.Count() > 0)
                {
                    string filePath = photo.PhotoList.First().Photo.ToString();
                    serverPath = filePath.Remove(filePath.LastIndexOf("/"));
                }
                else
                {
                    savePath += DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM_dd") + "/";
                    serverPath = savePath + id.ToString();
                }

                PhotosModel[] photoList = new PhotosModel[uploadPhoto.Count()];

                foreach(HttpPostedFileBase photos in uploadPhoto)
                {
                    if (photos != null && photos.ContentLength > 0)
                        try
                        {
                            if(!allowedExtensions.Contains(Path.GetExtension(photos.FileName)))
                            {
                                Exception ex = new Exception("неверный формат файла \""+ Path.GetFileName(photos.FileName) + "\". Доступные расширения: .jpg, .jpeg, .png, .gif");
                                throw ex;
                            }
                            if (!Directory.Exists(Server.MapPath(serverPath))) { DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(serverPath)); }

                            double filesCount = Directory.EnumerateFiles(Server.MapPath(serverPath)).Count();
                            double newFilenameInt = Math.Ceiling(filesCount / 2) + 1;
                            string newFilename = newFilenameInt.ToString() + Path.GetExtension(photos.FileName);

                            while(System.IO.File.Exists(Server.MapPath(Path.Combine(serverPath, newFilename))))
                            {
                                newFilenameInt++;
                                newFilename = newFilenameInt.ToString() + Path.GetExtension(photos.FileName);
                            }
                            //сохраняем оригинал
                            photos.SaveAs(Server.MapPath(Path.Combine(serverPath, newFilename)));

                            //сохраняем превью
                            Bitmap _File = (Bitmap)Bitmap.FromStream(photos.InputStream);
                            _File = Imaging.Resize(_File, 120, 120, "center", "center");
                            _File.Save(Server.MapPath(serverPath + "/prev_" + newFilename));

                            photoList[counter] = new PhotosModel()
                            {
                                Id = Guid.NewGuid(),
                                Album_Id = id,
                                Title = Path.GetFileName(photos.FileName),
                                Date = DateTime.Now, 
                                Preview = serverPath + "/prev_" + newFilename,
                                Photo = serverPath + "/" + newFilename
                            };
                            counter++;

                            ViewBag.Message = "Запись сохранена";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                            break;
                        }
                }
                #endregion

                model.Item.SiteId = Domain;

                _repository.updatePhotoAlbums(id, model.Item);
                _repository.insertPhotos(id, photoList); //photos
                _repository.insertLog(id, AccountInfo.id, "update", RequestUserInfo.IP);

                ViewBag.backurl = id;                    
            }            
            model = new PhotoAlbumsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo,

                Item = _repository.getPhotoAlbums(id, Domain),
                PhotoList = _repository.getPhotos(id), //photos
            };
            ViewBag.Date = Convert.ToDateTime(model.Item.Date).ToString("yyyy-MM-dd");
            return View("Item", model);
        }


        // Удаление фотоальбома вместе с фотографиями
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ViewBag.Message = "Запись удалена";
            ViewBag.backurl = id;
            PhotoAlbumsViewModel model = new PhotoAlbumsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                Menu = MenuInfo,
                UserResolution = UserResolutionInfo,

                PhotoList = _repository.getPhotos(id)
            };
                        
            if(model.PhotoList.Count() > 0)
            {
                string filePath = model.PhotoList.First().Photo.ToString();
                string directoryPath = filePath.Remove(filePath.LastIndexOf("/") + 1);

                try
                {
                    Directory.Delete(Server.MapPath(directoryPath), true);
                }
                catch (IOException)
                {
                    Thread.Sleep(0);
                    Directory.Delete(Server.MapPath(directoryPath), true);
                }
            }

            _repository.deletePhotos(id); //photos
            _repository.deletePhotoAlbums(id, Domain);
            _repository.insertLog(id, AccountInfo.id, "delete", RequestUserInfo.IP);
            return View("Item", model);
        }


        // Удаление одной фотографии из фотоальбома
        [HttpPost]        
        public string DeletePhoto(string id)
        {
            PhotosModel photo = _repository.getPhoto(Guid.Parse(id));

            string photoPath = photo.Photo.ToString();
            string previewPath = photo.Preview.ToString();

            try
            {
                if (System.IO.File.Exists(Server.MapPath(photoPath)))
                {
                    System.IO.File.Delete(Server.MapPath(photoPath));
                    System.IO.File.Delete(Server.MapPath(previewPath));
                }
                else return "Не удалось удалить фотографию.";
            }
            catch (IOException)
            {
                Thread.Sleep(0);
                if (System.IO.File.Exists(Server.MapPath(photoPath)))
                {
                    System.IO.File.Delete(Server.MapPath(photoPath));
                    System.IO.File.Delete(Server.MapPath(previewPath));
                }
                else return "Не удалось удалить фотографию.";
            }

            return (_repository.deletePhoto(Guid.Parse(id))) ? "" : "Не удалось удалить фотографию.";
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            return RedirectToAction("", ViewBag.ControllerName);
        }
    }
}