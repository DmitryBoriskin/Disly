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
                CurrentPage = currentPage
            };

            //#region Создаем переменные (значения по умолчанию)
            //string PageTitle = model.CurrentPage.Title;
            //string PageDesc = model.CurrentPage.Desc;
            //string PageKeyw = model.CurrentPage.Keyw;
            //#endregion

            //#region Метатеги
            //ViewBag.Title = PageTitle;
            //ViewBag.Description = PageDesc;
            //ViewBag.KeyWords = PageKeyw;
            //#endregion
        }

        // GET: /RedirectRegisty/Hospitals/{id}
        public ActionResult Hospitals(string id)
        {
            Hospital hospital = repoRegistry.getHospital(id);

            if (hospital != null)
                return Redirect(hospital.Url);
            else
                return RedirectToAction("Index", "Home");
        }

        // GET: /RedirectRegistry/Doctors/{id}/?oid=oid
        public ActionResult Doctors(Guid id, string oid)
        {
            string snils = _repository.getPeopleSnils(id);

            var doctors = repoRegistry.getVDoctors(oid, snils);

            if (doctors == null || doctors.Length > 1)
            {
                model.Doctors = doctors;

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
