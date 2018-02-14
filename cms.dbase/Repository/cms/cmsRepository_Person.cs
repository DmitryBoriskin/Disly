using cms.dbModel;
using System;
using System.Linq;
using cms.dbModel.entity.cms;
using cms.dbase.models;
using LinqToDB;
using cms.dbModel.entity;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с сотрудниками
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Получаем сотрудника
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override EmployeeModel getEmployee(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from emp in db.content_peoples
                             join l in db.content_people_postss
                             on emp.id equals l.f_people
                             join pe in db.content_employee_postss
                             on l.f_post equals pe.id
                             where emp.id.Equals(id)
                             select new { emp, pe });

                var data = query.ToArray()
                    .GroupBy(p => new { p.emp.id })
                    .Select(s => new EmployeeModel
                    {
                        Id = s.Key.id,
                        Surname = s.First().emp.c_surname,
                        Name = s.First().emp.c_name,
                        Patronymic = s.First().emp.c_patronymic,
                        Snils = s.First().emp.c_snils,
                        Photo = new Photo
                        {
                            Url = s.First().emp.c_photo
                        },
                        Posts = s.Select(d => new EmployeePostModel
                        {
                            Id = d.pe.id,
                            Parent = d.pe.n_parent,
                            Name = d.pe.c_name
                        }).ToArray()
                    });


                if (!data.Any()) return null;
                return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Обновляем сотрудника
        /// </summary>
        /// <param name="item">Сущность</param>
        /// <returns></returns>
        public override bool updateEmployee(EmployeeModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool isExist = db.content_peoples
                        .Where(w => w.id.Equals(item.Id)).Any();

                    if (isExist)
                    {
                        db.content_peoples
                            .Where(w => w.id.Equals(item.Id))
                            .Set(s => s.c_surname, item.Surname)
                            .Set(s => s.c_name, item.Name)
                            .Set(s => s.c_patronymic, item.Patronymic)
                            .Set(s => s.c_photo, item.Photo.Url)
                            .Update();

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Sites,
                            Action = LogAction.update,
                            PageId = item.Id,
                            PageName = item.Fullname,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tr.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}
