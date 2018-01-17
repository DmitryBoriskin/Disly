using System;

namespace PhotoImport.Models
{
    /// <summary>
    /// Фотография
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор альбома
        /// </summary>
        public Guid AlbumId { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Превьюшка
        /// </summary>
        public string Preview { get; set; }

        /// <summary>
        /// Оригинал изображения
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }
    }
}