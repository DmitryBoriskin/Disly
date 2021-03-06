﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
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
        /// Идентификатор сайта
        /// </summary>
        public Guid? SiteId { get; set; }

        /// <summary>
        /// логотип сайта
        /// </summary>
        public string SiteImgUrl { get; set; }

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Ссылка на сайт главного специалиста
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Специализация
        /// </summary>
        public int[] Specialisations { get; set; }

        /// <summary>
        /// Главные специалисты
        /// </summary>
        public GSMemberModel[] Specialists { get; set; }

        /// <summary>
        /// Экспертный совет
        /// </summary>
        public GSMemberModel[] Experts { get; set; }
    }

    /// <summary>
    /// Модель, описывающая гc для привязки к объектам
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
        /// member id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// id гc
        /// </summary>
        [Required(ErrorMessage = "Обязательный параметр")]
        public Guid GSId { get; set; }

        /// <summary>
        /// тип (spec or expert)
        /// </summary>
        [Required(ErrorMessage = "Обязательный параметр")]
        public GSMemberType MemberType { get; set; }

        /// <summary>
        /// Инфо о человеке
        /// </summary>
        public PeopleModel People { get; set; }

        /// <summary>
        /// организации в которых он работает
        /// </summary>
        public OrgsModel[] Orgs { get; set; }

        /// <summary>
        /// Должности человека
        /// </summary>
        public Specialisation[] Posts { get; set; }

    }
}
