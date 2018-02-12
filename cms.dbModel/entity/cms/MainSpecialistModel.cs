using System;
using System.Collections.Generic;

namespace cms.dbModel.entity.cms
{
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
        public int AllCount { get; set; }
    }
    
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
        public string Title { get; set; }

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
    /// Модель, описывающая главного специалиста для привязки
    /// </summary>
    public class MainSpecialistShortModel
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
        /// Наличие связей с объектом
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Принадлежность к объекту( хозяин по отношению к объекту)
        /// </summary>
        public bool Origin { get; set; }

    }

}
