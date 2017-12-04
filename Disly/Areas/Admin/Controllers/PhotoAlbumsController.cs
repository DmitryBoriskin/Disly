using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class PhotoAlbumsController : CoreController
    {
        PhotoViewModel model;
        FilterParams filter;
        string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".gif", ".png" };

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new PhotoViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            //Справочник всех доступных категорий
            MaterialsGroup[] GroupsValues = _cmsRepository.getAllMaterialGroups();
            ViewBag.AllGroups = GroupsValues;

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Materials
        public ActionResult Index()
        {            
            model.List = _cmsRepository.getPhotoAlbum(filter);
            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid id)
        {
            model.Album = _cmsRepository.getPhotoAlbumItem(id);
            if (model.Album == null)
            {
                model.Album = new PhotoAlbum()
                {
                    Id = id,
                    Date = DateTime.Now
                };
            }
            return View("Item", model);
        }

        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool disabled, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "disabled", disabled.ToString().ToLower());
            query = (date == null) ? addFiltrParam(query, "date", String.Empty) : addFiltrParam(query, "date", ((DateTime)date).ToString("dd.MM.yyyy").ToLower());
            query = (dateend == null) ? addFiltrParam(query, "dateend", String.Empty) : addFiltrParam(query, "dateend", ((DateTime)dateend).ToString("dd.MM.yyyy").ToString().ToLower());
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        /// <summary>
        /// Очищаем фильтр
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            //  При создании записи сбрасываем номер страницы
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "page", String.Empty);

            return Redirect(StartUrl + "Item/" + Guid.NewGuid() + "/" + query);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, PhotoViewModel bindData, IEnumerable<HttpPostedFileBase> uploadPhoto, HttpPostedFileBase upload)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                #region photos
                string savePath = Settings.UserFiles + Domain  + Settings.PhotoDir + bindData.Album.Date.ToString("yyyy") + "_" + bindData.Album.Date.ToString("MM") + "/" + bindData.Album.Date.ToString("dd") + "/" + id;
                int counter = 0;
                string serverPath = savePath;
                string PreviewAlbum = String.Empty; ;

                PhotoModel[] photoList = new PhotoModel[uploadPhoto.Count()];

                foreach (HttpPostedFileBase photos in uploadPhoto)
                {
                    if (photos != null && photos.ContentLength > 0)
                    {
                        //try
                        //{
                            if (!allowedExtensions.Contains(Path.GetExtension(photos.FileName).ToLower()))
                            {
                                Exception ex = new Exception("неверный формат файла \"" + Path.GetFileName(photos.FileName) + "\". Доступные расширения: .jpg, .jpeg, .png, .gif");
                                throw ex;
                            }
                            if (!Directory.Exists(Server.MapPath(serverPath))) { DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(serverPath)); }

                            double filesCount = Directory.EnumerateFiles(Server.MapPath(serverPath)).Count();
                            double newFilenameInt = Math.Ceiling(filesCount / 2) + 1;
                            string newFilename = newFilenameInt.ToString() + ".jpg";

                            while (System.IO.File.Exists(Server.MapPath(Path.Combine(serverPath, newFilename))))
                            {
                                newFilenameInt++;
                                newFilename = newFilenameInt.ToString() + ".jpg";
                            }

                            //сохраняем оригинал
                            photos.SaveAs(Server.MapPath(Path.Combine(serverPath, newFilename)));

                            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 70L); //cжатие 90


                            Bitmap _File = (Bitmap)Bitmap.FromStream(photos.InputStream);
                            //"оригинал"
                            Bitmap _FileOrigin = Imaging.Resize(_File, 4000, "width");
                            _FileOrigin.Save(Server.MapPath(serverPath + "/" + newFilename), myImageCodecInfo, myEncoderParameters);

                            //сохраняем full hd
                            Bitmap _FileHd = Imaging.Resize(_File, 2000, "width");
                            _FileHd.Save(Server.MapPath(serverPath + "/hd_" + newFilename), myImageCodecInfo, myEncoderParameters);

                            //сохраняем превью
                            Bitmap _FilePrev = Imaging.Resize(_File, 120, 120, "center", "center");
                            _FilePrev.Save(Server.MapPath(serverPath + "/prev_" + newFilename), myImageCodecInfo, myEncoderParameters);

                            photoList[counter] = new PhotoModel()
                            {
                                Id = Guid.NewGuid(),
                                AlbumId = id,
                                Title = Path.GetFileName(photos.FileName),
                                Date = DateTime.Now,
                                PreviewImage = new Photo{Url= serverPath + "/prev_" + newFilename},
                                PhotoImage = new Photo { Url = serverPath + "/" + newFilename }                                
                            };
                        if (counter == 1)
                        {
                            //записываем обложку фотоальбома
                            PreviewAlbum = photoList[0].PreviewImage.Url;
                        }                        
                            counter++;                            
                            ViewBag.Message = "Запись сохранена";
                        //}
                        //catch (Exception ex)
                        //{
                        //    ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                        //    break;
                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("", "Фотоальбом должен содержать хотя бы одну фотографию.");
                        break;
                    }
                }
                #endregion


                var getAlbum = _cmsRepository.getPhotoAlbumItem(id);                    
                bindData.Album.Id = id;                
                bindData.Album.PreviewImage = new Photo { Url = PreviewAlbum };
                bindData.Album.Path = savePath;

                //Определяем Insert или Update
                if (getAlbum != null)
                    if (_cmsRepository.updPhotoAlbum(id, bindData.Album))
                    {
                        userMessage.info = "Запись обновлена";
                    }
                    else
                    {
                        userMessage.info = "Произошла ошибка";
                    }
                else
                {
                    _cmsRepository.insPhotoAlbum(id, bindData.Album);
                        userMessage.info = "Запись добавлена";
                }
                _cmsRepository.insertPhotos(id, photoList);                


                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "/Admin/photoalbums/item/"+id, text = "ок", action = "false" }
                 };
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            }

            model.Album = _cmsRepository.getPhotoAlbumItem(id);
            if (model.Album != null && model.Album.PreviewImage != null && !string.IsNullOrEmpty(model.Album.PreviewImage.Url))
            {
                model.Album.PreviewImage = Files.getInfoImage(model.Album.PreviewImage.Url);
            }
            model.ErrorInfo = userMessage;

            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {
            // записываем информацию о результатах
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            var data = _cmsRepository.getPhotoAlbumItem(Id);
            if (_cmsRepository.delPhotoAlbum(Id))
            {
                try
                {
                    Directory.Delete(Server.MapPath(data.Path));
                }
                catch (IOException)
                {
                    Thread.Sleep(0);
                    Directory.Delete(Server.MapPath(data.Path), true);
                }
                userMassege.info = "Запись Удалена";
            }
            else {
                userMassege.info = "Произошла ошибка";
            };
            

            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;
            return RedirectToAction("Index");
        }

        

    }
}