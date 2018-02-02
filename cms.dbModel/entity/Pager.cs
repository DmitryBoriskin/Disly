using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class Pager
    {

        /// <summary>
        /// Текущая страница
        /// </summary>
        [Required]
        public int Page { get; set; }

        /// <summary>
        /// Кол-во эл-тов на странице
        /// </summary>
        [Required]
        public int Size { get; set; }

        /// <summary>
        /// Кол-во эл-тов
        ///// </summary>
        [Required]
        public int ItemsCount { get; set; }

        /// <summary>
        /// Кол-во страниц
        /// </summary>
        [Required]
        public int PageCount
        {
            get
            {
                return ItemsCount / Size + (ItemsCount % Size == 0 ? 0 : 1);
            }
        }
    }
}