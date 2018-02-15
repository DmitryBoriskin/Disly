using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity.cms
{
    /// <summary>
    /// Список главных специалистов и пейджер
    /// </summary>
    public class GSList
    {
        /// <summary>
        /// Список главных специалистов
        /// </summary>
        public IEnumerable<GSModel> Data { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
        public int AllCount { get; set; }
    }

    /// <summary>
    /// Модель, описывающая главного специалиста
    /// </summary>
    public class GSModel
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
        /// Ссылка на сайт главного специалиста
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// id людей Главных специалистов
        /// </summary>
        public IEnumerable<Guid> SpecialistsId { get; set; }
        /// <summary>
        /// Главные специалисты
        /// </summary>
        public People[] Specialists { get; set; }

        /// <summary>
        /// id людей Экспертный совет
        /// </summary>
        public IEnumerable<Guid> ExpertsId { get; set; }
        /// <summary>
        /// Экспертный совет
        /// </summary>
        public People[] Experts { get; set; }
    }

    /// <summary>
    /// Модель, описывающая главного специалиста для привязки
    /// </summary>
    public class GSShortModel
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

    /// <summary>
    /// Модель для прикрепления врача к гс
    /// </summary>
    public class GSMemberModel
    {
        /// <summary>
        /// id гc
        /// </summary>
        [Required(ErrorMessage = "Обязательный параметр")]
        public Guid Id { get; set; }

        /// <summary>
        /// тип (spec or expert)
        /// </summary>
        [Required(ErrorMessage = "Обязательный параметр")]
        public GSMemberType MemberType { get; set; }

        /// <summary>
        /// Идентификатор (человека f_people)
        /// </summary>
        public Guid PeopleId { get; set; }

        /// <summary>
        /// Информация о враче
        /// </summary>
        [Required(ErrorMessage = "Обязательный параметр")]
        public EmployeeModel Employee { get; set; }
    }
}
