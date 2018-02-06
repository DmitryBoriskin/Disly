using cms.dbModel.entity;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы новостей
    /// </summary>
    public class NewsViewModel : PageViewModel
    {
        public MaterialsList List { get; set; }
        public MaterialsModel Item { get; set; }

        public MaterialsGroup[] Group { get; set; }
        public SelectList NewInMedicin { get; set; }


    }
}
