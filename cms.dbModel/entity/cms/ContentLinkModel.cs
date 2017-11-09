using System;

namespace cms.dbModel.entity
{
     public class ContentLinkModel //: CoreViewModel
     {
        /// <summary>
        /// Id Объкта, который привязываем (Новости, События и тд) 
        /// </summary>
        public Guid ObjctId { get; set; }
        /// <summary>
        /// Тип Объкта, который привязываем (Новости, События и тд)
        /// </summary>
        public ContentType ObjctType { get; set; }

        /// <summary>
        /// Объекты, к которыму привязываем
        /// </summary>
        public Guid[] LinksId { get; set; }
        /// <summary>
        /// Тип объекта, к которому привязываем
        /// </summary>
        public ContentLinkType LinkType { get; set; }

    }
 }