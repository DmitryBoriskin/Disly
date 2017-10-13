using System.Web.Mvc;
using Disly.Areas.Admin.Models;
using System;
using System.Web;
using cms.dbModel.entity;
using System.IO;

namespace Disly.Areas.Admin.Controllers
{
    public class BannersController : CoreController
    {
        // Модель для вывода в представления
        private BannersViewModel model;

        // Фильтр
        private FilterParams filter;

        // Кол-во эл-тов на странице 100
        int pageSize = 100;

        /// <summary>
        /// Обрабатывается до вызова экшенов
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            model = new BannersViewModel
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
            ViewBag.Keywords = "";
            #endregion
        }

        /// <summary>
        /// Список секций для баннеров
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(Guid? id)
        {
            if (id == null)
            {
                // Наполняем модель списком секций
                model.Sections = _cmsRepository.getBannerSections(Domain);
            }
            else
            {
                // наполняем фильтр
                filter = getFilter(pageSize);

                // наполняем модель списка баннеров
                model.SectionItem = _cmsRepository.getBannerSection((Guid)id, Domain, filter);
            }
            
            return View(model);
        }

        /// <summary>
        /// Конкретный баннер
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.getBanner(id);

            // файл изображения
            if (model.Item != null)
            {
                var photo = model.Item.Photo;
                if (!string.IsNullOrEmpty(photo.Url))
                {
                    model.Item.Photo = getInfoPhoto(photo.Url);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Создание или редактирование баннера
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, BannersViewModel back_model, HttpPostedFileBase upload)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            #region Данные, необходимые для сохранения
            back_model.Item.Section = back_model.Item.Section != null ? back_model.Item.Section : Guid.Parse(Request.Form["Item_Section"]);
            back_model.Item.Site = Domain;
            #endregion

            if (ModelState.IsValid)
            {
                #region Сохранение изображения
                // путь для сохранения изображения
                string savePath = Settings.UserFiles + Domain + Settings.BannersDir;

                // секция
                var _section = _cmsRepository.getBannerSection((Guid)back_model.Item.Section, Domain, null);
                int width = _section.Width; // ширина
                int height = _section.Height; // высота

                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.IndexOf("."));

                    Photo photoNew = new Photo()
                    {
                        Name = id.ToString() + fileExtension,
                        Size = Files.FileAnliz.SizeFromUpload(upload),
                        Url = Files.SaveImageResizeRename(upload, savePath, id.ToString(), width, height)
                    };

                    back_model.Item.Photo = photoNew;
                }
                #endregion

                if (!_cmsRepository.checkBannerExist(id))
                {
                    _cmsRepository.createBanner(id, back_model.Item, AccountInfo.id, RequestUserInfo.IP);
                    userMessage.info = "Запись добавлена";
                }
                else
                {
                    _cmsRepository.updateBanner(id, back_model.Item, AccountInfo.id, RequestUserInfo.IP);
                    userMessage.info = "Запись обновлена";
                }

                string backUrl = back_model.Item.Section != null ? "index/" + back_model.Item.Section : string.Empty;

                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + backUrl, text = "Вернуться в список" },
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

            model.Item = _cmsRepository.getBanner(id);

            var photo = model.Item.Photo;
            if (!string.IsNullOrEmpty(photo.Url))
            {
                model.Item.Photo = getInfoPhoto(photo.Url);
            }

            return View(model);
        }

        /// <summary>
        /// Событие по кнопке "Отмена"
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <param name="section">Id-секции</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id, Guid? section)
        {
            model.Item = _cmsRepository.getBanner(id);
            var _section = model.Item != null ? model.Item.Section : Guid.Parse(Request.Form["Item_Section"]);
            //var _section = (section.Equals(null) && model.Item != null) ? model.Item.Section : section;

            if (_section != null)
            {
                return RedirectToAction("Index", new { id = _section });
            }
            return Redirect(StartUrl + Request.Url.Query);
        }
        
        /// <summary>
        /// Событие по кнопке "Удалить"
        /// </summary>
        /// <param name="id">id-баннера</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            model.Item = _cmsRepository.getBanner(id);

            _cmsRepository.deleteBanner(id, AccountInfo.id, RequestUserInfo.IP);

            // удаляем файл изображения
            if (System.IO.File.Exists(Server.MapPath(model.Item.Photo.Url)))
                System.IO.File.Delete(Server.MapPath(model.Item.Photo.Url));

            // записываем информацию о результатах
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;

            return RedirectToAction("Index", new { id = model.Item.Section });
        }
    }
}
