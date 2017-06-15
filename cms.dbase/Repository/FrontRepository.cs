using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Linq;

namespace cms.dbase
{
    public class FrontRepository : abstract_FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
        }
        public FrontRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }

        public override string getSiteId(string Domain)
        {
            using (var db = new CMSdb(_context))
            {
                string SiteId = String.Empty;

                var data = db.cms_sites_domainss.Where(w => w.c_domain == Domain).FirstOrDefault();

                SiteId = data.f_site;

                return SiteId;
            }
        }

        public override string getView(string siteId, string siteSection)
        {
            using (var db = new CMSdb(_context))
            {
                string ViewPath = "~/Error/404/";

                var data = db.front_sv_page_veiws.Where(w => w.f_site == siteId && w.f_pege_type == siteSection).FirstOrDefault();

                ViewPath = data.c_url;

                return ViewPath;
            }
        }
    }
}
