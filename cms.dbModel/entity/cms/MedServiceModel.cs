using System;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Медицинская услуга
    /// </summary>
    public class MedServiceModel
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
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }
    }
}


