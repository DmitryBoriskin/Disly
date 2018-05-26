using System;

namespace ImportOldInfo.Models
{
    /// <summary>
    /// Фотоальбом нового портала
    /// </summary>
    public class PhotoAlbumNew
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
        /// Превью
        /// </summary>
        public string Preview { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Доменное имя сайта
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Путь
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Идентификатор в старой БД
        /// </summary>
        public int? OldId { get; set; }
    }

    /// <summary>
    /// Фотоальбом старого портала
    /// </summary>
    public class PhotoAlbumOld
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Link { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        public int? Org { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Ширина
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Высота
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Директория
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Время создания
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Домен
        /// </summary>
        public string Domain { get; set; }
    }
}
