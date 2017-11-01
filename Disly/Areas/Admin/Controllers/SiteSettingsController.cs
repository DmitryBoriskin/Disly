using Disly.Areas.Admin.Models;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class SiteSettingsController : CoreController
    {
        // модель для вывода в представлении
        private SitesViewModel model;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SitesViewModel
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

        // GET: Admin/SiteSettings
        [HttpGet]
        public ActionResult Index()
        {
            model.Item = _cmsRepository.getSite(Domain);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Index(SitesViewModel backModel)
        {
            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                _cmsRepository.updateSiteInfo(backModel.Item, AccountInfo.id, RequestUserInfo.IP);
                userMessage.info = "Запись обновлена";
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMessage.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            model.Item = _cmsRepository.getSite(Domain);
            return View(model);
        }
    }
}