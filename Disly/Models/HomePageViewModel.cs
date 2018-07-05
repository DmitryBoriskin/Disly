using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Models
{
    /// <summary>
    /// Модель для главной страницы
    /// </summary>
    public class HomePageViewModel : PageViewModel
    {

        //public MaterialFrontModule ImportantMaterials { get; set; }

        /// <summary>
        /// Баннеры на главной
        /// </summary>
        //public BannersModel[] BannerArrayIndex { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// Плитки
        /// </summary>
        //public IEnumerable<SiteMapModel> SitemapPlate { get; set; }
        /// <summary>
        /// Слайдер
        /// </summary>
        //public IEnumerable<BannersModel> Slider { get; set; }
        //public IEnumerable<BannersModel> BenifitBanners { get; set; }



        //module index
        public IndexModel ModuleIndex{ get; set; }


        //public IEnumerable<MaterialFrontModule> ModuleAnnouncement { get; set; }
        //public IEnumerable<MaterialFrontModule> ModuleNews { get; set; }
        //public IEnumerable<MaterialFrontModule> ModuleEvents { get; set; }
        //public IEnumerable<MaterialFrontModule> ModuleActual { get; set; }

        //public MaterialFrontModule ModulePhoto { get; set; }
        //public MaterialFrontModule ModuleVideo { get; set; }

        //public MaterialFrontModule ModuleNewsWorld { get; set; }
        //public MaterialFrontModule ModuleNewsRus { get; set; }
        //public MaterialFrontModule ModuleNewsChuv { get; set; }
    }


   
}
