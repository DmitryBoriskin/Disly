using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Авторизованный пользователь
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Соль для шифрования пароля
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Хэш для шифрования пароля
        /// </summary>
        public string Hash { get; set; }
        public string Group { get; set; }        
        public string Surname { get; set; }
        public string Name { get; set; }        
        public string Patronymic { get; set; }
        public bool CountError { get; set; }
        public DateTime? LockDate { get; set; }
        public bool Disabled { get; set; }
        public DomainList[] Domains { get; set; }
    }

    public class DomainList
    {
        public int Permit { get; set; }
        public string SiteId { get; set; }
        public string DomainName { get; set; }
    }
}

