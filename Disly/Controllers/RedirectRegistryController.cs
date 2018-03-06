using System.Web.Mvc;
using cms.dbase.Repository;
using cms.dbModel.entity;
using System;
using cms.dbase;
using System.Collections.Generic;
using Disly.Models;

namespace Disly.Controllers
{
    public class RedirectRegistryController : RootController
    {
        private OnlineRegistryRepository repoRegistry { get; set; }

        private RedirectRegistryViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            repoRegistry = new OnlineRegistryRepository("registryConnection");

            model = new RedirectRegistryViewModel
            {
                SitesInfo = siteModel,
                SiteMapArray = siteMapArray,
                BannerArray = bannerArray,
                CurrentPage = currentPage,
                Breadcrumbs = new List<Breadcrumbs>()
            };

            #region Создаем переменные (значения по умолчанию)
            ViewBag.Title = "Страница";
            ViewBag.Description = "Страница без названия";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: /RedirectRegisty/Hospitals/{id}
        public ActionResult Hospitals(string id)
        {
            Hospital hospital = repoRegistry.getHospital(id);

            if (hospital != null)
                return Redirect(hospital.Url);
            else
                return RedirectToAction("Index", "Home");
            //Redirect("https://reg.med.cap.ru/");
        }

        // GET: /RedirectRegistry/Doctors/{id}/?oid=oid
        public ActionResult Doctors(Guid id, string oid)
        {
            string snils = _repository.getPeopleSnils(id);

            var doctors = repoRegistry.getVDoctors(oid, snils);

            if (doctors == null || doctors.Length > 1)
            {
                model.Doctors = doctors;

                model.Breadcrumbs.Add(new Breadcrumbs
                {
                    Title = "Расписание",
                    Url = ""
                });

                return View("~/Views/Doctors/Redirect.cshtml", model);
            }
            else if (!string.IsNullOrWhiteSpace(doctors[0].Url))
            {
                return Redirect(doctors[0].Url);
            }
            else
            {
                return RedirectToAction("Index", "Doctors");
            }
        }
    }
}
