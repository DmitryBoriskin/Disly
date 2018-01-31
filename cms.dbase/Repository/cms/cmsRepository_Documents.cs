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
        public override DocumentsModel[] getDocuments(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_documentss.Where(w => w.f_link == id)
                    .OrderBy(o => o.n_sort)
                    .Select(s => new DocumentsModel
                    {
                        id = s.id,
                        Title = s.c_title,
                        FilePath = s.c_file_path,
                        LinkId = s.f_link
                    })

                    ;
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override bool insDocuments(DocumentsModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                var queryMaxSort = db.content_documentss
                     .Where(w => w.f_link == insert.LinkId)
                     .Select(s => s.n_sort);

                int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 1;
                
                var data = db.content_documentss
                    .Value(v => v.c_title, insert.Title)
                    .Value(v => v.c_file_path, insert.FilePath)
                    .Value(v => v.f_link, insert.LinkId)
//#warning Убрать столбец и внешний ключ 
//                    .Value(v => v.id_page, Guid.Parse("12bbcf4f-1e4f-4c9d-bb73-a11c0d024d72"))
                    .Value(v => v.n_sort, maxSort)
                    .Insert();
                return true;
            }
        }

        public override DocumentsModel getDocumentsPath(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_documentss.Where(w => w.id == id)
                    .Select(s => new DocumentsModel
                    {
                        FilePath = s.c_file_path
                    });
                if (!data.Any()) { return null; }
                else { return data.FirstOrDefault(); }
            }
        }

        public override bool deleteSiteMapDocuments(Guid id)
        {
            using (var db = new CMSdb(_context))
            {

                var data = db.content_documentss.Where(w => w.id == id);
                if (data != null)
                {
                    //смещение пермитов
                    var ThisPageId = data.FirstOrDefault().f_link;
                    var ThisPermit = data.FirstOrDefault().n_sort;

                    var query = db.content_documentss.Where(w => w.f_link== ThisPageId && w.n_sort > ThisPermit);
                    if (query.Any())
                    {
                        query.Set(p => p.n_sort, p => p.n_sort - 1).Update();
                    }

                    data.Where(w => w.id == id).Delete();

                    return true;
                }
                else return false;
            }
        }


        public override bool permit_Documents(Guid id, int num)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var query = db.content_documentss
                        .Where(w => w.id == id);

                    if (!query.Any())
                        return false;

                    var data = query
                        .Select(s => new DocumentsModel()
                        {
                            LinkId = s.f_link,
                            Permit = s.n_sort
                        })
                        .First();

                    var PageId = data.LinkId;

                    if (num > data.Permit)
                    {
                        db.content_documentss
                            .Where(w => w.f_link== PageId)
                            .Where(w => w.n_sort > data.Permit)
                            .Where(w => w.n_sort <= num)
                            .Set(p => p.n_sort, p => p.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_documentss
                            .Where(w => w.f_link == PageId)
                            .Where(w => w.n_sort < data.Permit)
                            .Where(w => w.n_sort >= num)
                            .Set(p => p.n_sort, p => p.n_sort + 1)
                            .Update();
                    }

                    db.content_documentss
                        .Where(w => w.id == id)
                        .Set(s => s.n_sort, num)
                        .Update();
                }
                return true;
            }
        }

    }
}
