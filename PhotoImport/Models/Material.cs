using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoImport.Models
{
    /// <summary>
    /// Новость
    /// </summary>
    public class Material
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Превью
        /// </summary>
        public string Preview { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Привязанная галерея
        /// </summary>
        public int Gallery { get; set; }

        /// <summary>
        /// Старый идентификатор
        /// </summary>
        public int? OldId { get; set; }
    }
}
