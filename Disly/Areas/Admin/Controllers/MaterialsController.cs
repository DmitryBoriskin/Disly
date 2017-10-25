using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class MaterialsController : CoreController
    {
        MaterialsViewModel model;
        FilterParams filter;

        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            model = new MaterialsViewModel()
            {
                Account = AccountInfo,
                Settings = SettingsInfo,
                UserResolution = UserResolutionInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Groups = _cmsRepository.getMaterialsGroups(),
                Events = _cmsRepository.getMaterialsEvents()
            };

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
            filter = getFilter(page_size);
            // Наполняем модель данными
            model.List = _cmsRepository.getMaterialsList(filter, Domain);

            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getMaterial(Id);
            if (model.Item == null)
                model.Item = new MaterialsModel()
                {
                    Date = DateTime.Now
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
        public ActionResult Search(string searchtext, bool disabled, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "disabled", disabled.ToString().ToLower());
            query = (date == null) ? addFiltrParam(query, "date", String.Empty) : addFiltrParam(query, "date", ((DateTime)date).ToString("dd.MM.yyyy").ToLower());
            query = (dateend == null) ? addFiltrParam(query, "dateend", String.Empty) : addFiltrParam(query, "dateend", ((DateTime)dateend).ToString("dd.MM.yyyy").ToString().ToLower());
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
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid Id, MaterialsViewModel bindData, HttpPostedFileBase upload)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                var res = false;
                var getMaterial = _cmsRepository.getMaterial(Id);

                // добавление необходимых полей перед сохранением модели
                bindData.Item.Id = Id;
                bindData.Item.DefaultSite = SiteInfo.Id;

                #region Сохранение изображения
                var width = 0;
                var height = 0;
                var defaultPreviewSizes = new string[] { "540", "360" };

                // путь для сохранения изображения //Preview image
                string savePath = Settings.UserFiles + Domain + Settings.MaterialsDir; //+2017_09
                if (upload != null && upload.ContentLength > 0)
                {
                    string fileExtension = upload.FileName.Substring(upload.FileName.IndexOf("."));

                    var validExtension = (!string.IsNullOrEmpty(Settings.PicTypes)) ? Settings.PicTypes.Split(',') : "jpg,jpeg,png,gif".Split(',');
                    if(!validExtension.Contains(fileExtension.ToLower()))
                    {
                        model.Item = _cmsRepository.getMaterial(Id);

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
                    res = _cmsRepository.updateCmsMaterial(bindData.Item);
                else
                {
                    bindData.Item.DefaultSite =  SiteInfo.ContentId;
                    bindData.Item.DefaultSiteType = SiteInfo.Type;
                    res = _cmsRepository.insertCmsMaterial(bindData.Item);
                }
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

            model.Item = _cmsRepository.getMaterial(Id);
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
            var res = _cmsRepository.deleteCmsMaterial(Id);

            // записываем информацию о результатах
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";
            userMassege.info = "Запись Удалена";
            userMassege.buttons = new ErrorMassegeBtn[]{
                new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
            };

            model.ErrorInfo = userMassege;

            return RedirectToAction("Index");
        }

        // admin/materials/orgs/{id}
        [HttpGet]
        public ActionResult Orgs(Guid id)
        {
            model.Item = _cmsRepository.getMaterial(id);
            model.OrgsByType = _cmsRepository.getOrgByType(id);
            return PartialView("Orgs", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-org-btn")]
        public ActionResult Orgs(MaterialsViewModel model)
        {
            MaterialOrgType modelInsert = new MaterialOrgType
            {
                OrgTypes = model.OrgsByType,
                Material = model.Item
            };

            _cmsRepository.insertMaterialsLinksToOrgs(modelInsert);
            
            return PartialView("OrgsSaved");
        }
    }
}