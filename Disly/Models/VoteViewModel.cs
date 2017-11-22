using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для голосование
    /// </summary>
    public class VoteViewModel : PageViewModel
    {         
        public VoteModel[] List { get; set; }        
    }
}
