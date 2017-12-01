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


        #region FeedBacks
        public override FeedbacksList getFeedbacksList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_feedbackss
                    .Where(w => w.f_site == _domain)
                    .OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Skip(filtr.Size * (filtr.Page - 1))
                        .Take(filtr.Size)
                        .Select(s => new FeedbackModel
                        {
                            Id = s.id,
                            Title = s.c_title,
                            Text = s.c_text,
                            Date = s.d_date,
                            SenderName = s.c_sender_name,
                            SenderEmail = s.c_sender_email,
                            Answer = s.c_answer,
                            Answerer = s.c_answerer,
                            IsNew = s.b_new,
                            Disabled = s.b_disabled
                        });

                    FeedbackModel[] eventsInfo = List.ToArray();

                    return new FeedbacksList
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
        public override FeedbackModel getFeedbackItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_feedbackss.
                    Where(w => w.id == id).
                    Select(s => new FeedbackModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        SenderName = s.c_sender_name,
                        SenderEmail = s.c_sender_email,
                        Answer = s.c_answer,
                        Answerer = s.c_answerer,
                        IsNew = s.b_new,
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        public override bool insertCmsFeedback(FeedbackModel feedback)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_feedbacks cdFeedback = db.content_feedbackss
                                                .Where(p => p.id == feedback.Id)
                                                .SingleOrDefault();
                    if (cdFeedback != null)
                    {
                        throw new Exception("Запись с таким Id уже существует");
                    }

                    cdFeedback = new content_feedbacks
                    {
                        id = feedback.Id,
                        c_title = feedback.Title,
                        c_text = feedback.Text,
                        d_date = feedback.Date,
                        c_sender_name = feedback.SenderName,
                        c_sender_email = feedback.SenderEmail,
                        c_answer = feedback.Answer,
                        c_answerer = feedback.Answerer,
                        b_new = feedback.IsNew,
                        b_disabled = feedback.Disabled,
                        f_site = _domain
                    };

                    using (var tran = db.BeginTransaction())
                    {
                        db.Insert(cdFeedback);
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
        public override bool updateCmsFeedback(FeedbackModel feedback)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_feedbacks cdFeedback = db.content_feedbackss
                                                .Where(p => p.id == feedback.Id)
                                                .SingleOrDefault();
                    if (cdFeedback == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    cdFeedback.c_title = feedback.Title;
                    cdFeedback.c_text = feedback.Text;
                    cdFeedback.c_sender_email = feedback.SenderEmail;
                    cdFeedback.c_sender_name = feedback.SenderName;
                    cdFeedback.c_answer = feedback.Answer;
                    cdFeedback.c_answerer = feedback.Answerer;
                    cdFeedback.d_date = feedback.Date;
                    cdFeedback.b_new = feedback.IsNew;
                    cdFeedback.b_disabled = feedback.Disabled;

                    using (var tran = db.BeginTransaction())
                    {
                        db.Update(cdFeedback);
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
        public override bool deleteCmsFeedback(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    content_feedbacks cdfeedback = db.content_feedbackss
                                                .Where(p => p.id == id)
                                                .SingleOrDefault();
                    if (cdfeedback == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Delete(cdfeedback);
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
