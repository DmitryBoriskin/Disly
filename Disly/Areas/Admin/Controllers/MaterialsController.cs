using cms.dbModel.entity;
using cms.dbModel.entity.cms;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Disly.Areas.Admin.Controllers
{
    public class MaterialsController : CoreController
    {
        MaterialsViewModel model;
        FilterParams filter;
        MaterialsGroup[] Groups;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new MaterialsViewModel()
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

            //Справочник всех доступных категорий
            Groups = _cmsRepository.getAllMaterialGroups();

            model.NewInMedicin = new SelectList(
              new List<SelectListItem>
              {
                        new SelectListItem { Text = "не выбрано", Value =""},
                        new SelectListItem { Text = "Мира", Value ="world"},
                        new SelectListItem { Text = "России", Value ="russia"},
                        new SelectListItem { Text = "Чувашии", Value = "chuvashia" }
              }, "Value", "Text"
          );



            #region Метатеги
            ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Materials
        public ActionResult Index(string category, string type)
        {
            // Наполняем фильтр значениями
            var mfilter = FilterParams.Extend<MaterialFilter>(filter);
            model.List = _cmsRepository.getMaterialsList(mfilter);

            MaterialsGroup[] categories = Groups;
            if (categories != null)

                model.Categories = categories.Select(p => new Catalog_list()
                {
                    Text = p.Title,
                    Value = p.Alias
                }).ToArray();

            #region Group filter
            var alias = "group";
            var groupLink = "/admin/materials/";
            var editGroupUrl = "/admin/materials/groupinfo/";

            string Link = Request.Url.Query;
            string active = Request.QueryString[alias];

            if (model.Categories != null && model.Categories.Count() > 0)
            {
                model.Filtr = new FiltrModel()
                {
                    Title = "Группы новостей",
                    Icon = "icon-th-list-3",
                    BtnName = "Новая группа новостей",
                    Alias = alias,
                    Url = editGroupUrl,
                    ReadOnly = true,
                    AccountGroup = (model.Account != null) ? model.Account.Group : "",
                    Items = model.Categories.Select(p =>
                        new Catalog_list()
                        {
                            Text = p.Text,
                            Value = p.Value,
                            Link = AddFiltrParam(Link, alias, p.Value),
                            Url = editGroupUrl + p.Value + "/",
                            Selected = (active == p.Value) ? true : false
                        })
                        .ToArray(),
                    Link = groupLink
                };
            }
            #endregion

            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getMaterial(Id);

            ViewBag.DataPath = Settings.UserFiles + Domain + Settings.MaterialsDir;
            ViewBag.DataPath = (model.Item == null) ?
                ViewBag.DataPath + DateTime.Today.ToString("yyyy") + "/" + DateTime.Today.ToString("MM") + "/" + DateTime.Today.ToString("dd") + "/"
                :
                ViewBag.DataPath + model.Item.Date.ToString("yyyy") + "/" + model.Item.Date.ToString("MM") + "/" + model.Item.Date.ToString("dd") + "/";

            if (model.Item == null)
            {
                model.Item = new MaterialsModel()
                {
                    Id = Id,
                    Date = DateTime.Now
                };
           
            }
            else
            {
                model.NewInMedicin = new SelectList(
                      new List<SelectListItem>
                      {
                        new SelectListItem { Text = "не выбрано", Value =""},
                        new SelectListItem { Text = "Мира", Value ="world"},
                        new SelectListItem { Text = "России", Value ="russia"},
                        new SelectListItem { Text = "Чувашии", Value = "chuvashia" }
                      }, "Value", "Text",model.Item.SmiType
                  );
            }
            
            if (model.Item.PreviewImage != null && model.Item.PreviewImage != null && !string.IsNullOrEmpty(model.Item.PreviewImage.Url))
            {
                var photo = model.Item.PreviewImage;
                model.Item.PreviewImage = Files.getInfoImage(photo.Url);
                model.Item.PreviewImage.Source = photo.Source;
            }

            //Заполняем для модели связи с другими объектами
            EventsModel[] events = null;
            var eventFilter = FilterParams.Extend<EventFilter>(filter);
            eventFilter.RelId = Id;
            eventFilter.Domain = null;
            eventFilter.RelType = ContentType.MATERIAL;
            var eventsList = _cmsRepository.getEventsList(eventFilter);
            events = (eventsList != null) ? eventsList.Data : null;

            OrgsModel[] orgs = null;
            var orgfilter = FilterParams.Extend<OrgFilter>(filter);
            orgfilter.RelId = Id;
            orgfilter.RelType = ContentType.MATERIAL;
            orgs = _cmsRepository.getOrgs(orgfilter);

            GSModel[] spec = null;
            var specfilter = FilterParams.Extend<GSFilter>(filter);
            specfilter.RelId = Id;
            specfilter.RelType = ContentType.MATERIAL;
            var specList = _cmsRepository.getGSList(specfilter);
            spec = (specList != null)? 
                (specList.Data != null)? specList.Data.ToArray() : null
                :null;

            model.Item.Links = new ObjectLinks()
            {
                Events = events,
                Orgs = orgs,
                Specs = spec
            };

            ViewBag.AllGroups = Groups;

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
        public ActionResult Search(string searchtext, string group, bool enabled, string size, DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFiltrParam(query, "searchtext", searchtext);
            query = AddFiltrParam(query, "disabled", (!enabled).ToString().ToLower());
            if (datestart.HasValue)
                query = AddFiltrParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());
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

            return Redirect(StartUrl + "Item/" + Guid.NewGuid() + "/" + query);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, MaterialsViewModel bindData, HttpPostedFileBase upload)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                var res = false;
                var getMaterial = _cmsRepository.getMaterial(Id);

                // добавление необходимых полей перед сохранением модели
                bindData.Item.Id = Id;
                bindData.Item.ContentLink = SiteInfo.ContentId;
                bindData.Item.ContentLinkType = SiteInfo.Type;

                #region Сохранение изображения
                var width = 0;
                var height = 0;
                var defaultPreviewSizes = new string[] { "540", "360" };

                // путь для сохранения изображения //Preview image
                string savePath = Settings.UserFiles + Domain + Settings.MaterialsDir; //+2017_09
                if (upload != null && upload.ContentLength > 0)
                {
                    if (!AttachedPicExtAllowed(upload.FileName))
                    {
                        model.Item = (_cmsRepository.getMaterial(Id) != null) ? _cmsRepository.getMaterial(Id)
                          : new MaterialsModel() { Id = Id };

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
               
                if (getMaterial != null)
                {
                    userMessage.info = "Запись обновлена";
                    res = _cmsRepository.updateCmsMaterial(bindData.Item);
                }

                else
                {
                    userMessage.info = "Запись добавлена";                    
                    res = _cmsRepository.insertCmsMaterial(bindData.Item);
                }
                //Сообщение пользователю
                if (res)
                {
                    string currentUrl = Request.Url.PathAndQuery;

                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = currentUrl, text = "ок" }
                 };
                }
                else
                {
                    userMessage.info = "Произошла ошибка";

                    userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false"  }
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

            model.Item = _cmsRepository.getMaterial(Id);
            if (model.Item != null && model.Item.PreviewImage != null && !string.IsNullOrEmpty(model.Item.PreviewImage.Url))
            {
                var photo = model.Item.PreviewImage;
                model.Item.PreviewImage = Files.getInfoImage(model.Item.PreviewImage.Url);
                model.Item.PreviewImage.Source = photo.Source;
            }
            model.NewInMedicin = new SelectList(
                   new List<SelectListItem>
                   {
                        new SelectListItem { Text = "не выбрано", Value =""},
                        new SelectListItem { Text = "Мира", Value ="world"},
                        new SelectListItem { Text = "России", Value ="russia"},
                        new SelectListItem { Text = "Чувашии", Value = "chuvashia" }
                   }, "Value", "Text", model.Item.SmiType
               );

            model.ErrorInfo = userMessage;

            ViewBag.AllGroups = Groups;

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
            // записываем информацию о результатах
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";
            //В случае ошибки
            userMessage.info = "Ошибка, Запись не удалена";
            userMessage.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.Item = _cmsRepository.getMaterial(Id);
            if (model.Item != null)
            {
                var image = (model.Item.PreviewImage != null) ? model.Item.PreviewImage.Url : null;
                var res = _cmsRepository.deleteCmsMaterial(Id);
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

            ViewBag.AllGroups = Groups;

            return View(model);
            //return RedirectToAction("Index");
        }

        //Получение списка организаций по параметрам
        [HttpGet]
        public ActionResult Orgs(Guid id)
        {
            model.Item = _cmsRepository.getMaterial(id);
            model.OrgsByType = _cmsRepository.getOrgByType(id);

            // прочие организации, непривязанные к типам
            OrgType anotherOrgs = new OrgType
            {
                Id = Guid.Parse("4D30E508-5C56-43A0-9F25-EEFF026F95EF"),
                Title = "Прочие организации",
                Sort = model.OrgsByType.Select(s => s.Sort).Max() + 1,
                Orgs = _cmsRepository.getOrgAttachedToTypes(id)
            };

            if (anotherOrgs.Orgs != null)
                model.OrgsByType.Add(anotherOrgs);

            return PartialView("Orgs", model);
        }

        #region rss импорт

        [HttpGet]
        public ActionResult RssList()
        {
            ViewBag.Title = "Импорт из RSS";
            //список подключенных rss лент  + очистка объектов прошлой выборки из базы
            model.RssChannelList = _cmsRepository.getRssChannelMiniList();
            if (model.RssChannelList != null && model.RssChannelList.Count() > 0)
            {
                foreach (var item in model.RssChannelList)
                {
                    XmlParser(item.RssLink);//запись в таблицу актуальных значений подключенных rss лент
                }
            }

            //вывод значений записанных в предыдущем шаге
            model.RssObject = _cmsRepository.getRssObjects();

            return View(model);
        }

        // Вспомогательный метод для парсинга XML
        public void XmlParser(string Url)
        {
            //try { 
            string XmlUrl = Url;
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;

            string xml = client.DownloadString(XmlUrl);

            XDocument doc = XDocument.Load(XmlUrl);

            DateTime dt;
            XNamespace yandex = "http://news.yandex.ru";

            List<RssItem> items = (from s in doc.Descendants("item")
                                   select new RssItem()
                                   {
                                       title = s.Element("title").Value,
                                       link = s.Element("link").Value,
                                       enclosure = (s.Element("enclosure") != null) ? s.Element("enclosure").Attribute("url").Value : null,
                                       yandex_full_text = (string)s.Element(yandex + "full-text").Value,
                                       pubDate = DateTime.TryParseExact((s.Element("pubDate").Value)
                                       , "ddd, dd MMM yyyy HH:mm:ss +ffff",
                                       System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt) ?
                                       DateTime.ParseExact((s.Element("pubDate").Value)
                                       , "ddd, dd MMM yyyy HH:mm:ss +ffff",
                                       System.Globalization.CultureInfo.InvariantCulture) : DateTime.ParseExact((s.Element("pubDate").Value)
                                       , "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                       System.Globalization.CultureInfo.InvariantCulture),
                                       description = s.Element("description").Value
                                   }).ToList();

            List<RssImportModel> channel = (from s in doc.Descendants("channel")
                                            select new RssImportModel()
                                            {
                                                title = s.Element("title").Value,
                                                link = s.Element("link").Value,
                                                description = s.Element("description").Value,
                                                language = s.Element("language").Value,
                                                copyright = s.Element("copyright").Value,

                                                //             lastBuildDate = DateTime.TryParseExact((s.Element("lastbuilddate").Value)
                                                //    , "ddd, dd MMM yyyy HH:mm:ss +ffff",
                                                //    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt) ?
                                                //    DateTime.ParseExact((s.Element("lastbuilddate").Value)
                                                //             , "ddd, dd MMM yyyy HH:mm:ss +ffff",
                                                //    System.Globalization.CultureInfo.InvariantCulture) : DateTime.ParseExact((s.Element("lastbuilddate").Value)
                                                //    , "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                                //System.Globalization.CultureInfo.InvariantCulture),
                                                items = items
                                            }
                                          ).ToList();

            RssImportModel importModel = channel[0];
            if (importModel.items != null && importModel.items.Count() > 0)
            {
                foreach (RssItem rssItem in importModel.items)
                {
                    Guid id = Guid.NewGuid();
                    Photo Prev = (rssItem.enclosure != null) ? new Photo { Url = rssItem.enclosure } : null;
                    MaterialsModel item = new MaterialsModel()
                    {
                        Id = id,
                        Title = rssItem.title,
                        PreviewImage = Prev,
                        Alias = Transliteration.Translit(rssItem.title),
                        Desc = rssItem.description,
                        Date = rssItem.pubDate,
                        Year = Convert.ToInt32(rssItem.pubDate.ToString("yyyy")),
                        Month = Convert.ToInt32(rssItem.pubDate.ToString("MM")),
                        Day = Convert.ToInt32(rssItem.pubDate.ToString("dd")),
                        Text = rssItem.yandex_full_text,
                        Url = rssItem.link,
                        UrlName = importModel.title,
                        Disabled = false,
                        ImportRss = true
                    };
                    _cmsRepository.insertRssObject(item);
                }
            }

            //}
            //catch { }
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-rss")]
        public ActionResult AddRssLenta(string RssUrl)
        {
            ErrorMessage userMessage = new ErrorMessage();
            userMessage.title = "Информация";

            WebRequest request = WebRequest.Create(RssUrl);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response.StatusDescription == "OK")
            {
                string XmlUrl = RssUrl;
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string xml = client.DownloadString(XmlUrl);
                XDocument doc = XDocument.Load(XmlUrl);

                RssChannel channel
                    = (from s in doc.Descendants("channel")
                       select new RssChannel()
                       {
                           Title = s.Element("title").Value
                       }).FirstOrDefault();

                if (_cmsRepository.insertRssLink(RssUrl, channel.Title))
                {
                    userMessage.info = "Rss лента добавлена";
                }
                else
                {
                    userMessage.info = "Эта Rss лента добавлена";
                }
            }

            userMessage.buttons = new ErrorMassegeBtn[]{
                                     new ErrorMassegeBtn { url = "/Admin/materials/rsslist", text = "ок"}
                                 };



            model.ErrorInfo = userMessage;
            return View(model);
        }


        [HttpPost]
        public ActionResult RealizeRssImport(Guid id)
        {
            MaterialsModel data = _cmsRepository.getRssMaterial(id);
            data.Alias = Transliteration.Translit(data.Title);
            data.Text = AdaptationXMLForHtml(data.Text);
            data.Title = AdaptationXMLForHtml(data.Title);
            data.ContentLink = SiteInfo.ContentId;
            data.ContentLinkType = SiteInfo.Type;
            _cmsRepository.insertCmsMaterialRss(data);
            return null;
        }

        /// <summary>
        /// удаляет rss ленту
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteRssLenta(Guid id)
        {
            _cmsRepository.delRssLink(id);
            return null;
        }

        public string AdaptationXMLForHtml(string source)
        {
            if (source == null) return null;
            Dictionary<string, string> words = new Dictionary<string, string>();

            words.Add("&amp;", "&");
            words.Add("&lt;", "<");
            words.Add("&gt;", ">");
            words.Add("&apos;", "'");
            words.Add("&quot;", "\"");

            Regex re = new Regex("[-]{2,}");
            Regex Re = new Regex("[_]{2,}");
            Regex StartRe = new Regex("^[-|_]{1,}");
            Regex EndRe = new Regex("[-|_]${1,}");

            foreach (KeyValuePair<string, string> pair in words)
            {
                source = source.Replace(pair.Key, pair.Value);
            }
            source = re.Replace(source, "-");
            source = Re.Replace(source, "_");
            source = StartRe.Replace(source, "");
            source = EndRe.Replace(source, "");
            return source;
        }
        public string AdaptationHtmlForXML(string source)
        {
            if (source == null) return null;
            Dictionary<string, string> words = new Dictionary<string, string>();

            words.Add("&", "&amp;");
            words.Add("<", "&lt;");
            words.Add(">", "&gt;");
            words.Add("'", "&apos;");
            words.Add("\"", "&quot;");

            Regex re = new Regex("[-]{2,}");
            Regex Re = new Regex("[_]{2,}");
            Regex StartRe = new Regex("^[-|_]{1,}");
            Regex EndRe = new Regex("[-|_]${1,}");

            foreach (KeyValuePair<string, string> pair in words)
            {
                source = source.Replace(pair.Key, pair.Value);
            }
            source = re.Replace(source, "-");
            source = Re.Replace(source, "_");
            source = StartRe.Replace(source, "");
            source = EndRe.Replace(source, "");
            return source;
        }
        #endregion

        [HttpGet]
        public ActionResult GroupInfo(string id)
        {
            GroupModel model = _cmsRepository.getMaterialGroup(id);

            return PartialView("Modal/Group", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-group-btn")]
        public ActionResult NewGroup(GroupModel bindData)
        {
            if (bindData != null)
            {
                if (ModelState.IsValid)
                {
                    var res = _cmsRepository.updateMaterialGroup(bindData);
                    if (res)
                        return PartialView("Modal/Success");
                }
            }

            return PartialView("Modal/Error");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-group-btn")]
        public ActionResult SaveGroup(GroupModel bindData)
        {
            if (bindData != null)
            {
                var res = _cmsRepository.updateMaterialGroup(bindData);
                if (res)
                    return PartialView("Modal/Success");
            }

            return PartialView("Modal/Error");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-group-btn")]
        public ActionResult DeleteGroup(GroupModel bindData)
        {
            var res = _cmsRepository.deleteMaterialGroup(bindData.Alias);
            if (res)
                return PartialView("Modal/Success");

            return PartialView("Modal/Error");
        }
    }
}