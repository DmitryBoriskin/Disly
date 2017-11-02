﻿using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
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
        /// Страница по умолчанию (Список)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            string return_url = ViewBag.urlQuery = HttpUtility.UrlDecode(Request.Url.Query);

            FilterParams filter = getFilter();            
            model.List = _cmsRepository.getSiteList(filter, page, page_size);

            return View(model);
        }
                
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getSite(Id);

            //тип контента и источник контента
            
            switch (model.Item.Type)
            {
                case "org":
                    var data =_cmsRepository.getOrgItem(model.Item.ContentId);
                    ViewBag.ContentLink="/admin/orgs/item/"+ data.Id;
                    ViewBag.ContentType = "организации";
                    ViewBag.ContentTitle = data.Title;
                    break;
                case "people":
                    var data_p = _cmsRepository.getPerson(model.Item.ContentId);
                    ViewBag.ContentLink = "/admin/Person/item/" + data_p.Id;
                    ViewBag.ContentType = "персоны";
                    ViewBag.ContentTitle = data_p.FIO;
                    break;
                case "event":
                    var data_e = _cmsRepository.getEvent(model.Item.ContentId);
                    ViewBag.ContentLink = "/admin/events/item/" + data_e.Id;
                    ViewBag.ContentType = "события";
                    ViewBag.ContentTitle = data_e.Title;
                    break;
            }
            return View("Item", model);
        }
                
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Master()
        {
            FilterParams filter = new FilterParams()
            {
                Page = 1,
                Size = 999999
            };
            #region Данные из адресной строки(для случаев когда сайт создается со страницы того кому создается сайт)
            string OrgType = Request.QueryString["type"];
            string ContentId = Request.QueryString["contentid"];

            if (!String.IsNullOrEmpty(OrgType))
            {
                ViewBag.OrgType = OrgType;
            }
            else{ }
            if (!String.IsNullOrEmpty(ContentId))
            {
                ViewBag.ContentId = Guid.Parse(ContentId);                                
            }            
            #endregion
            #region данные для выпадающих списков
            model.TypeList = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Не выбрано", Value =""},
                        new SelectListItem { Text = "Организация", Value ="org"},
                        new SelectListItem { Text = "Врач", Value = "people" },
                        new SelectListItem { Text = "Событие", Value = "event" }
                    }, "Value", "Text", OrgType
                );
            model.OrgsList = new SelectList(_cmsRepository.getOrgsList((OrgFilter)filter), "Id", "Title", ContentId);
            model.PeopleList = new SelectList(_cmsRepository.getPersonList(filter).Data, "Id", "FIO", ContentId);
            model.EventsList = new SelectList(_cmsRepository.getEventsList(filter).Data, "Id", "Title", ContentId);
            #endregion
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
        public ActionResult Search(string filter, bool disabled, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", filter);
            if (disabled) query = addFiltrParam(query, "disabled", String.Empty);
            else query = addFiltrParam(query, "disabled", enabeld.ToString().ToLower());
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
            if (!_cmsRepository.check_Site(Id) && back_model.Item.ContentId == null)
            {
                ModelState.AddModelError("Name", "Необходимо выбрать тип и контент сайта");
            }

            if (ModelState.IsValid)
            {
                #region ///
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
                #endregion

                if (_cmsRepository.check_Site(Id))
                {                    
                    _cmsRepository.updSite(Id, back_model.Item, AccountInfo.id, RequestUserInfo.IP);
                    userMassege.info = "Запись обновлена";
                }
                else if (!_cmsRepository.check_Site(Id))
                {
                    back_model.Item.Id = Id;
                    _cmsRepository.insSite(back_model.Item, AccountInfo.id, RequestUserInfo.IP);
                    userMassege.info = "Запись добавлена";
                }
                else
                {
                    userMassege.info = "Запись с таким псевдонимом уже существует. <br />Замените псевдоним.";
                }
                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                    new ErrorMassegeBtn { url = StartUrl + "item/" + Id + "/" + Request.Url.Query, text = "редактировать" }
                };
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы";
                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
                model.Item = _cmsRepository.getSite(Id);
                model.ErrorInfo = userMassege;

                return View("Master", model);
            }

            model.Item = _cmsRepository.getSite(Id);
            model.ErrorInfo = userMassege;


            #region Данные из адресной строки(для случаев когда сайт создается со страницы того кому создается сайт)
            FilterParams filter = new FilterParams()
            {
                Page = 1,
                Size = 999999
            };
            string OrgType = Request.QueryString["type"];
            string ContentId = Request.QueryString["contentid"];

            if (!String.IsNullOrEmpty(OrgType))
            {
                ViewBag.OrgType = OrgType;
            }
            else { }
            if (!String.IsNullOrEmpty(ContentId))
            {
                ViewBag.ContentId = Guid.Parse(ContentId);
            }
            #endregion
            #region данные для выпадающих списков
            model.TypeList = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Не выбрано", Value =""},
                        new SelectListItem { Text = "Организация", Value ="org"},
                        new SelectListItem { Text = "Врач", Value = "people" },
                        new SelectListItem { Text = "Событие", Value = "event" }
                    }, "Value", "Text", OrgType
                );
            model.OrgsList = new SelectList(_cmsRepository.getOrgsList((OrgFilter)filter), "Id", "Title", ContentId);
            model.PeopleList = new SelectList(_cmsRepository.getPersonList(filter).Data, "Id", "FIO", ContentId);
            model.EventsList = new SelectList(_cmsRepository.getEventsList(filter).Data, "Id", "Title", ContentId);
            #endregion

            return View("Item", model);
        }

        /// <summary>
        /// Добавление домена
        /// </summary>
        /// <returns>перезагружает страницу</returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-new-domain")]
        public ActionResult AddDomain()
        {
            Guid id=Guid.Parse(Request["Item.Id"]);
            var SiteId = _cmsRepository.getSite(id).Alias;
            string Domain=Request["new_domain"];
            _cmsRepository.insDomain(SiteId, Domain, AccountInfo.id, RequestUserInfo.IP);
            return Redirect(((System.Web.HttpRequestWrapper)Request).RawUrl);
        }
        [HttpPost]
        public ActionResult DelDomain(Guid id)
        {
            _cmsRepository.delDomain(id, AccountInfo.id, RequestUserInfo.IP);
            return null;
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
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";
            if(_cmsRepository.delSite(Id, AccountInfo.id, RequestUserInfo.IP))////——————УДАЛЕНИЕ DELETE
            {
                userMassege.info = "Запись Удалена";
                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Ок" }
                };
            }
            else
            {
                userMassege.info = "Произошла ошибка";
                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }                        
            model.ErrorInfo = userMassege;
            return View("item", model);
        }

    }
}