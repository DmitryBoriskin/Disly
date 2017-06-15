using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Linq;

namespace cms.dbase
{
    public class AccountRepository : abstract_AccountRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        /// <summary>
        /// Конструктор
        /// </summary>
        public AccountRepository()
        {
            _context = "defaultConnection";
        }
        public AccountRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }

        
        /// <summary>
        /// Получаем данные об пользователе по email или id
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public override AccountModel getCmsAccount(string Email)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_userss.
                    Where(w => w.c_email == Email).
                    Select(s => new AccountModel
                    {
                        id = s.id,
                        Mail = s.c_email,
                        Salt = s.c_salt,
                        Hash = s.c_hash,
                        Group = s.f_group,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Disabled = s.b_disabled,
                        Deleted = s.b_deleted
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override AccountModel getCmsAccount(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_userss.
                    Where(w => w.id == Id).
                    Select(s => new AccountModel
                    {
                        id = s.id,
                        Mail = s.c_email,
                        Salt = s.c_salt,
                        Hash = s.c_hash,
                        Group = s.f_group,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Disabled = s.b_disabled,
                        Deleted = s.b_deleted

                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        /// <summary>
        /// Списка сайтов, доступных пользователю
        /// </summary>
        /// <returns></returns>
        public override DomainList[] getUserDomains(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sv_user_sitess.
                    Where(w => w.f_user == Id).
                    Select(s => new DomainList
                    {
                        SiteId = s.f_site,
                        DomainName = s.c_name
                    })
                    .ToArray();

                if (!data.Any()) { return null; }
                else { return data; }
            }
        }

        /// <summary>
        /// Права пользователей
        /// </summary>
        /// <returns></returns>
        public override ResolutionsModel getCmsUserResolutioInfo(Guid _userId, string _pageUrl)
        {

            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sv_resolutionss
                    .Where(w => (w.c_alias == _pageUrl && w.c_user_id == _userId))
                    .Select(s => new ResolutionsModel
                    {
                        Title = s.c_title,
                        Read = s.b_read,
                        Write = s.b_write,
                        Change = s.b_change,
                        Delete = s.b_delete
                    }).ToArray();
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        /// <summary>
        /// Смена пароля
        /// </summary>
        /// <param name="id">id аккаунта</param>
        /// <param name="NewSalt">открытый ключ</param>
        /// <param name="NewHash">закрытый ключ</param>
        /// <returns></returns>
        public override void changePasswordUser(Guid id, string NewSalt, string NewHash, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_userss.Where(w => w.id == id)
                    .Set(u => u.c_salt, NewSalt)
                    .Set(u => u.c_hash, NewHash)
                    .Update();

                insertLog(id, id, "change_pass", IP);
            }

        }


        /// <summary>
        /// Запись логов
        /// </summary>
        /// <param name="PageId"></param>
        /// <param name="UserId"></param>
        /// <param name="Action"></param>
        /// <param name="IP"></param>
        public override void insertLog(Guid PageId, Guid UserId, string Action, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                db.cms_logs.Insert(() => new cms_log
                {
                    f_page = PageId,
                    f_user = UserId,
                    d_date = DateTime.Now,
                    f_action = Action,
                    c_ip = IP
                });
            }
        }
    }
}
