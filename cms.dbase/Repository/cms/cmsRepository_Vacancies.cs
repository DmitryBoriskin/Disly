using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {

        #region Vacancies
        public override VacanciesList getVacanciesList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_vacanciess.AsQueryable();

                if(!string.IsNullOrEmpty(filtr.Domain))
                {
                    query = query.Where(w => w.f_site == _domain);
                }

                query = query.OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new VacancyModel
                        {
                            Id = s.id,
                            Profession = s.c_profession,
                            Post = s.c_post,
                            Date = s.d_date,
                            Experience = s.с_experience,
                            Сonditions = s.с_conditions,
                            Salary = s.c_salary,
                            Desc = s.c_desc,
                            Temporarily = s.b_temporarily,
                            Disabled = s.b_disabled
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    VacancyModel[] eventsInfo = List.ToArray();

                    return new VacanciesList
                    {
                        Data = eventsInfo,
                        Pager = new Pager
                        {
                            page = filtr.Page,
                            size = filtr.Size,
                            items_count = ItemCount,
                            page_count = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                        }
                    };
                }
                return null;
            }
        }
        public override VacancyModel getVacancy(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_vacanciess.
                    Where(w => w.id == id).
                    Select(s => new VacancyModel
                    {
                        Id = s.id,
                        Profession = s.c_profession,
                        Post = s.c_post,
                        Date = s.d_date,
                        Сonditions = s.с_conditions,
                        Experience = s.с_experience,
                        Salary = s.c_salary,
                        Desc = s.c_desc,
                        Temporarily = s.b_temporarily,
                        Disabled = s.b_disabled
                    });


                if (!data.Any())
                    return null;
                else
                    return data.First();
            }
        }

        public override bool insertCmsVacancy(VacancyModel vacancy)
        {
            //try
            //{
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_vacancies cdVacancy = db.content_vacanciess
                                                .Where(p => p.id == vacancy.Id)
                                                .SingleOrDefault();
                        if (cdVacancy != null)
                        {
                            throw new Exception("Запись с таким Id уже существует");
                        }

                        cdVacancy = new content_vacancies
                        {
                            id = vacancy.Id,
                            c_profession = vacancy.Profession,
                            c_post = vacancy.Post,
                            d_date = vacancy.Date,
                            с_conditions = vacancy.Сonditions,
                            с_experience = vacancy.Experience,
                            c_salary = vacancy.Salary,
                            c_desc = vacancy.Desc,
                            b_temporarily = vacancy.Temporarily,
                            b_disabled = vacancy.Disabled,
                            f_site = _domain
                        };


                        db.Insert(cdVacancy);
                        tran.Commit();
                        return true;
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    //write to log ex
            //    return false;
            //}
        }
        public override bool updateCmsVacancy(VacancyModel vacancy)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_vacancies cdVacancy = db.content_vacanciess
                                                .Where(p => p.id == vacancy.Id)
                                                .SingleOrDefault();
                    if (cdVacancy == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    cdVacancy.c_profession = vacancy.Profession;
                    cdVacancy.c_post = vacancy.Post;
                    cdVacancy.d_date = vacancy.Date;
                    cdVacancy.с_conditions = vacancy.Сonditions;
                    cdVacancy.с_experience = vacancy.Experience;
                    cdVacancy.c_salary = vacancy.Salary;
                    cdVacancy.c_desc = vacancy.Desc;
                    cdVacancy.b_temporarily = vacancy.Temporarily;
                    cdVacancy.b_disabled = vacancy.Disabled;

                    using (var tran = db.BeginTransaction())
                    {
                        db.Update(cdVacancy);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override bool deleteCmsVacancy(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_vacancies cdVacancy = db.content_vacanciess
                                                .Where(p => p.id == id)
                                                .SingleOrDefault();
                    if (cdVacancy == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Delete(cdVacancy);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }

        #endregion

    }
}
