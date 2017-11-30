using System;
using System.Web.Mvc;

namespace Disly.Controllers
{
    public class RedirectController : RootController
    {
        // GET: Redirect
        public ActionResult Link(Guid id)
        {
            var banner = _repository.getBanner(id);

            string link = (banner != null && !string.IsNullOrWhiteSpace(banner.Url))
                ? banner.Url : "/";

            return Redirect(link);
        }
    }
}