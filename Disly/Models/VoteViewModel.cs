using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Models
{
    /// <summary>
    /// Модель для голосование
    /// </summary>
    public class VoteViewModel : PageViewModel
    {         
        public IEnumerable<VoteModel> List { get; set; }    
        
        public VoteList VoteList { get; set; }

        public VoteModel Item { get; set; }
        public List<SiteMapModel> Siblings { get; set; }
    }
}
