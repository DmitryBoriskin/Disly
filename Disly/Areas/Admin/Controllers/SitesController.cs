using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class SitesController : CoreController
    {
        SitesViewModel model;
        string filter = String.Empty;
        bool enabeld = true;

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
        /// Страница по умолчанию (Список)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            string return_url = ViewBag.urlQuery = HttpUtility.UrlDecode(Request.Url.Query);

            FilterParams filter = getFilter();
            var sitefilter = FilterParams.Extend<SiteFilter>(filter);
            model.List = _cmsRepository.getSiteList(sitefilter);

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
                    var data = _cmsRepository.getOrgItem(model.Item.ContentId);
                    ViewBag.ContentLink = "/admin/orgs/item/" + data.Id;
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
                        new SelectListItem { Text = "Главный специалист", Value = "spec" },
                        new SelectListItem { Text = "Событие", Value = "event" }
                    }, "Value", "Text", OrgType
                );

            var orgfilter = FilterParams.Extend<OrgFilter>(filter);
            var evfilter = FilterParams.Extend<EventFilter>(filter);

            model.OrgsList = new SelectList(_cmsRepository.getOrgs(orgfilter), "Id", "Title", ContentId);
            model.MainSpecialistList = new SelectList(_cmsRepository.getMainSpecialistList(filter).Data, "Id", "Name", ContentId);
            model.EventsList = new SelectList(_cmsRepository.getEventsList(evfilter).Data, "Id", "Title", ContentId);
            #endregion
            return View("Master", model);
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
        public ActionResult Search(string searchtext, bool enabled, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "searchtext", searchtext);
            query = AddFiltrParam(query, "disabled", (!enabled).ToString().ToLower());
            if (date.HasValue)
                query = AddFiltrParam(query, "date", date.Value.ToString("dd.MM.yyyy").ToLower());
            if (dateend.HasValue)
                query = AddFiltrParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());
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


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            //  При создании записи сбрасываем номер страницы
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "page", String.Empty);

            return Redirect(StartUrl + "Master/" + Guid.NewGuid() + "/" + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, SitesViewModel back_model)
        {
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            if (!_cmsRepository.check_Site(Id) && back_model.Item.ContentId == null)
            {
                ModelState.AddModelError("Name", "Необходимо выбрать тип и контент сайта");
            }

            if (ModelState.IsValid)
            {
                // дополнительные домены
                List<string> domains = new List<string>();
                domains.Add(back_model.Item.Alias + "."+Settings.BaseURL);

                if (!string.IsNullOrEmpty(back_model.Item.DomainListString))
                {
                    string[] dopDomains = back_model.Item.DomainListString.Replace(" ","").Split(';');
                    if (dopDomains != null && dopDomains.Count() > 0)
                    {
                        foreach (var d in dopDomains)
                        {
                            if (!string.IsNullOrEmpty(d))
                            {
                                domains.Add(d);
                            }
                        }
                    }
                    back_model.Item.DomainListArray = domains;
                }

                if (_cmsRepository.check_Site(Id))
                {
                    _cmsRepository.updateSite(Id, back_model.Item); //, AccountInfo.id, RequestUserInfo.IP
                    userMassege.info = "Запись обновлена";
                }
                else if (!_cmsRepository.check_Site(Id))
                {
                    back_model.Item.Id = Id;
                    _cmsRepository.insertSite(back_model.Item); //, AccountInfo.id, RequestUserInfo.IP
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
                        new SelectListItem { Text = "Главный специалист", Value = "spec" },
                        new SelectListItem { Text = "Событие", Value = "event" }
                    }, "Value", "Text", OrgType
                );
            var orgfilter = FilterParams.Extend<OrgFilter>(filter);
            var evfilter = FilterParams.Extend<EventFilter>(filter);
            model.OrgsList = new SelectList(_cmsRepository.getOrgs(orgfilter), "Id", "Title", ContentId);
            model.MainSpecialistList = new SelectList(_cmsRepository.getMainSpecialistList(filter).Data, "Id", "Name", ContentId);
            model.EventsList = new SelectList(_cmsRepository.getEventsList(evfilter).Data, "Id", "Title", ContentId);
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
            try
            {

                Guid id = Guid.Parse(Request["Item.Id"]);
                var SiteId = _cmsRepository.getSite(id).Alias;
                string Domain = Request["new_domain"].Replace(" ","");

                _cmsRepository.insertDomain(SiteId, Domain);
            }
            catch (Exception ex)
            {
                throw new Exception("SitesController > AddDomain: " + ex);
            }

            return Redirect(((System.Web.HttpRequestWrapper)Request).RawUrl);
        }

        [HttpPost]
        public ActionResult SetDomainDefault(Guid id)
        {
            var res = _cmsRepository.setDomainDefault(id);
            if (res)
                return Json("Success");
            
            return Json("An Error Has occourred");
        }

        [HttpPost]
        public ActionResult DelDomain(Guid id)
        {
            _cmsRepository.deleteDomain(id);
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
            ErrorMessage userMassege = new ErrorMessage();
            userMassege.title = "Информация";
            if (_cmsRepository.deleteSite(Id))
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


        //Получение списка сайтов по параметрам для отображения в модальном окне
        [HttpGet]
        public ActionResult SiteListModal(Guid objId, ContentType objType)
        {
            var filtr = new SiteFilter()
            {
                Domain = Domain,
                RelId = objId,
                RelType = objType,
                Size = 1000
            };

            var model = new SitesModalViewModel()
            {
                ObjctId = objId,
                ObjctType = objType,
                SitesList = _cmsRepository.getSiteListWithCheckedForBanner(filtr)
            };

            return PartialView("Modal/Sites", model);
        }

        [HttpPost]
        public ActionResult UpdateLinkToSite(ContentLinkModel data)
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