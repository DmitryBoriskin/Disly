using Disly.Areas.Admin.Models;
using System;
using System.Configuration;
using System.Web.Mvc;
using Disly.Areas.Admin.Service;

namespace Disly.Areas.Admin.Controllers
{
    public class SiteMapController : CoreController
    {
        // Модель для вывода в представление
        private SiteMapViewModel model;

        // Фильтр
        private FilterParams filter;

        // Кол-во элементов на странице
        int pageSize = 100;
        
        /// <summary>
        /// Обрабатывается до вызыва экшена
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            model = new SiteMapViewModel
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                FrontSectionList = _cmsRepository.getSiteMapFrontSectionList(),
                MenuTypes = _cmsRepository.getSiteMapMenuTypes()
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Список эл-тов карты сайта
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Наполняем фильтр значениями
            filter = getFilter(pageSize);

            // Наполняем модель списка данными
            model.List = _cmsRepository.getSiteMapList(Domain, filter);

            ViewBag.Group = filter.Group;

            return View(model);
        }
        
        /// <summary>
        /// Едичная запись эл-та карты сайта
        /// </summary>
        /// <param name="id">Идентификатор карты сайта</param>
        /// <param name="parent">Родительский идентификатор</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Item(Guid id, Guid? parent)
        {
            // текущий элемент карты сайта
            model.Item = _cmsRepository.getSiteMapItem(id);
            var m = new MultiSelectList(model.MenuTypes, "value", "text", model.Item != null ?  model.Item.MenuGroups : null);
            ViewBag.GroupMenu = m;

            if (model.Item != null)
                model.Item.MenuGroups = null;

            Guid? _parent = (parent.Equals(null) && model.Item != null) ? model.Item.ParentId : parent;

            // хлебные крошки
            model.BreadCrumbs = _cmsRepository.getSiteMapBreadCrumbs(_parent);

            // список дочерних элементов
            model.Childrens = _cmsRepository.getSiteMapChildrens(id);

            return View(model);
        }

        /// <summary>
        /// POST-обработка эл-та карты сайта
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="parent">Родительский идентификатор</param>
        /// <param name="back_model">Возвращаемая модель</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, Guid? parent, SiteMapViewModel back_model)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            #region Данные необходимые для сохранения
            back_model.Item.ParentId = back_model.Item.ParentId != null ? back_model.Item.ParentId : parent; // родительский id

            var p = back_model.Item.ParentId != null ? _cmsRepository.getSiteMapItem((Guid)back_model.Item.ParentId) : null;

            back_model.Item.Path = p == null ? "/" :
                p.Path.Equals("/") ? p.Path + p.Alias : p.Path + "/" + p.Alias;

            back_model.Item.Site = Domain;

            if (String.IsNullOrEmpty(back_model.Item.Alias))
            {
                back_model.Item.Alias = Transliteration.Translit(back_model.Item.Title);
            }
            else
            {
                back_model.Item.Alias = Transliteration.Translit(back_model.Item.Alias);
            }

            // хлебные крошки
            model.BreadCrumbs = _cmsRepository.getSiteMapBreadCrumbs(parent);

            // список дочерних элементов
            model.Childrens = _cmsRepository.getSiteMapChildrens(id);
            #endregion

            if (ModelState.IsValid)
            {
                if (_cmsRepository.checkSiteMap(id))
                {
                    _cmsRepository.updateSiteMapItem(id, back_model.Item, AccountInfo.id, RequestUserInfo.IP);
                    userMessage.info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.createSiteMapItem(id, back_model.Item, AccountInfo.id, RequestUserInfo.IP);
                    userMessage.info = "Запись добавлена";
                }

                string backUrl = back_model.Item.ParentId != null ? "item/" + back_model.Item.ParentId : string.Empty;

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

            model.Item = _cmsRepository.getSiteMapItem(id);

            var m = new MultiSelectList(model.MenuTypes, "value", "text", model.Item.MenuGroups);
            ViewBag.GroupMenu = m;
            model.Item.MenuGroups = null;

            model.ErrorInfo = userMessage;

            return View(model);
        }
        
        /// <summary>
        /// Обработчика кнопки "Назад"
        /// </summary>
        /// <param name="id">Идентификатор эл-та карты сайта</param>
        /// <param name="parent">Родительский идентификатор</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id, Guid? parent)
        {
            var p = _cmsRepository.getSiteMapItem(id);

            // получим родительский элемент для перехода
            var _parent = (parent.Equals(null) && p != null) ? p.ParentId : parent;

            if (_parent != null)
            {
                return RedirectToAction("Item", new { id = _parent });
            }
            return Redirect(StartUrl + Request.Url.Query);
        }

        /// <summary>
        /// Обработчик события кнопки "Удалить"
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            model.Item = _cmsRepository.getSiteMapItem(id);

            _cmsRepository.deleteSiteMapItem(id, AccountInfo.id, RequestUserInfo.IP);

            // записываем информацию о результатах
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };
            
            model.ErrorInfo = userMassege;

            if (model.Item.ParentId.Equals(null))
            {
                return Redirect(StartUrl + Request.Url.Query);
            }
            else
            {
                return RedirectToAction("Item", new { id = model.Item.ParentId });
            }
        }
    }
}