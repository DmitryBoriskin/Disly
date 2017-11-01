using cms.dbModel.entity;
 using Disly.Areas.Admin.Models;
 using System;
 using System.Collections.Generic;
 using System.Configuration;
 using System.IO;
 using System.Linq;
 using System.Net;
 using System.Web;
 using System.Web.Mvc;
 
 namespace Disly.Areas.Admin.Controllers
 {
     public class EventsController : CoreController
     {
         EventsViewModel model;
         FilterParams filter;
         
         protected override void OnActionExecuting(ActionExecutingContext filterContext)
         {
             base.OnActionExecuting(filterContext);
 
             ViewBag.ControllerName = (String) RouteData.Values["controller"];
             ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();
 
             ViewBag.HttpKeys = Request.QueryString.AllKeys;
             ViewBag.Query = Request.QueryString;
 
             filter = getFilter();
 
              model = new EventsViewModel()
             {
                 Account = AccountInfo,
                 Settings = SettingsInfo,
                 UserResolution = UserResolutionInfo,
                 ControllerName = ControllerName,
                 ActionName = ActionName
              };
 
             #region Метатеги
             ViewBag.Title = UserResolutionInfo.Title;
             ViewBag.Description = "";
             ViewBag.KeyWords = "";
             #endregion
         }
 
 
         /// <summary>
         /// GET: Список событий
         /// </summary>
         /// <param name="category"></param>
         /// <param name="type"></param>
         /// <returns></returns>
         public ActionResult Index(string category, string type)
         {
             // Наполняем модель данными
             model.List = _cmsRepository.getEventsList(filter);
 
             return View(model);
         }
 
         /// <summary>
         /// GET: Форма редактирования/добавления события
         /// </summary>
         /// <returns></returns>
         public ActionResult Item(Guid Id)
         {
             model.Item = _cmsRepository.getEvent(Id);
            if (model.Item == null)
                model.Item = new EventModel()
                {
                    DateBegin = DateTime.Now
                };
            if (model.Item != null)
            {
                var photo = model.Item.PreviewImage;
                if (!string.IsNullOrEmpty(photo.Url))
                {
                    model.Item.PreviewImage = getInfoPhoto(photo.Url);
                }
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
         public ActionResult Search(string filter, bool enabeld, string size)
         {
             string query = HttpUtility.UrlDecode(Request.Url.Query);
             query = addFiltrParam(query, "filter", filter);
             if (enabeld) query = addFiltrParam(query, "enabeld", String.Empty);
             else query = addFiltrParam(query, "enabeld", enabeld.ToString().ToLower());
 
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
         
         /// <summary>
         /// Создание новой записи
         /// </summary>
         /// <returns></returns>
         [HttpPost]
         [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
         public ActionResult Insert()
         {
             //  При создании записи сбрасываем номер страницы
             string query = HttpUtility.UrlDecode(Request.Url.Query);
             query = addFiltrParam(query, "page", String.Empty);
 
             return Redirect(StartUrl + "Item/" + Guid.NewGuid() + "/" + query);
         }
 
         /// <summary>
         /// Сохранение изменений в записи
         /// </summary>
         /// <param name="Id"></param>
         /// <param name="bindData"></param>
         /// <returns></returns>
         [HttpPost]
         [ValidateInput(false)]
         [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
         public ActionResult Save(Guid Id, EventsViewModel bindData, HttpPostedFileBase upload)
         {
             ErrorMassege userMessage = new ErrorMassege();
             userMessage.title = "Информация";
 
             if (ModelState.IsValid)
             {
                 var res = false;
                 var getEvent = _cmsRepository.getEvent(Id);
 
                 bindData.Item.Id = Id;

                #region Сохранение изображения
                var width = 0;
                var height = 0;
                var defaultPreviewSizes  = new string[] { "540","360" };

                // путь для сохранения изображения //Preview image
                string savePath = Settings.UserFiles + Domain + Settings.EventsDir;
                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.IndexOf(".")).ToLower();

                    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                    if (!validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        model.Item = _cmsRepository.getEvent(Id);
                        model.ErrorInfo = new ErrorMassege()
                        {
                            title = "Ошибка",
                            info = "Вы не можете загружать файлы данного формата",
                            buttons = new ErrorMassegeBtn[]
                            {
                             new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                            }
                        };

                        return View("Item", model);
                    }

                    var sizes = (!string.IsNullOrEmpty(Settings.MaterialPreviewImgSize)) ? Settings.MaterialPreviewImgSize.Split(',') : defaultPreviewSizes;
                    int.TryParse(sizes[0], out width);
                    int.TryParse(sizes[1], out height);
                    bindData.Item.PreviewImage = new Photo() 
                    {
                        Name = Id.ToString() + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, Id.ToString(), width, height)
                    };
                }
                #endregion

                //Определяем Insert или Update
                if (getEvent != null)
                     res = _cmsRepository.updateCmsEvent(bindData.Item);
                 else
                     res = _cmsRepository.insertCmsEvent(bindData.Item);
                 //Сообщение пользователю
                 if (res)
                     userMessage.info = "Запись обновлена";
                 else
                     userMessage.info = "Произошла ошибка";
 
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

            model.Item = _cmsRepository.getEvent(Id);
            if (model.Item != null && model.Item.PreviewImage != null && !string.IsNullOrEmpty(model.Item.PreviewImage.Url))
            {
                model.Item.PreviewImage = getInfoPhoto(model.Item.PreviewImage.Url);
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
             //var res = _cmsRepository.deleteCmsEvent(Id);
 
             // записываем информацию о результатах
             ErrorMassege userMassege = new ErrorMassege();
             userMassege.title = "Информация";
             userMassege.info = "Запись Удалена";
             userMassege.buttons = new ErrorMassegeBtn[]{
                 new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
             };
 
             model.ErrorInfo = userMassege;
 
             return View("Item", model);
         }
     }
 }