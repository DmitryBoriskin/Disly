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
        public VoteModel Item { get; set; }
    }
}
