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
        int pageSize = 40;

        // Доменное имя сайта
        private string domain;

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
                FrontSectionList = _cmsRepository.getSiteMapFrontSectionList(),
                MenuTypes = _cmsRepository.getSiteMapMenuTypes()
            };

            string _domain = HttpContext.Request.Url.Host.ToString().ToLower().Replace("www.", "").Replace("new.", "");

            try { domain = _cmsRepository.getSiteId(_domain); }
            catch
            {
                if (_domain != ConfigurationManager.AppSettings["BaseURL"]) filterContext.Result = Redirect("/Error/");
                else domain = String.Empty;
            }

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: SiteMap
        [HttpGet]
        public ActionResult Index()
        {
            // Наполняем фильтр значениями
            filter = getFilter(pageSize);

            // Наполняем модель списка данными
            model.List = _cmsRepository.getSiteMapList(domain, filter);

            return View(model);
        }
        
        [HttpGet]
        public ActionResult Item(Guid id, Guid? parent)
        {
            // текущий элемент карты сайта
            model.Item = _cmsRepository.getSiteMapItem(id);

            var m = new MultiSelectList(model.MenuTypes, "value", "text", model.Item.MenuGroups);
            ViewBag.GroupMenu = m;

            model.Item.MenuGroups = null;

            Guid? _parent = (parent.Equals(null) && model.Item != null) ? model.Item.ParentId : parent;

            // хлебные крошки
            model.BreadCrumbs = _cmsRepository.getSiteMapBreadCrumbs(_parent);

            // список дочерних элементов
            model.Childrens = _cmsRepository.getSiteMapChildrens(id);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, Guid? parent, SiteMapViewModel back_model)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            #region Данные необходимые для сохранения
            back_model.Item.ParentId = parent; // родительский id

            back_model.Item.Path = back_model.Item.ParentId.Equals(null) ? "/"
                : _cmsRepository.getSiteMapItem((Guid)back_model.Item.ParentId).Path + "/" + _cmsRepository.getSiteMapItem((Guid)back_model.Item.ParentId).Alias;

            back_model.Item.Site = domain;

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

            model.Item = _cmsRepository.getSiteMapItem(id);

            var m = new MultiSelectList(model.MenuTypes, "value", "text", model.Item.MenuGroups);
            ViewBag.GroupMenu = m;
            model.Item.MenuGroups = null;

            model.ErrorInfo = userMessage;

            return View(model);
        }
        
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