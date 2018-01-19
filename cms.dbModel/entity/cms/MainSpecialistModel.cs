using System;
using System.Collections.Generic;

namespace cms.dbModel.entity.cms
{
    /// <summary>
    /// Модель, описывающая главного специалиста
    /// </summary>
    public class MainSpecialistModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Специализация
        /// </summary>
        public int[] Specialisations { get; set; }

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public Guid? SiteId { get; set; }

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Адреса сайта
        /// </summary>
        public string DomainUrl { get; set; }

        /// <summary>
        /// Главные специалисты
        /// </summary>
        public IEnumerable<Guid> EmployeeMainSpecs { get; set; }

        /// <summary>
        /// Экспертный совет
        /// </summary>
        public IEnumerable<Guid> EmployeeExpSoviet { get; set; }
        public  OrgsModel Organization { get; set; }
    }

    /// <summary>
    /// Список главных специалистов и пейджер
    /// </summary>
    public class MainSpecialistList
    {
        /// <summary>
        /// Список главных специалистов
        /// </summary>
        public IEnumerable<MainSpecialistModel> Data { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }
}
