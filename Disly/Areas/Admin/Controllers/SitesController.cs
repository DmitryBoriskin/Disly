using Disly.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class SitesController : CoreController
    {
        SitesViewModel model;
        // 
        string filter = String.Empty;
        bool enabeld = true;
        int page = 1;
        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SitesViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo
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
            //group = String.IsNullOrEmpty(Request.QueryString["group"]) ? String.Empty : Request.QueryString["group"];
            // записываем в переменную значение "enabeld" из URL
            enabeld = String.IsNullOrEmpty(Request.QueryString["enabeld"]);
            // разделяем значения из переменной "filter" по словам
            string[] SearchParams = filter.Split(' ');

            // Если парамметры из адресной строки равны значениям по умолчанию - удаляем их из URL
            if (return_url.ToLower() != HttpUtility.UrlDecode(Request.Url.Query).ToLower())
                return Redirect(StartUrl + return_url);
            #endregion

            // Наполняем модель данными
            model.List = _cmsRepository.getSiteList(SearchParams, page, page_size);

            return View(model);
        }
                
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getSite(Id);

            return View("Item", model);
        }
                
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Master()
        {
            model.Item = _cmsRepository.getSite(Guid.NewGuid());

            return View("Master", model);
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

            return Redirect(StartUrl + "Master/" + Guid.NewGuid() + "/" + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, SitesViewModel back_model)
        {
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
            //    //main_Sites
            //    _repository.createSite(id, input.Site);

            //    //main_SitesDomainList
            //    SitesDomainModel sitedomain = new SitesDomainModel()
            //    {
            //        Id = Guid.NewGuid(),
            //        SiteId = input.Site.C_Domain,
            //        Domain = input.Site.C_Domain + "." + Request.Url.Authority.Substring(0, Request.Url.Authority.IndexOf(":"))
            //    };
            //    _repository.createSiteDomain(sitedomain);

            //    //cms_UserSiteLink
            //    UserSiteLink usersitelink = new UserSiteLink()
            //    {
            //        SiteId = input.Site.C_Domain,
            //        UserId = input.User.Id
            //    };
            //    _repository.createUserSiteLink(usersitelink);

                if (_cmsRepository.check_cmsMenu(Id))
                {
                    //_repository.updateSite(id, input.Site, AccountInfo.id, RequestUserInfo.IP);
                    userMassege.info = "Запись обновлена";
                }
                else if (!_cmsRepository.check_cmsMenu(back_model.Item.Alias))
                {
                    //_cmsRepository.createSite(id, input.Site, AccountInfo.id, RequestUserInfo.IP);

                    userMassege.info = "Запись добавлена";
                }
                else
                {
                    userMassege.info = "Запись с таким псевдонимом уже существует. <br />Замените псевдоним.";
                }

               


                userMassege.info = "Запись с таким псевдонимом уже существует. <br />Замените псевдоним.";
                
                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                    new ErrorMassegeBtn { url = StartUrl + "item/" + Id + "/" + Request.Url.Query, text = "редактировать" }
                };
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы";
            }

            model.Item = _cmsRepository.getSite(Id);
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
            //_cmsRepository.deleteCmsMenu(Id, AccountInfo.id, RequestUserInfo.IP);

            // записываем информацию о результатах
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Ок" }
            };

            model.Item = _cmsRepository.getSite(Id);
            model.ErrorInfo = userMassege;

            return View("item", model);
        }



        /// <summary>
        /// Доменные имена
        /// </summary>
        /// <returns></returns>
        public ActionResult Domains(string siteId)
        {
            //SitesDomainModel newDomain = new SitesDomainModel()
            //{
            //    Id = Guid.NewGuid(),
            //    SiteId = siteId,
            //    Domain = ""
            //};

            SitesViewModel model = new SitesViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,

                //Domain = newDomain,                
                //Domains = _repository.getSiteDomains(siteId)
            };

            return View("Domains", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn-group")]
        public ActionResult Domains(string siteId, SitesViewModel model)
        {
            ////model.Domain.Id = Id;
            //model.Domain.SiteId = siteId;

            //if (ModelState.IsValid)
            //{
            //    _repository.createSiteDomain(model.Domain);
            //    _repository.insertLog(model.Domain.Id, AccountInfo.id, "insert", RequestUserInfo.IP);

            //    ViewBag.SuccesAlert = "Домен успешно добавлен.";
            //    ViewBag.backurl = model.Domain.Id;
            //}

            return RedirectToAction("Domains", new { @siteId = siteId });
        }

        [HttpPost]
        public string DeleteDomains(string id)
        {
            Guid domId = Guid.Parse(id);
            try
            {
                //_repository.deleteSiteDomain(domId);
                //_repository.insertLog(domId, AccountInfo.id, "delete", RequestUserInfo.IP);

                return "";
            }
            catch { return "Не удалось удалить доменное имя."; }
        }
    }
}