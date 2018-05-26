using System;

namespace ImportOldInfo.Models
{
    /// <summary>
    /// Идентификаторы организации
    /// </summary>
    public class Org
    {
        /// <summary>
        /// Интовый идентификатор
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Новый идентификатор
        /// </summary>
        public Guid UUID { get; set; }

        /// <summary>
        /// Домен
        /// </summary>
        public string Alias { get; set; }
    }
}
