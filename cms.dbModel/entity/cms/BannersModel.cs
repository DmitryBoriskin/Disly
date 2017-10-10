using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity.cms
{
    /// <summary>
    /// Модель, описывающая баннеры
    /// </summary>
    public class BannersModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Изображени
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Порядок
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Флаг запрещённости эл-та
        /// </summary>
        public bool Disabled { get; set; }
    }
}
