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
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

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
        /// Список занимаемых должностей
        /// </summary>
        public IEnumerable<EmployeePostModel> Posts { get; set; }

        /// <summary>
        /// Фотография
        /// </summary>
        public Photo Photo { get; set; }
    }

    /// <summary>
    /// Модель, описывающая должности 
    /// </summary>
    public class EmployeePostModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        public int? Parent { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
    }
}
