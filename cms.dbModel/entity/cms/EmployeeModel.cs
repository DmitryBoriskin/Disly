using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity.cms
{
    /// <summary>
    /// Модель, описывающая сотрудиника
    /// </summary>
    public class EmployeeModel
    {
        /// <summary>
        /// Идентификатор сотрудника
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор человека
        /// </summary>
        public Guid PeopleId { get; set; }

        /// <summary>
        /// Фотография
        /// </summary>
        public Photo Photo { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Снилс
        /// </summary>
        public string Snils { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string Fullname
        {
            get
            {
                return string.Format("{0} {1} {2}", Surname, Name, Patronymic);
            }
        }

        /// <summary>
        /// Список организаций в которых работает врач
        /// </summary>
        public OrgsModel[] Orgs { get; set; }

        /// <summary>
        /// Список занимаемых должностей
        /// </summary>
        public EmployeePost[] Posts { get; set; }

    }

    /// <summary>
    /// Должность сотрудника
    /// </summary>
    public class EmployeePost
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Родительский ключ
        /// </summary>
        public int? Parent { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Организация по данной должности
        /// </summary>
        public OrgsShortModel Org { get; set; }

    }

}
