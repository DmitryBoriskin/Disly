using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
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
        public ActionResult Index(string category, string type)
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
                model.Album = new PhotoAlbum();
                model.Album.Date = DateTime.Now;

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
        public ActionResult Save(Guid id, PhotoViewModel bindData, HttpPostedFileBase upload)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                
                var getAlbum = _cmsRepository.getPhotoAlbumItem(id);
                bindData.Album.Id = id;

                var status = false;//если trueдобавляем в фотоальбом фотографии иначе - не добавляем
                //Определяем Insert или Update
                if (getAlbum != null)
                    if (_cmsRepository.updPhotoAlbum(id, bindData.Album))
                    {
                        status = true;
                        userMessage.info = "Запись обновлена";
                    }
                    else
                    {userMessage.info = "Произошла ошибка";}
                else
                {
                    if (_cmsRepository.insPhotoAlbum(id, bindData.Album))
                    {
                        userMessage.info = "Запись добавлена";
                        status = true;
                    }
                    else{userMessage.info = "Произошла ошибка";}
                }
                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
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
        /// <summary>
        /// удаляем фотоальбом и входящие в него фотографии
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {

            // записываем информацию о результатах
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            var data = _cmsRepository.getPhotoAlbumItem(Id);
            if(data != null)
            {
                var delpath = data.Path;

                if (_cmsRepository.delPhotoAlbum(Id))
                {
                    userMassege.info = "Запись Удалена";
                    #region удаление файлов
                    try
                    {
                        try
                        {
                            Directory.Delete(Server.MapPath(delpath), true);
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(0);
                            Directory.Delete(Server.MapPath(delpath), true);
                        }
                    }
                    catch
                    {
                        //на случай когда в базе есть - а физически изображений не существует
                    } 
                    #endregion
                }
                else
                {
                    userMassege.info = "Произошла ошибка";
                }        
            }            
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };
            model.ErrorInfo = userMassege;
            //return RedirectToAction("Index");
            return View("Item", model);
        }

        

    }
}