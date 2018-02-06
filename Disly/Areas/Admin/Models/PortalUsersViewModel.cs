using cms.dbModel.entity;
using cms.dbModel.entity.cms;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    public class PortalUsersViewModel : CoreViewModel
    {
        public UsersList List { get; set; }
        public UsersModel Item { get; set; }
        public Catalog_list[] GroupList { get; set; }
        public PasswordModel Password { get; set; }

        public FiltrModel Filter {get; set;}
    }

}
