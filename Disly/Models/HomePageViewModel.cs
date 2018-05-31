using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Models
{
    /// <summary>
    /// Модель для главной страницы
    /// </summary>
    public class HomePageViewModel : PageViewModel
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public List<MaterialFrontModule> Materials { get; set; }

        public List<MaterialFrontModule> MaterialsNewInMedicin { get; set; }


        public MaterialFrontModule ImportantMaterials { get; set; }

        /// <summary>
        /// Баннеры на главной
        /// </summary>
        public BannersModel[] BannerArrayIndex { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// Плитки
        /// </summary>
        public IEnumerable<SiteMapModel> SitemapPlate { get; set; }
        /// <summary>
        /// Слайдер
        /// </summary>
        public IEnumerable<BannersModel> Slider { get; set; }
        public IEnumerable<BannersModel> BenifitBanners { get; set; }



        //press module
        
        public IEnumerable<MaterialFrontModule> ModuleAnnouncement { get; set; }
        public IEnumerable<MaterialFrontModule> ModuleNews { get; set; }
        public IEnumerable<MaterialFrontModule> ModuleEvents { get; set; }
        public IEnumerable<MaterialFrontModule> ModuleActual { get; set; }

        public MaterialFrontModule ModulePhoto { get; set; }
        public MaterialFrontModule ModuleVideo { get; set; }
        public MaterialFrontModule ModuleNewsWorld { get; set; }
        public MaterialFrontModule ModuleNewsRus { get; set; }
        public MaterialFrontModule ModuleNewsChuv { get; set; }


    }
}
