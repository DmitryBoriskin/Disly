using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;
using System.Web;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {      
       
        public override VoteList getVoteList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrEmpty(filter.Domain))
                {
                    var query = db.content_votes
                                  .Where(w => w.f_site == filter.Domain).AsQueryable();
                    if (filter.Disabled != null)
                    {
                        query = query.Where(w => w.b_disabled == filter.Disabled);
                    }
                    if (filter.SearchText != null)
                    {
                        query = query.Where(w => (w.c_header.Contains(filter.SearchText) || w.c_text.Contains(filter.SearchText)));
                    }
                    if (filter.Date != null)
                    {
                        query = query.Where(w => w.d_date_end >= filter.Date || (w.d_date_end == null && w.d_date_start>=filter.Date));
                    }
                    if (filter.DateEnd != null) {
                        query = query.Where(w => w.d_date_start.Date <= filter.DateEnd );
                    }

                    query = query.OrderBy(o=>o.d_date_start);

                    int itemCount = query.Count();

                    var voteList = query
                            .Skip(filter.Size * (filter.Page - 1))
                            .Take(filter.Size)
                            .Select(s => new VoteModel
                            {
                                Id = s.id,
                                DateStart=s.d_date_start,
                                Header=s.c_header,
                                Disabled=s.b_disabled
                            });

                    if (voteList!=null)
                        return new VoteList
                        {
                            Data = voteList.ToArray(),
                            Pager = new Pager
                            {
                                page = filter.Page,
                                size = filter.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                            }
                        };
                }
                return null;
            }
        }        

        public override VoteModel getVoteItem(Guid id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.content_votes.Where(w => (w.id == id && w.f_site == domain))
                                        .OrderBy(o=>o.d_date_start)
                                        .Select(s=>new VoteModel {
                                            Id=s.id,
                                            Header=s.c_header,
                                            DateStart=s.d_date_start,
                                            DateEnd=s.d_date_end,
                                            Text=s.c_text,
                                            Type=s.b_type,
                                            Disabled=s.b_disabled,
                                            HisAnswer=s.b_his_answer                                            
                                        });
                if (q.Any())
                {
                    var data = q.Single();
                    data.Answer = db.content_vote_answerss
                                  .Where(w => w.f_vote == data.Id)
                                  .OrderBy(o=>o.n_sort)
                                  .Select(a=>new VoteAnswer() {
                                      id=a.id,
                                      Sort=a.n_sort,
                                      Variant=a.c_variant
                                  }).ToArray();
                    return data;
                }
                return null;
            }
        }


        public override bool insVote(Guid id, VoteModel ins,string domain)
        {
            using (var db = new CMSdb(_context))
            {
                db.content_votes
                  .Value(v => v.id, id)
                  .Value(v => v.f_site, domain)
                  .Value(v => v.c_header, ins.Header)
                  .Value(v => v.c_text, ins.Text)
                  .Value(v => v.d_date_start, ins.DateStart)
                  .Value(v => v.d_date_end, ins.DateEnd)
                  .Insert();
                // логирование
                //insertLog(userId, IP, "insert", id, String.Empty, "Banners", item.Title);
                var log = new LogModel()
                {
                    Site = _domain,
                    Section = LogSection.Vote,
                    Action = LogAction.insert,
                    PageId = id,
                    PageName = ins.Header,
                    UserId = _currentUserId,
                    IP = _ip,
                };
                insertLog(log);

                return false;
            }
        }
        public override bool updVote(Guid id, VoteModel ins)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_votes.Where(w => w.id == id);
                if (data.Any())
                {
                    data
                      .Set(s => s.c_header, ins.Header)
                      .Set(s => s.c_text, ins.Text)
                      .Set(s => s.b_disabled, ins.Disabled)
                      .Set(s => s.b_his_answer, ins.HisAnswer)
                      .Set(s => s.b_type, ins.Type)
                      .Set(s => s.d_date_start, ins.DateStart)
                      .Set(s => s.d_date_end, ins.DateEnd)
                      .Update();
                    return true;
                }                 
                else return false;
            }
        }

        public override bool delVote(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_votes.Where(w => w.id == id);
                if (data.Any())
                {
                    data.Delete();
                    return true;
                }
                return false;
            }
        }

        public override bool insAnswer(Guid idVote, string Variant)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_votes.Where(w => w.id == idVote);
                if (data.Any())
                {                    
                    var queryMaxSort = db.content_vote_answerss
                        .Where(w => w.f_vote==idVote)                            
                        .Select(s => s.n_sort);

                    int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 1;

                    db.content_vote_answerss
                        .Value(v => v.f_vote, idVote)
                        .Value(v => v.c_variant, Variant)
                        .Value(v => v.n_sort, maxSort)
                        .Insert();
                }
                return false;
            }
        }


        /// <summary>
        /// Меняем приоритет сортировки вариантов ответа отдельного опроса
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <param name="permit">Приоритет</param>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override bool permit_VoteAnswer(Guid id, Guid VoteId, int num)
        {
            using (var db = new CMSdb(_context))
            {
                //текущее значение элемента чей приоритет меняется
                var data = db.content_vote_answerss
                    .Where(w => w.f_vote.Equals(id))
                    .Select(s => new VoteAnswer
                    {                        
                        Sort = s.n_sort,
                        VoteId=s.f_vote
                    });

                if (data.Any())
                {
                    var query = data.FirstOrDefault();
                    if (num > query.Sort)
                    {
                        db.content_vote_answerss
                            .Where(w => w.f_vote.Equals(VoteId))                            
                            .Where(w => w.n_sort > query.Sort && w.n_sort <= num)
                            .Set(u => u.n_sort, u => u.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_vote_answerss
                            .Where(w => w.f_vote.Equals(VoteId))
                            .Where(w => w.n_sort < query.Sort && w.n_sort >= num)
                            .Set(u => u.n_sort, u => u.n_sort + 1)
                            .Update();
                    }
                    db.content_vote_answerss
                        .Where(w => w.id.Equals(id))
                        .Set(u => u.n_sort, num)
                        .Update();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool delVoteAnswer(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_vote_answerss.Where(w => w.id == id);
                    if (data.Any())
                    {
                        Guid VoteId = data.Single().f_vote;
                        //удаление
                        int sort = data.Single().n_sort;
                        data.Delete();
                        //корректировка приоритетов
                        db.content_vote_answerss
                          .Where(w => (w.f_vote == VoteId && w.n_sort> sort))
                          .Set(u=>u.n_sort,u=>u.n_sort-1)
                          .Update();
                    }                    
                    tran.Commit();
                }
                return true;
            }
        }
    }
}
