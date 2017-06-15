using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class PortalUsersController : CoreController
    {
        PortalUsersViewModel model;
        //
        string filter =  String.Empty;
        string group =  String.Empty;
        bool enabeld = true;
        int page =  1;
        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            model = new PortalUsersViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                GroupList = _cmsRepository.getUsersGroupList()
            };

            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Страница по умолчанию (Список)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // 
            string return_url = ViewBag.urlQuery = HttpUtility.UrlDecode(Request.Url.Query);

            #region Получаем значения фильров из адресной строки
            // если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            return_url = (Convert.ToInt32(Request.QueryString["page"]) == page) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            // записываем в переменную значение "page" из URL
            page = (Convert.ToInt32(Request.QueryString["page"]) > 0) ? Convert.ToInt32(Request.QueryString["page"]) : page;
            // если в URL кол-во записей на странице равно значению по умолчанию - удаляем его из URL
            return_url = (Convert.ToInt32(Request.QueryString["size"]) == page_size) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            // записываем в переменную значение "size" из URL
            page_size = (Convert.ToInt32(Request.QueryString["size"]) > 0) ? Convert.ToInt32(Request.QueryString["size"]) : page_size;
            // записываем в переменную значение "filter" из URL
            filter = String.IsNullOrEmpty(Request.QueryString["filter"]) ? String.Empty : Request.QueryString["filter"];
            // записываем в переменную значение "group" из URL
            group = String.IsNullOrEmpty(Request.QueryString["group"]) ? String.Empty : Request.QueryString["group"];
            // записываем в переменную значение "enabeld" из URL
            enabeld = String.IsNullOrEmpty(Request.QueryString["enabeld"]);
            // разделяем значения из переменной "filter" по словам
            string[] SearchParams = filter.Split(' ');

            // Если парамметры из адресной строки равны значениям по умолчанию - удаляем их из URL
            if (return_url.ToLower() != HttpUtility.UrlDecode(Request.Url.Query).ToLower())
                return Redirect(StartUrl + return_url);
            #endregion

            // Наполняем модель данными
            model.List = _cmsRepository.getUsersList(SearchParams, group, enabeld, page, page_size);

            return View(model);
        }
                
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getUser(Id);            

            return View("Item", model);
        }

        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <param name="search-btn">Поиск по доменному имени</param>
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, PortalUsersViewModel back_model)
        {
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                if (_cmsRepository.check_user(Id))
                {
                    _cmsRepository.updateUser(Id, back_model.Item, AccountInfo.id, RequestUserInfo.IP);
                    userMassege.info = "Запись обновлена";
                }
                else if (!_cmsRepository.check_user(back_model.Item.EMail))
                {
                    char[] _pass = back_model.Password.Password.ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    back_model.Item.Salt = NewSalt;
                    back_model.Item.Hash = NewHash;

                    _cmsRepository.createUser(Id, back_model.Item, AccountInfo.id, RequestUserInfo.IP);

                    userMassege.info = "Запись добавлена";
                }
                else
                {
                    userMassege.info = "Пользователь с таким EMail адресом уже существует.";
                }

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            model.Item = _cmsRepository.getUser(Id);
            model.ErrorInfo = userMassege;

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
            _cmsRepository.deleteUser(Id, AccountInfo.id, RequestUserInfo.IP);

            // записываем информацию о результатах
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.Item = _cmsRepository.getUser(Id);
            model.ErrorInfo = userMassege;

            return View("Item", model);
        }
    }
}