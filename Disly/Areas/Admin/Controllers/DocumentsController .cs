using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using Disly.Areas.Admin.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class DocumentsController : CoreController
    {
        DocumentsViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.ControllerName = (String)RouteData.Values["controller"];
            ViewBag.ActionName = RouteData.Values["action"].ToString().ToLower();

            ViewBag.HttpKeys = Request.QueryString.AllKeys;
            ViewBag.Query = Request.QueryString;

            filter = getFilter(page_size);

            model = new DocumentsViewModel()
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
            MaterialsGroup[] GroupsValues = _cmsRepository.getAllMaterialGroups();
            ViewBag.AllGroups = GroupsValues;

            #region Метатеги
            ViewBag.Title = "Документы";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Documents
        public ActionResult Index(Guid id)
        {
            model.List = _cmsRepository.getDocuments(id);
            return View(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "create-btn-files")]
        public ActionResult Create(Guid id, IEnumerable<HttpPostedFileBase> upload)
        {
            string savePath = Settings.UserFiles + Domain + "/sitemap/doc/" + id + "/";
            if(upload != null && upload.Count() > 0)
            {
                foreach (var doc in upload)
                {
                    if (doc != null && doc.ContentLength > 0)
                    {
                        if (!AttachedFileExtAllowed(doc.FileName))
                        {
                            model.ErrorInfo = new ErrorMessage()
                            {
                                title = "Ошибка",
                                info = "Вы не можете загружать файлы данного формата",
                                buttons = new ErrorMassegeBtn[]
                                {
                                //Без перезагрузки, просто отменяем
                                new ErrorMassegeBtn { url = "#", text = "ок", action = "false", style="primary" }
                                }
                            };

                            return View("Item", model);
                        }
                        int idx = doc.FileName.LastIndexOf('.');
                        string Title = doc.FileName.Substring(0, idx);

                        string TransTitle = Transliteration.Translit(Title);
                        string FileName = TransTitle + Path.GetExtension(doc.FileName);

                        if (System.IO.File.Exists(Server.MapPath(Path.Combine(savePath, FileName))))
                        {
                            FileName = TransTitle + "(1)" + Path.GetExtension(doc.FileName);
                            Title = Title + "(1)";
                        }
                        string FullName = savePath + FileName;
                        if (!Directory.Exists(Server.MapPath(savePath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(savePath));
                        }
                        //сохраняем оригинал
                        doc.SaveAs(Server.MapPath(Path.Combine(savePath, FileName)));


                        DocumentsModel docModel = new DocumentsModel()
                        {
                            FilePath = FullName,
                            Title = Title,
                            LinkId = id
                        };

                        _cmsRepository.insDocuments(docModel);
                    }
                }
            }

            model.List = _cmsRepository.getDocuments(id);
            return View(model);
        }


        [HttpPost]
        public string Delete(Guid id)
        {
            //Guid Id = Guid.Parse(id);
            DocumentsViewModel model = new DocumentsViewModel()
            {
                Item = _cmsRepository.getDocumentsPath(id)
            };

            var res = _cmsRepository.deleteSiteMapDocuments(id);

            if (res && System.IO.File.Exists(Server.MapPath(model.Item.FilePath)))
            {
                System.IO.File.Delete(Server.MapPath(model.Item.FilePath));
            }

            return "";
        }

    }
}