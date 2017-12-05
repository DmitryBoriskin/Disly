using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Frmp.library
{
    /// <summary>
    /// Параметры для интеграции
    /// </summary>
    public class IntegrationParams
    {
        /// <summary>
        /// Время начала интеграции
        /// </summary>
        public string StartTime { get; set; }
        
        /// <summary>
        /// Название директории
        /// </summary>
        public string DirName { get; set; }

        /// <summary>
        /// Путь для сохранения изображений
        /// </summary>
        public string SaveImgPath { get; set; }

        /// <summary>
        /// Название файла
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Минимальная допустимая дата рождения сотрудника
        /// </summary>
        public DateTime MinDate { get; set; }

        /// <summary>
        /// Максимальная допустимая дата рождения сотрудника
        /// </summary>
        public DateTime MaxDate { get; set; }

    }
}
