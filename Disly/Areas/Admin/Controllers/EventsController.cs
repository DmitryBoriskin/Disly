using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
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

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
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

            if (AccountInfo != null)
            {
                model.Menu = _cmsRepository.getCmsMenu(AccountInfo.Id);
            }

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
            var evfilter = FilterParams.Extend<EventFilter>(filter);
            model.List = _cmsRepository.getEventsList(evfilter);

            return View(model);
        }

        /// <summary>
        /// GET: Форма редактирования/добавления события
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getEvent(Id);

            ViewBag.DataPath = Settings.UserFiles + Domain + Settings.EventsDir;
            ViewBag.DataPath = (model.Item == null) ?
                ViewBag.DataPath + DateTime.Today.ToString("yyyy") + "/" + DateTime.Today.ToString("MM") + "/" + DateTime.Today.ToString("dd") + "/"
                :
                ViewBag.DataPath + model.Item.DateBegin.ToString("yyyy") + "/" + model.Item.DateBegin.ToString("MM") + "/" + model.Item.DateBegin.ToString("dd") + "/";

            if (model.Item == null)
                model.Item = new EventsModel()
                {
                    Id = Id,
                    DateBegin = DateTime.Now
                };
            if (model.Item != null && model.Item.PreviewImage != null && !string.IsNullOrEmpty(model.Item.PreviewImage.Url))
            {
                var photo = model.Item.PreviewImage;
                model.Item.PreviewImage = Files.getInfoImage(photo.Url);
                model.Item.PreviewImage.Source = photo.Source;
            }

            //Заполняем для модели связи с другими объектами
            var eventFilter = FilterParams.Extend<EventFilter>(filter);
            eventFilter.RelId = Id;
            eventFilter.RelType = ContentType.EVENT;
            var eventsList = _cmsRepository.getEventsList(eventFilter);

            var orgfilter = FilterParams.Extend<OrgFilter>(filter);
            orgfilter.RelId = Id;
            orgfilter.RelType = ContentType.EVENT;
            var orgs = _cmsRepository.getOrgs(orgfilter);

            model.Item.Links = new ObjectLinks()
            {
                Events = (eventsList != null) ? eventsList.Data : null,
                Orgs = orgs,
                //Persons = null
            };

            ViewBag.Backlink = StartUrl + Request.Url.Query;

            return View("Item", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="disabled"></param>
        /// <param name="size"></param>
        /// <param name="date"></param>
        /// <param name="dateend"></param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool disabled, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "searchtext", searchtext);
            query = AddFiltrParam(query, "disabled", disabled.ToString().ToLower());
            query = (date.HasValue) ? AddFiltrParam(query, "date", date.Value.ToString("dd.MM.yyyy").ToLower()) : null;
            query = (dateend.HasValue) ? AddFiltrParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower()) : null;
            query = AddFiltrParam(query, "page", String.Empty);
            query = AddFiltrParam(query, "size", size);

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
            query = AddFiltrParam(query, "page", String.Empty);

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
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                var res = false;
                var getEvent = _cmsRepository.getEvent(Id);

                bindData.Item.Id = Id;

                #region Сохранение изображения
                var width = 0;
                var height = 0;
                var defaultPreviewSizes = new string[] { "540", "360" };

                // путь для сохранения изображения //Preview image
                string savePath = Settings.UserFiles + Domain + Settings.EventsDir;
                if (upload != null && upload.ContentLength > 0)
                {
                    if (!AttachedPicExtAllowed(upload.FileName))
                    {
                        model.Item = (_cmsRepository.getEvent(Id) != null) ? _cmsRepository.getEvent(Id)
                           : new EventsModel() { Id = Id };

                        model.ErrorInfo = new ErrorMessage()
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

                    string fileExtension = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).ToLower();

                    var sizes = (!string.IsNullOrEmpty(Settings.MaterialPreviewImgSize)) ? Settings.MaterialPreviewImgSize.Split(',') : defaultPreviewSizes;
                    int.TryParse(sizes[0], out width);
                    int.TryParse(sizes[1], out height);
                    bindData.Item.PreviewImage = new Photo()
                    {
                        Name = Id.ToString() + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, Id.ToString(), width, height),
                        Source = bindData.Item.PreviewImage.Source
                    };
                }
                #endregion

                if (String.IsNullOrEmpty(bindData.Item.Alias))
                {
                    bindData.Item.Alias = Transliteration.Translit(bindData.Item.Title);
                }
                else
                {
                    bindData.Item.Alias = Transliteration.Translit(bindData.Item.Alias);
                }

                //Определяем Insert или Update
                if (getEvent != null)
                {
                    userMessage.info = "Запись обновлена";
                    res = _cmsRepository.updateCmsEvent(bindData.Item);
                }
                else
                {
                    userMessage.info = "Запись добавлена";
                    bindData.Item.ContentLink = SiteInfo.ContentId;
                    bindData.Item.ContentLinkType = SiteInfo.Type;
                    res = _cmsRepository.insertCmsEvent(bindData.Item);
                }
                //Сообщение пользователю
                if (res)
                {
                    string currentUrl = Request.Url.PathAndQuery;
                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                         new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                         new ErrorMassegeBtn { url = currentUrl, text = "ок"}
                    };
                }
                else
                {
                    userMessage.info = "Произошла ошибка";
                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                         new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                         new ErrorMassegeBtn {  url = "#", text = "ок", action = "false"}
                    };
                }
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
                var photo = model.Item.PreviewImage;
                model.Item.PreviewImage = Files.getInfoImage(photo.Url);
                model.Item.PreviewImage.Source = photo.Source;
            }

            if (model.Item != null && model.Item.PreviewImage != null && !string.IsNullOrEmpty(model.Item.PreviewImage.Url))
            {
                model.Item.PreviewImage = Files.getInfoImage(model.Item.PreviewImage.Url);
            }
            model.ErrorInfo = userMessage;

            return View("Item", model);
        }

        //Удалить!!!
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
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";
            //В случае ошибки
            userMessage.info = "Ошибка, Запись не удалена";
            userMessage.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.Item = _cmsRepository.getEvent(Id);
            if (model.Item != null)
            {
                var image = (model.Item.PreviewImage != null) ? model.Item.PreviewImage.Url : null;
                var res = _cmsRepository.deleteCmsEvent(Id);
               
                if (res)
                {
                    if (!string.IsNullOrEmpty(image))
                        Files.deleteImage(image);

                    // записываем информацию о результатах
                    userMessage.title = "Информация";
                    userMessage.info = "Запись удалена";

                    userMessage.buttons = new ErrorMassegeBtn[]
                    {
                        new ErrorMassegeBtn { url = StartUrl, text = "ок" }
                    };
                }
            }

            model.ErrorInfo = userMessage;

            return View(model);
        }

        //Получение списка организаций по параметрам для отображения в модальном окне
        [HttpGet]
        public ActionResult EventsListModal(Guid objId, ContentType objType)
        {
            var filtr = new EventFilter()
            {
                Domain = Domain,
                RelId = objId,
                RelType = objType,
                Size = 100
            };

            var model = new EventsModalViewModel()
            {
                ObjctId = objId,
                ObjctType = objType,
                EventsList = _cmsRepository.getLastEventsListWithCheckedFor(filtr),
            };

            //var model = new OrgsModalViewModel()
            //{

            //    OrgsList = _cmsRepository.getOrgs(filtr),
            //    OrgsAll = _cmsRepository.getOrgs(new OrgFilter() { }),
            //    OrgsTypes = _cmsRepository.getOrgTypesList(new OrgTypeFilter() { })
            //};

            return PartialView("Modal/Events", model);
        }

        [HttpPost]
        public ActionResult UpdateLinkToEvent(ContentLinkModel data)
        {
            if (data != null)
            {
                var res = _cmsRepository.updateContentLink(data);
                if (res)
                    return Json("Success");
            }

            //return Response.Status = "OK";
            return Json("An Error Has occourred"); //Ne
        }
    }
}