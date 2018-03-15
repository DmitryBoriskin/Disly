using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Кол-во сайтов по категориям
    /// </summary>
    public class CountSites
    {
        /// <summary>
        /// Кол-во всех сайтов
        /// </summary>
        public int CountAllSites { get; set; }

        /// <summary>
        /// Кол-во сайтов организаций
        /// </summary>
        public int CountOrgSites { get; set; }

        /// <summary>
        /// Кол-во сайтов главных специалистов
        /// </summary>
        public int CountGsSites { get; set; }

        /// <summary>
        /// Кол-во сайтов событий
        /// </summary>
        public int CountEventSites { get; set; }
    }
}
