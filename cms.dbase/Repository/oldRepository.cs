using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Linq;

namespace cms.dbase
{
    public class AccountRepository : abstract_AccountRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        /// <summary>
        /// Конструктор
        /// </summary>
        public AccountRepository()
        {
            _context = "defaultConnection";
        }
        public AccountRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }

        // ------------------------ Для CMS --------------------------
        #region Для CMS

        public override cmsLogModel[] getCmsUserLog(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Logs.
                    Where(w => w.Id == Id).
                    Select(s => new cmsLogModel
                    {
                        PageId = s.Id,
                        UserId = s.F_UserId,
                        Date = s.D_Date,
                        Action = s.ActionName
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override cmsLogModel[] getCmsPageLog(Guid PageId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Logs.
                    Where(w => w.F_PageId == PageId).
                    Select(s => new cmsLogModel
                    {
                        PageId = s.Id,
                        UserId = s.F_UserId,
                        Date = s.D_Date,
                        Surname = s.C_Surname,
                        Name = s.C_Name,
                        Action = s.ActionName
                    })
                    .OrderByDescending(o => o.Date);

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override void insertLog(Guid PageId, Guid UserId, string Action, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                db.cms_Logs.Insert(() => new cms_Log
                {
                    F_PageId = PageId,
                    F_UserId = UserId,
                    D_Date = DateTime.Now,
                    C_Action = Action,
                    C_IP = IP
                });
            }
        }

        #region CmsMenu
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override cmsMenuModel[] getCmsMenu(string SectionId, Guid UserId)
        {
            using (var db = new CMSdb(_context))
            {                
                var data = db.SV_cms_Resolutionss.
                    Where(w => (w.C_Group == SectionId && w.B_Read == true && w.C_UserId== UserId)).
                    Select(s => new cmsMenuModel
                    {
                        id = s.C_MenuId,
                        Permit = s.N_Permit,
                        Alias = s.C_Alias,
                        Url = s.C_Url,
                        Title = s.C_Title,
                        Class = s.C_Class,
                        Group = s.C_Group
                    }).
                    OrderBy(o => (o.Permit));

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }

        }
        public override cmsMenuModel getCmsMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_Menus.
                    Where(w => w.Id == id).
                    Select(s => new cmsMenuModel
                    {
                        id = s.Id,
                        Permit = s.N_Permit,
                        Alias = s.C_Alias,
                        Url = s.C_Url,
                        Title = s.C_Title,
                        Desc = s.C_Desc,
                        Class = s.C_Class,
                        Group = s.C_Group
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool updateCmsMenu(Guid id, cmsMenuModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_Menus.Where(w => w.Id == id);

                if (data != null)
                {
                    data.Where(w => w.Id == id)
                    .Set(p => p.C_Title, Item.Title)
                    .Set(p => p.C_Alias, Item.Alias)
                    .Set(p => p.C_Url, Item.Url)
                    .Set(p => p.C_Class, Item.Class)
                    .Set(p => p.C_Group, Item.Group)
                    .Set(p => p.C_Desc, Item.Desc)
                    .Update();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool createCmsMenu(Guid id, cmsMenuModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                int Permit = 0;
                Permit = db.cms_Menus.Where(w => w.C_Group == Item.Group).Select(s => s.N_Permit).Count() + 1;

                db.cms_Menus
                    .Value(p => p.Id, id)
                    .Value(p => p.N_Permit, Permit)
                    .Value(p => p.C_Title, Item.Title)
                    .Value(p => p.C_Alias, Item.Alias)
                    .Value(p => p.C_Url, Item.Url)
                    .Value(p => p.C_Class, Item.Class)
                    .Value(p => p.C_Group, Item.Group)
                    .Value(p => p.C_Desc, Item.Desc)
                   .Insert();

                //добавить права группе
                //добавить права группы пользователей
                Guid Dev_users = Guid.Parse("00000000-0000-0000-0000-000000000000");
                var data_group = db.cms_UsersGroups.Where(w => w.C_Alias != "Developer").Select(s => new cms_UsersGroup { id = s.id,C_Alias=s.C_Alias }).ToArray();//спсиок групп
                var data_group_develop = db.cms_UsersGroups.Where(w=>w.C_Alias== "Developer").Select(s => new cms_UsersGroup { id = s.id, C_Alias = s.C_Alias }).ToArray();//спсиок групп
                var data_users = db.cms_Userss.Where(w=>w.Id!= Dev_users).Select(s => new cms_Users { Id = s.Id }).ToArray();//спсиок пользователей
                var data_users_develop = db.cms_Userss.Where(w => w.Id == Dev_users).Select(s => new cms_Users { Id = s.Id }).ToArray();//разработчик системная учетная запись

                //добавление прав в группы и 
                foreach(cms_UsersGroup s in data_group)
                {
                    db.cms_ResolutionsTemplatess
                        .Value(v=>v.F_MenuId, id)
                        .Value(v => v.F_UserGroupId, s.C_Alias)
                        .Value(v => v.B_Read, false)
                        .Value(v => v.B_Write, false)
                        .Value(v => v.B_Change, false)
                        .Value(v => v.B_Delete, false)                        
                        .Insert();
                }
                foreach (cms_UsersGroup s in data_group_develop)
                {
                    db.cms_ResolutionsTemplatess
                        .Value(v => v.F_MenuId, id)
                        .Value(v => v.F_UserGroupId, s.C_Alias)
                        .Value(v => v.B_Read, true)
                        .Value(v => v.B_Write, true)
                        .Value(v => v.B_Change, true)
                        .Value(v => v.B_Delete, true)
                        .Insert();
                }
                //добавить прав пользователям
                foreach (cms_Users s in data_users)
                {
                    db.cms_Resolutionss
                        .Value(v => v.C_MenuId, id)
                        .Value(v => v.C_UserId, s.Id)                        
                        .Value(v => v.B_Read, false)
                        .Value(v => v.B_Write, false)
                        .Value(v => v.B_Change, false)
                        .Value(v => v.B_Delete, false)
                        .Value(v => v.B_Importent, false)
                        .Insert();
                }
                foreach (cms_Users s in data_users_develop)
                {
                    db.cms_Resolutionss
                        .Value(v => v.C_MenuId, id)
                        .Value(v => v.C_UserId, s.Id)
                        .Value(v => v.B_Read, true)
                        .Value(v => v.B_Write, true)
                        .Value(v => v.B_Change, true)
                        .Value(v => v.B_Delete, true)
                        .Value(v => v.B_Importent, false)
                        .Insert();
                }

                return true;
            }
        }
        public override bool deleteCmsMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                int Num = db.cms_Menus.Where(w => w.Id == id).ToArray().First().N_Permit;
                string Group = db.cms_Menus.Where(w => w.Id == id).ToArray().First().C_Group;
                
                db.cms_Menus
                    .Where(w => w.N_Permit > Num)
                    .Where(w => w.C_Group == Group)
                    .Set(p => p.N_Permit, p => p.N_Permit - 1)
                    .Update();

                db.cms_Menus.Where(w => w.Id == id).Delete();
                return true;
            }
        }
        public override bool permit_cmsMenu(Guid id, int num)
        {
            using (var db = new CMSdb(_context))
            {
                db.cmsMenu_ChangePermit(id, num).ToArray();
                return true;
            }
        }
        #endregion

        #region SectionsGroup (SectionsGroupItem)
        /// <summary>
        /// Фильтр
        /// </summary>
        /// <returns></returns>
        public override SectionGroupModel[] getSectionsGroup(string SectionId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroups
                    .Where(w => w.C_SectionId == SectionId)
                    .Select(s => new SectionGroupModel
                    {
                        Guid = s.C_Guid,
                        Alias = s.C_Alias,
                        Title = s.C_Title,
                        SectionId = s.C_SectionId,
                        Permit = s.N_Permit,
                        ReadOnly = s.B_ReadOnly,
                        Filtr = s.B_Filtr,
                        Items = getSectionsGroupItem(s.C_SectionId, s.C_Alias)
                    })
                    .OrderBy(o => o.Permit)
                    .ToArray();
                if (!data.Any()) { return null; }
                else { return data; }
            }
        }
        public override SectionGroupModel getSectionGroup(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroups
                    .Where(w => w.C_Guid == Id)
                    .Select(s => new SectionGroupModel
                    {
                        Guid = s.C_Guid,
                        Alias = s.C_Alias,
                        Title = s.C_Title,
                        SectionId = s.C_SectionId,
                        Permit = s.N_Permit,
                        ReadOnly = s.B_ReadOnly,
                        Filtr = s.B_Filtr
                    }).First();
                if (data == null) { return null; }
                else { return data; }
            }
        }
        public override bool createSectionGroup(Guid Id, SectionGroupModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                int Permit = 0;
                Permit = db.cms_SectionsGroups.Where(w => w.C_SectionId == Item.SectionId).Select(s => s.N_Permit).Count() + 1;

                try
                {
                    db.cms_SectionsGroups
                        .Value(p => p.C_Guid, Id)
                        .Value(p => p.C_SectionId, Item.SectionId)
                        .Value(p => p.C_Alias, Item.Alias)
                        .Value(p => p.C_Title, Item.Title)
                        .Value(p => p.N_Permit, Permit)
                        .Value(p => p.B_ReadOnly, Item.ReadOnly)
                        .Value(p => p.B_Filtr, Item.Filtr)
                       .Insert();

                    return true;
                }
                catch { return false; }
            }
        }
        public override bool updateSectionGroup(string Alias, SectionGroupModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroups.Where(w => w.C_Alias == Alias);

                try
                {
                    if (data != null)
                    {
                        data
                            .Set(p => p.C_SectionId, Item.SectionId)
                            .Set(p => p.C_Alias, Item.Alias)
                            .Set(p => p.C_Title, Item.Title)
                            .Set(p => p.N_Permit, Item.Permit)
                            .Set(p => p.B_ReadOnly, Item.ReadOnly)
                            .Set(p => p.B_Filtr, Item.Filtr)
                            .Update();
                        return true;
                    }
                    else return false;
                }
                catch { return false; }
            }
        }
        public override bool deleteSectionGroup(string Alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroups.Where(w => w.C_Alias == Alias).Select(s => new SectionGroupModel { Alias = s.C_Alias, SectionId = s.C_SectionId, Permit = s.N_Permit }).First();

                int Children = 0;
                Children = db.cms_SectionsGroupItemss
                    .Where(w => w.F_SectionId == data.SectionId)
                    .Where(w => w.F_GroupId == data.Alias)
                    .Count();

                if (Children == 0)
                {
                    db.cms_SectionsGroups
                        .Where(w => w.C_SectionId == data.SectionId)
                        .Where(w => w.N_Permit > data.Permit)
                        .Set(p => p.N_Permit, p => p.N_Permit - 1)
                        .Update();

                    db.cms_SectionsGroups.Where(w => w.C_Alias == Alias).Delete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool permit_SectionGroup(Guid Id, int num)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroups.Where(w => w.C_Guid == Id).Select(s => new SectionGroupModel { SectionId = s.C_SectionId, Permit = s.N_Permit }).First();

                if (num > data.Permit)
                {
                    db.cms_SectionsGroups
                        .Where(w => w.C_SectionId == data.SectionId)
                        .Where(w => w.N_Permit > data.Permit)
                        .Where(w => w.N_Permit <= num)
                        .Set(p => p.N_Permit, p => p.N_Permit - 1)
                        .Update();
                }
                else if (num < data.Permit)
                {
                    db.cms_SectionsGroups
                        .Where(w => w.C_SectionId == data.SectionId)
                        .Where(w => w.N_Permit < data.Permit)
                        .Where(w => w.N_Permit >= num)
                        .Set(p => p.N_Permit, p => p.N_Permit + 1)
                        .Update();
                }

                db.cms_SectionsGroups
                    .Where(w => w.C_Guid == Id)
                    .Set(p => p.N_Permit, num)
                    .Update();

                return true;
            }
        }

        /// <summary>
        /// Фильтр (разделы в секцииях)
        /// </summary>
        /// <param name="ItemId">id для получения одной записи</param>
        /// <returns></returns>
        public override SectionGroupItemsModel[] getSectionsGroupItem(string sectionId, string groupId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroupItemss
                    .Where(w => w.F_SectionId == sectionId && w.F_GroupId == groupId)
                    .Select(s => new SectionGroupItemsModel
                    {
                        Guid = s.C_Guid,
                        Title = s.C_Title,
                        Alias = s.C_Alias,
                        Desc = s.C_Desc,
                        Logo = s.C_Logo,
                        SectionId = s.F_SectionId,
                        GroupId = s.F_GroupId,
                        Permit = s.N_Permit
                    })
                    .OrderBy(o => o.Permit)
                    .ToArray();

                if (data.Any()) { return data; }
                else { return null; }
            }
        }
        public override SectionGroupItemsModel getSectionGroupItem(Guid ItemId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroupItemss
                    .Where(w => w.C_Guid == ItemId)
                    .Select(s => new SectionGroupItemsModel
                    {
                        Guid = s.C_Guid,
                        Permit = s.N_Permit,
                        Title = s.C_Title,
                        Alias = s.C_Alias,
                        Desc = s.C_Desc,
                        Logo = s.C_Logo,
                        SectionId = s.F_SectionId,
                        GroupId = s.F_GroupId
                    }).First();
                if (data == null) { return null; }
                else { return data; }
            }
        }
        public override bool createSectionGroupItem(Guid id, SectionGroupItemsModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                int Permit = 0;
                Permit = db.cms_SectionsGroupItemss
                    .Where(w => w.F_SectionId == Item.SectionId && w.F_GroupId == Item.GroupId)
                    .Select(s => s.N_Permit).Count() + 1;
                try
                {
                    db.cms_SectionsGroupItemss
                        .Value(p => p.C_Guid, id)
                        .Value(p => p.N_Permit, Permit)
                        .Value(p => p.C_Title, Item.Title)
                        .Value(p => p.C_Alias, Item.Alias)
                        .Value(p => p.C_Desc, Item.Desc)
                        .Value(p => p.C_Logo, Item.Logo)
                        .Value(p => p.F_SectionId, Item.SectionId)
                        .Value(p => p.F_GroupId, Item.GroupId)
                       .Insert();

                    return true;
                }
                catch { return false; }
            }
        }
        public override bool updateSectionGroupItem(Guid id, SectionGroupItemsModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroupItemss.Where(w => w.C_Guid == id);

                try
                {
                    if (data != null)
                    {
                        data
                            .Set(p => p.C_Guid, id)
                            .Set(p => p.C_Title, Item.Title)
                            .Set(p => p.C_Alias, Item.Alias)
                            .Set(p => p.C_Desc, Item.Desc)
                            .Set(p => p.C_Logo, Item.Logo)
                            .Set(p => p.F_SectionId, Item.SectionId)
                            .Set(p => p.F_GroupId, Item.GroupId)
                           .Update();
                        return true;
                    }
                    else return false;
                }
                catch { return false; }
            }
        }
        public override bool deleteSectionGroupItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    db.cms_SectionsGroupItemss.Where(w => w.C_Guid == id).Delete();
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool permit_SectionGroupItem(Guid Id, int num)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_SectionsGroupItemss.Where(w => w.C_Guid == Id).Select(s => new SectionGroupItemsModel { SectionId = s.F_SectionId, GroupId = s.F_GroupId, Permit = s.N_Permit }).First();

                if (num > data.Permit)
                {
                    db.cms_SectionsGroupItemss
                        .Where(w => w.F_SectionId == data.SectionId)
                        .Where(w => w.F_GroupId == data.GroupId)
                        .Where(w => w.N_Permit > data.Permit)
                        .Where(w => w.N_Permit <= num)
                        .Set(p => p.N_Permit, p => p.N_Permit - 1)
                        .Update();
                }
                else if (num < data.Permit)
                {
                    db.cms_SectionsGroupItemss
                        .Where(w => w.F_SectionId == data.SectionId)
                        .Where(w => w.F_GroupId == data.GroupId)
                        .Where(w => w.N_Permit < data.Permit)
                        .Where(w => w.N_Permit >= num)
                        .Set(p => p.N_Permit, p => p.N_Permit + 1)
                        .Update();
                
                }

                db.cms_SectionsGroupItemss
                    .Where(w => w.C_Guid == Id)
                    .Set(p => p.N_Permit, num)
                    .Update();

                return true;
            }
        }
        #endregion

        #region карта сайта
        /// <summary>
        /// карта сайта
        /// </summary>
        /// <returns></returns>
        public override cmsSiteMapModel getCmsSiteMap(string SiteId, Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SiteMaps.
                    Where(w => w.F_Site == SiteId && w.Id == id).
                    Select(s => new cmsSiteMapModel
                    {
                        id = s.Id,                        
                        View = ((Guid)s.F_ViewId!=null)?(Guid)s.F_ViewId:Guid.Parse("2E455534-6287-43E5-9BE5-B5F5680B4ABF"),
                        Type = s.F_Type,
                        Menu=s.F_Menu,
                        Permit = s.N_Permit,
                        Alias = s.C_Alias,
                        Path = s.C_Path,
                        Title = s.C_Title,
                        Logo = s.C_Logo,
                        File = s.C_File,
                        Url = s.C_Url,
                        Text = s.C_Text,
                        Desc = s.C_Desc,
                        Keyw = s.C_Keyw,
                        Disabled = s.B_Disabled
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override cmsSiteMapModel[] getCmsSiteMap(string SiteId, string Url)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SiteMaps.
                    Where(w => w.F_Site == SiteId && w.C_Path == Url).
                    OrderBy(p => p.N_Permit).
                    Select(s => new cmsSiteMapModel
                    {
                        id = s.Id,
                        //View=s.F_ViewId,
                        Type = s.F_Type,
                        Permit = s.N_Permit,
                        Menu=s.F_Menu,
                        Alias = s.C_Alias,
                        Path = s.C_Path,
                        Title = s.C_Title,
                        Logo = s.C_Logo,
                        File = s.C_File,
                        Url = s.C_Url,
                        Text = s.C_Text,
                        Desc = s.C_Desc,
                        Keyw = s.C_Keyw,
                        Disabled = s.B_Disabled
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override bool insCmsSiteMap(Guid id, string path, cmsSiteMapModel insert)
        {
            int MaxPermit = 0;
            using (var db = new CMSdb(_context))
            {
                try { MaxPermit = (from s in db.main_SiteMaps where s.C_Path == path select s.N_Permit).Max(); }
                catch { }
                db.main_SiteMaps
                    .Value(p => p.Id, id)
                    .Value(p => p.F_Site, insert.Site)
                    .Value(p => p.C_Title, insert.Title)
                    .Value(p => p.C_Alias, insert.Alias)
                    .Value(p => p.C_Logo, insert.Logo)
                    .Value(p => p.C_Path, path.ToLower())
                    .Value(p => p.F_Menu, insert.Menu)
                    .Value(p => p.N_Permit, MaxPermit + 1)
                    .Value(p => p.C_Text, insert.Text)
                    .Value(p => p.F_Type, insert.Type)
                    .Value(p => p.C_Keyw, insert.Text)
                    .Value(p => p.C_Desc, insert.Desc)
                    .Value(p => p.B_Disabled, insert.Disabled)
                    .Insert();
                return true;
            }
        }
        public override bool setCmsSiteMap(Guid id, cmsSiteMapModel update)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SiteMaps.Where(w => w.Id == id );
                string oldPath = data.Select(p => p.C_Path + p.C_Alias + "/").FirstOrDefault().ToString();
                string newPath = data.Select(p => p.C_Path + update.Alias + "/").FirstOrDefault().ToString();

                var datachild = db.main_SiteMaps.Where(w => w.C_Path.Contains(oldPath) && w.F_Site == update.Site).Select(s=>new cmsSiteMapModel { id=s.Id,Path=s.C_Path}).ToArray();

                //изьменение дочерних путей
                if (datachild != null)
                {
                   foreach(cmsSiteMapModel s in datachild)
                    {
                        db.main_SiteMaps
                        .Where(w => w.Id == s.id)
                        .Set(p => p.C_Path, s.Path.ToLower().Replace(oldPath.ToLower(), newPath.ToLower()))
                        .Update();                                                
                    }
                }

                data = db.main_SiteMaps.Where(w => w.Id == id);

                if (data != null)
                {
                    data
                    .Where(w => w.Id == id)
                    .Set(p => p.C_Title, update.Title)
                    .Set(p => p.C_Alias, update.Alias)
                    .Set(p => p.C_Logo, update.Logo)
                    .Set(p => p.F_Menu, update.Menu)
                    .Set(p => p.F_ViewId, update.View)
                    .Set(p => p.C_Url, update.Url)
                    .Set(p => p.C_Text, update.Text)
                    .Set(p => p.F_Type, update.Type)
                    .Set(p => p.C_Keyw, update.Keyw)
                    .Set(p => p.C_Desc, update.Desc)
                    .Set(p => p.B_Disabled, update.Disabled)
                    .Update();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool delCmsSiteMap(string SiteId, Guid id)
        {
            string ChildPath = string.Empty;
            using (var db = new CMSdb(_context))
            {
                try { ChildPath = (from s in db.main_SiteMaps where s.F_Site == SiteId && s.Id == id select s.C_Path + s.C_Alias + '/').FirstOrDefault(); }
                catch { }

                var data = db.main_SiteMaps;
                if (db.main_SiteMaps.Where(w => w.F_Site == SiteId && w.Id == id) != null)
                {
                    data
                        .Where(w => w.F_Site == SiteId && (w.Id == id || w.C_Path.Contains(ChildPath)))
                        .Delete();
                    return true;
                }
                else return false;
            }
        }
        public override cmsSiteMapTypeModel[] getSiteMapType()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SiteMapTypes.
                    Select(s => new cmsSiteMapTypeModel
                    {
                        TypeName = s.C_TypeName,
                        Alias = s.C_Alias
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        #endregion

        /// <summary>
        /// Поиск по новостям
        /// </summary>
        /// <returns></returns>
        public override cmsMaterialsModel[] getSearchCmsMaterial(string SiteId, string SearchLine, DateTime? Begin, DateTime? End, string category, string type)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Materialss
                    .OrderByDescending(p => p.D_Date)
                    .Where(w => w.F_Site == SiteId 
                        && (w.D_Date >= Begin || Begin == null)
                        && (w.D_Date <= End || End == null)
                        && (SearchLine == null || w.C_Title.ToLower().Contains(SearchLine))
                        && (category == null || w.F_Category.ToLower().Contains(category))
                        && (type == null || w.F_Type.ToLower().Contains(category))
                        )
                    .Select(s => new cmsMaterialsModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Alias = s.C_Alias,
                        Keyw = s.C_Keyw,
                        Desc = s.C_Desc,
                        Date = s.D_Date,
                        Year = s.N_Year,
                        Month = s.N_Month,
                        Day = s.N_Day,
                        Text = s.C_Text,
                        Photo = s.C_Photo,
                        Type = s.F_Type,
                        Category = s.F_Category,
                        Disabled = s.B_Disabled,
                        UrlName = s.C_UrlName,
                        Url = s.C_Url
                    }).ToArray();
                return data;
            }
        }

        public override cmsMaterialsModel[] getMaterials(string SiteId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Materialss
                    .OrderByDescending(p => p.D_Date)
                    .Where(w => w.F_Site == SiteId && w.B_Disabled == false)
                    .Select(s => new cmsMaterialsModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Alias = s.C_Alias,
                        Keyw = s.C_Keyw,
                        Desc = s.C_Desc,
                        Date = s.D_Date,
                        Year = s.N_Year,
                        Month = s.N_Month,
                        Day = s.N_Day,
                        Text = s.C_Text,
                        Photo = s.C_Photo,
                        Type = s.F_Type,
                        Category = s.F_Category,
                        UrlName = s.C_UrlName,
                        Url = s.C_Url
                    }).ToArray();
                return data;
            }
        }

        public override cmsMaterialsModel getMaterialsItem(string Path,string SiteId)
        {
            using (var db = new CMSdb(_context))
            {
                try {
                    String[] _Path = Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    string Year = _Path[1];
                    string Month = _Path[2];
                    string Day = _Path[3];
                    string Alias = _Path[4];
                    var data = db.main_Materialss
                        .Where(w => w.F_Site == SiteId && w.N_Year == Year && w.N_Month == Month && w.N_Day == Day && w.C_Alias == Alias)
                        .Select(s => new cmsMaterialsModel
                        {
                            id = s.id,
                            Title = s.C_Title,
                            Text = s.C_Text,
                            Alias = s.C_Alias,
                            Keyw = s.C_Keyw,
                            Desc = s.C_Desc,
                            Date = s.D_Date,
                            Year = s.N_Year,
                            Month = s.N_Month,
                            Day = s.N_Day,
                            Photo = s.C_Photo,
                            Video = s.C_Video,
                            Type = s.F_Type,
                            Category = s.F_Category,
                            Disabled = s.B_Disabled,
                            UrlName = s.C_UrlName,
                            Url = s.C_Url
                        });

                    if (!data.Any()) { return null; }
                    else { return data.First(); }
                }
                catch { return null; }
               
            }
        }

        public override cmsMaterialsModel getCmsMaterial(string SiteId, Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Materialss
                    .Where(w => w.F_Site == SiteId && w.id == id)
                    .Select(s => new cmsMaterialsModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Text=s.C_Text,
                        Alias = s.C_Alias,
                        Keyw = s.C_Keyw,
                        Desc = s.C_Desc,
                        Date = s.D_Date,
                        Year = s.N_Year,
                        Month = s.N_Month,
                        Day = s.N_Day,
                        Photo = s.C_Photo,
                        Video = s.C_Video,
                        Type = s.F_Type,
                        Category = s.F_Category,
                        Disabled = s.B_Disabled,
                        UrlName = s.C_UrlName,
                        Url = s.C_Url
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }

            }
        }
        public override bool insCmsMaterials(Guid id, cmsMaterialsModel insert)
        {
            using (var db = new CMSdb(_context))
            {                            
                db.main_Materialss
                    .Value(v => v.id, id)
                    .Value(p => p.F_Site, insert.Site)
                    .Value(v => v.C_Title, insert.Title)
                    .Value(v => v.C_Alias, insert.Alias)
                    .Value(v => v.C_Keyw, insert.Keyw)
                    .Value(v => v.C_Desc, insert.Desc)
                    .Value(v => v.D_Date, insert.Date)
                    .Value(v => v.N_Year, insert.Year)
                    .Value(v => v.N_Month, insert.Month)
                    .Value(v => v.N_Day, insert.Day)
                    .Value(v => v.C_Text, insert.Text)
                    .Value(v => v.C_Photo, insert.Photo)
                    .Value(v => v.C_Video, insert.Video)
                    .Value(v => v.F_Type, insert.Type)
                    .Value(v => v.F_Category, insert.Category)
                    .Value(v => v.B_Disabled, insert.Disabled)
                    .Value(v => v.C_Url, insert.Url)
                    .Value(v => v.C_UrlName, insert.UrlName)                    
                    .Insert();
                return true;
            }
        }
        public override bool setCmsMaterials(Guid id, cmsMaterialsModel update)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Materialss.Where(w => w.id == id);
                if (data != null)
                {
                    data
                    .Where(w => w.id == id)
                    .Set(u => u.C_Title, update.Title)
                    .Set(u => u.C_Alias, update.Alias)
                    .Set(u => u.C_Keyw, update.Keyw)
                    .Set(u => u.C_Desc, update.Desc)
                    .Set(u => u.D_Date, update.Date)
                    .Set(u => u.C_Text, update.Text)
                    .Set(u => u.C_Photo, update.Photo)
                    .Set(u => u.C_Video, update.Video)
                    .Set(u => u.F_Type, update.Type)
                    .Set(u => u.F_Category, update.Category)
                    .Set(u => u.B_Disabled, update.Disabled)
                    .Set(u => u.C_UrlName, update.UrlName)
                    .Set(u => u.C_Url, update.Url)
                    .Update();
                    return true;
                }
                else return false;
            }
        }
        public override bool delCmsMaterials(string SiteId, Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Materialss;
                if (data.Where(w => w.F_Site == SiteId && w.id == id) != null)
                {
                    data
                        .Where(w => w.F_Site == SiteId && w.id == id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }


        /// <summary>
        /// События (афиша)
        /// </summary>
        /// <returns></returns>
        public override PlaceCardModel[] getCmsPlaceCards(string SiteId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PlaceCards
                    .OrderByDescending(p => p.D_DateStart)
                    .Where(w => w.F_Site == SiteId)
                    .Select(s => new PlaceCardModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Alias = s.C_Alias,
                        Site = s.F_Site,
                        Keyw = s.C_Keyw,
                        Desc = s.C_Desc,
                        DateStart = s.D_DateStart,
                        DateEnd = s.D_DateEnd,
                        Year = s.N_Year,
                        Month = s.N_Month,
                        Day = s.N_Day,
                        Text = s.C_Text,
                        Photo = s.C_Photo,
                        Video = s.C_Video,
                        Place = s.C_Place,
                        Url = s.C_Url,
                        UrlName = s.C_UrlName,
                        Disabled = s.B_Disabled
                    }).ToArray();
                return data;
            }
        }

        public override PlaceCardModel[] getPlaceCards(string SiteId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PlaceCards
                    .OrderByDescending(p => p.D_DateStart)
                    .Where(w => w.F_Site == SiteId && w.B_Disabled == false)
                    .Select(s => new PlaceCardModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Alias = s.C_Alias,
                        Site = s.F_Site,
                        Keyw = s.C_Keyw,
                        Desc = s.C_Desc,
                        DateStart = s.D_DateStart,
                        DateEnd = s.D_DateEnd,
                        Year = s.N_Year,
                        Month = s.N_Month,
                        Day = s.N_Day,
                        Text = s.C_Text,
                        Photo = s.C_Photo,
                        Video = s.C_Video,
                        Place = s.C_Place,
                        Url = s.C_Url,
                        UrlName = s.C_UrlName
                    }).ToArray();
                return data;
            }
        }

        public override PlaceCardModel getPlaceCard(string SiteId, Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PlaceCards
                    .Where(w => w.F_Site == SiteId && w.id == id)
                    .Select(s => new PlaceCardModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Alias = s.C_Alias,
                        Site = s.F_Site,
                        Keyw = s.C_Keyw,
                        Desc = s.C_Desc,
                        DateStart = s.D_DateStart,
                        DateEnd = s.D_DateEnd,
                        Year = s.N_Year,
                        Month = s.N_Month,
                        Day = s.N_Day,
                        Text = s.C_Text,
                        Photo = s.C_Photo,
                        Video = s.C_Video,
                        Place = s.C_Place,
                        Url = s.C_Url,
                        UrlName = s.C_UrlName,
                        Disabled = s.B_Disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }

            }
        }
        public override bool createPlaceCard(Guid id, PlaceCardModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                db.main_PlaceCards
                    .Value(v => v.id, id)
                    .Value(v => v.C_Title, insert.Title)
                    .Value(v => v.C_Alias, insert.Alias)
                    .Value(p => p.F_Site, insert.Site)
                    .Value(v => v.C_Keyw, insert.Keyw)
                    .Value(v => v.C_Desc, insert.Desc)
                    .Value(v => v.D_DateStart, insert.DateStart)
                    .Value(v => v.D_DateEnd, insert.DateEnd)
                    .Value(v => v.N_Year, insert.Year)
                    .Value(v => v.N_Month, insert.Month)
                    .Value(v => v.N_Day, insert.Day)
                    .Value(v => v.C_Text, insert.Text)
                    .Value(v => v.C_Photo, insert.Photo)
                    .Value(v => v.C_Video, insert.Video)
                    .Value(v => v.C_Place, insert.Place)
                    .Value(v => v.C_Url, insert.Url)
                    .Value(v => v.C_UrlName, insert.UrlName)
                    .Value(v => v.B_Disabled, insert.Disabled)
                    .Insert();
                return true;
            }
        }
        public override bool updatePlaceCard(Guid id, PlaceCardModel update)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PlaceCards.Where(w => w.id == id);
                if (data != null)
                {
                    data
                    .Where(w => w.id == id)
                    .Set(u => u.C_Title, update.Title)
                    .Set(u => u.C_Alias, update.Alias)
                    .Set(u => u.F_Site, update.Site)
                    .Set(u => u.C_Keyw, update.Keyw)
                    .Set(u => u.C_Desc, update.Desc)
                    .Set(u => u.D_DateStart, update.DateStart)
                    .Set(u => u.D_DateEnd, update.DateEnd)
                    .Set(u => u.C_Text, update.Text)
                    .Set(u => u.C_Photo, update.Photo)
                    .Set(u => u.C_Video, update.Video)
                    .Set(u => u.C_Place, update.Place)
                    .Set(u => u.C_Url, update.Url)
                    .Set(u => u.C_UrlName, update.UrlName)
                    .Set(u => u.B_Disabled, update.Disabled)
                    .Update();
                    return true;
                }
                else return false;
            }
        }
        public override bool deletePlaceCard(string SiteId, Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PlaceCards;
                if (data.Where(w => w.F_Site == SiteId && w.id == id) != null)
                {
                    data
                        .Where(w => w.F_Site == SiteId && w.id == id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// Представления
        /// </summary>
        /// <returns></returns>
        public override cmsPageViewsModel[] getCmsPageViews()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_PageViewss.
                    Select(s => new cmsPageViewsModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Url = s.C_Url,
                        ReadOnly = s.B_ReadOnly
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override cmsSiteMapViewsModel[] getCmsPageViewsSelec()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_PageViewss.
                    Select(s => new cmsSiteMapViewsModel
                    {
                        id = s.id,
                        Title = s.C_Title                        
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }



        public override cmsPageViewsModel getCmsPageViews(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_PageViewss
                    .Where(w => w.id == id)
                    .Select(s => new cmsPageViewsModel
                    {
                        id = s.id,
                        Title = s.C_Title,
                        Url = s.C_Url,
                        ReadOnly = s.B_ReadOnly
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool setCmsPageViews(Guid id, cmsPageViewsModel update)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_PageViewss.Where(w => w.id == id);
                if (data != null)
                {
                    data
                        .Where(w => w.id == id)
                        .Set(u => u.C_Title, update.Title)
                        .Set(u => u.C_Url, update.Url)
                        .Set(u => u.B_ReadOnly, update.ReadOnly)
                        .Update();
                    return true;
                }
                else return false;

            }
        }
        public override bool insCmsPageViews(Guid id, cmsPageViewsModel update)
        {
            using (var db = new CMSdb(_context))
            {
                
                    db.cms_PageViewss
                        .Value(u => u.id ,id)
                        .Value(u => u.C_Title, update.Title)
                        .Value(u => u.C_Url, update.Url)
                        .Value(u => u.B_ReadOnly, update.ReadOnly)
                        .Insert();
                    return true;
               

            }
        }
        public override bool delCmsPageViews(Guid id) {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_PageViewss;
                if (data.Where(w => w.id == id) != null)
                {
                    data
                        .Where(w => w.id == id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public override AccountModel getCmsAccount(string Email)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_Userss.
                    Where(w => w.C_EMail == Email).
                    Select(s => new AccountModel
                    {
                        id = s.Id,
                        Mail = s.C_EMail,
                        Salt = s.C_Salt,
                        Hash = s.C_Hash,
                        Group = s.F_Group,
                        Surname = s.C_Surname,
                        Name = s.C_Name,
                        Patronymic = s.C_Patronymic,
                        Disabled = s.B_Disabled,
                        Deleted = s.B_Deleted
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override AccountModel getCmsAccount(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_Userss.
                    Where(w => w.Id == Id).
                    Select(s => new AccountModel
                    {
                        id = s.Id,
                        Mail = s.C_EMail,
                        Salt = s.C_Salt,
                        Hash = s.C_Hash,
                        Group = s.F_Group,
                        Surname = s.C_Surname,
                        Name = s.C_Name,
                        Patronymic = s.C_Patronymic,
                        Disabled = s.B_Disabled,
                        Deleted = s.B_Deleted

                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        /// <summary>
        /// Вывод списка сайтов, доступных пользователю
        /// </summary>
        /// <returns></returns>
        public override DomainList[] getUserDomains(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SP_cms_GetUserDomains(Id)
                    .Select(s => new DomainList
                    {
                        Permit = s.Num,
                        SiteId = s.F_SiteId,
                        DomainName = s.C_Domain
                    })
                    .ToArray();

                if (!data.Any()) { return null; }
                else { return data; }
            }
        }

        /// <summary>
        /// Вывод настроек сайта
        /// </summary>
        /// <returns></returns>
        public override SettingsModel getCmsSettings()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Settingss.
                    Select(s => new SettingsModel
                    {
                        Title = s.C_Title,
                        Info = s.C_Info,
                        Desc = s.C_Desc,
                        Keyw = s.C_Keyw,
                        MailServer = s.C_MailServer,
                        MailFrom = s.C_MailFrom,
                        MailFromAlias = s.C_MailFromAlias,
                        MailTo = s.C_MailTo,
                        Adres = s.C_Adres,
                        F_CoordX = s.F_CoordX,
                        F_CoordY = s.F_CoordY,
                        Phone = s.C_Phone,
                        Fax = s.C_Fax,
                        Mail = s.C_Mail,
                        Url = s.C_Url,
                        WorkMode = s.C_WorkMode,
                        SSL=s.B_SSL,
                        MailPort=s.N_MailPort
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool updateCmsSettings(SettingsModel settings)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Settingss.Where(w => w.C_Guid != null);
                if (data != null)
                {
                    data.
                        Set(u => u.C_Title, settings.Title).
                        Set(u => u.C_Info, settings.Info).
                        Set(u => u.C_Desc, settings.Desc).
                        Set(u => u.C_Keyw, settings.Keyw).
                        Set(u => u.C_MailServer, settings.MailServer).
                        Set(u => u.C_MailFrom, settings.MailFrom).
                        Set(u => u.C_MailFromAlias, settings.MailFromAlias).
                        Set(u => u.C_MailPass, settings.MailPass).
                        Set(u => u.C_MailTo, settings.MailTo).
                        Set(u => u.C_Adres, settings.Adres).
                        Set(u => u.F_CoordX, settings.F_CoordX).
                        Set(u => u.F_CoordY, settings.F_CoordY).
                        Set(u => u.C_Phone, settings.Phone).
                        Set(u => u.C_Fax, settings.Fax).
                        Set(u => u.C_Mail, settings.Mail).
                        Set(u => u.C_Url, settings.Url).
                        Set(u => u.C_WorkMode, settings.WorkMode).
                        Set(u => u.B_SSL, settings.SSL).
                        Set(u => u.N_MailPort, settings.MailPort).
                        Update();

                    return true;
                }
                else return false;
            }
        }


        /// <summary>
        /// Права пользователей
        /// </summary>
        /// <returns></returns>
        public override UserResolutionModel getCmsUserResolutioInfo(Guid _userId,string _pageUrl)
        {
            
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Resolutionss
                    .Where(w => (w.C_Alias == _pageUrl && w.C_UserId == _userId))
                    .Select(s => new UserResolutionModel {
                        Title = s.C_Title,
                        Read = s.B_Read,
                        Write = s.B_Write,
                        Change = s.B_Change,
                        Delete = s.B_Delete
                    }).ToArray();
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        /// <summary>
        /// Для построения выпадающего списка с типами пользователей
        /// </summary>
        /// <returns></returns>
        public override UsersGroupModel[] getUserGroup()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_UsersGroups
                    .Select(s => new UsersGroupModel
                    {
                        C_Alias = s.C_Alias,
                        C_GroupName = s.C_GroupName,
                        id=s.id
                    }).ToArray();
                if (!data.Any()) { return null; }
                else { return data; }
            }
        }
        public override UsersGroupModel getUserGroup(string Alias) {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_UsersGroups
                    .Where(w => w.C_Alias == Alias)
                    .Select(s => new UsersGroupModel
                    {
                        C_Alias = s.C_Alias,
                        C_GroupName = s.C_GroupName,
                        id = s.id
                    }).ToArray();
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool createUsersGroup(Guid id,string Alias,string GroupName)
        {
            using (var db = new CMSdb(_context))
            {
                db.cms_UsersGroups
                    .Value(v => v.id, id)
                    .Value(v => v.C_Alias, Alias)
                    .Value(v => v.C_GroupName, GroupName)
                    .Insert();
                //добавить права группы пользователей
                var data=db.cms_Menus
                    .Select(s => new cmsMenuModel{ id = s.Id}).ToArray();
                foreach(cmsMenuModel s in data)
                {
                    db.cms_ResolutionsTemplatess
                        .Value(v => v.F_MenuId, s.id)
                        .Value(v => v.F_UserGroupId, Alias)
                        .Value(v => v.B_Read, false)
                        .Value(v => v.B_Change, false)
                        .Value(v => v.B_Delete, false)                        
                        .Insert();
                }
                return true;
            }

        }
        public override bool updateUsersGroup(string Alias,string GroupName)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_UsersGroups.Where(w => w.C_Alias == Alias);
                data
                    .Set(u => u.C_GroupName, GroupName)
                    .Update();
                return true;
            }                
        }
        public override bool deleteUsersGroup(string Alias) {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_UsersGroups.Where(w => w.C_Alias == Alias);
                data                    
                    .Delete();
                return true;
            }
        }

        public override bool changePasswordUser(Guid id,string NewSalt, string NewHash)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_Userss.Where(w => w.Id == id)
                    .Set(u => u.C_Salt, NewSalt)
                    .Set(u => u.C_Hash, NewHash)
                    .Update();
                return true;
            }

        }


        /// <summary>
        /// Права группы пользователей
        /// </summary>        
        /// <returns></returns>
        //public override ResolutionsModel[] getResolutions()
        public override ResolutionsTemplatesModel[] getResolutions(string UserGroupId)
        {
            using (var db = new CMSdb(_context))
            {

                var data = db.SV_cms_ResolutionsTemplatess
                    .Where(w=>w.F_UserGroupId== UserGroupId)
                    .Select(s => new ResolutionsTemplatesModel {                        
                        MenuId=s.Id,
                        MenuTitle=s.C_Title,
                        Section = s.C_Group,
                        Permit = s.N_Permit,
                        UserGroupId =s.F_UserGroupId,
                        Read=s.B_Read,
                        Write = s.B_Write,
                        Change = s.B_Change,
                        Delete= s.B_Delete
                    })
                    .OrderByDescending(d => d.Section)                    
                    .ToArray();
                if (!data.Any()) { return null; }
                else { return data; }
            }
        }
        public override bool appointResolutionsTemplates(string user, Guid url, string action, int val)
        {
            bool BoolVal = Convert.ToBoolean(val);
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_ResolutionsTemplatess.Where(w => (w.F_MenuId == url && w.F_UserGroupId == user));
                switch (action)
                {
                    case "delete":
                        data.Set(u => u.B_Delete, BoolVal)
                            .Update();
                        break;
                    case "change":
                        data.Set(u => u.B_Change, BoolVal)
                            .Update();
                        break;
                    case "write":
                        data.Set(u => u.B_Write, BoolVal)
                            .Update();
                        break;
                    case "read":
                        data.Set(u => u.B_Read, BoolVal)
                            .Update();
                        break;
                }
                return true;
            }
        }
        public override bool delResolutionsTemplates(string UserGroup)
        {
            using (var db = new CMSdb(_context)) {
                var data = db.cms_ResolutionsTemplatess.Where(w => w.F_UserGroupId == UserGroup);
                if (data != null)
                {
                    data.Delete();
                    return true;
                }
                else { return false; }
            }
        }

        /// <summary>
        /// права отдельных пользователей
        /// </summary>
        /// <param name="UserId">id пользователя</param>
        /// <returns></returns>
        public override ResolutionsModel[] getResolutionsPerson(Guid UserId) {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Resolutionss                                       
                    .Where(w => w.C_UserId == UserId)
                    .Select(s => new ResolutionsModel {
                        MenuId=s.C_MenuId,
                        MenuTitle=s.C_Title,
                        Section=s.C_Group,
                        Permit=s.N_Permit,
                        UserId =s.C_UserId,
                        Read=s.B_Read,
                        Write=s.B_Write,
                        Change=s.B_Change,
                        Delete=s.B_Delete,
                        Importent=s.B_Importent  
                    })
                    .OrderByDescending(d=>d.Section)
                    .ToArray();
                if (!data.Any()) { return null; }
                else { return data; }

            }
        }
        public override bool appointResolutionsUser(Guid id, Guid url, string action, int val)
        {
            bool BoolVal = Convert.ToBoolean(val);
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_Resolutionss.Where(w => (w.C_UserId == id && w.C_MenuId == url));
                switch (action)
                {
                    case "delete":
                        data.Set(u => u.B_Delete, BoolVal)
                            .Set(u => u.B_Importent, true)
                            .Update();
                        break;
                    case "change":
                        data.Set(u => u.B_Change, BoolVal)
                            .Set(u => u.B_Importent, true)
                            .Update();
                        break;
                    case "write":
                        data.Set(u => u.B_Write, BoolVal)
                            .Set(u => u.B_Importent, true)
                            .Update();
                        break;
                    case "read":
                        data.Set(u => u.B_Read, BoolVal)
                            .Set(u => u.B_Importent, true)
                            .Update();
                        break;
                }
            }
            return true;
        }


        /// <summary>
        /// Вывод списка всех пользователей (только для админов портала)
        /// </summary>
        /// <returns></returns>
        public override UsersModel[] getAllUserList(string group)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Userss
                    .Where(w => (w.B_Deleted != true)
                                && (w.F_Group == group || group == null))
                    .Select(s => new UsersModel
                    {
                        Id = s.Id,
                        C_EMail = s.C_EMail,
                        F_Group = s.F_Group,
                        F_GroupName = s.F_GroupName,
                        C_Surname = s.C_Surname,
                        C_Name = s.C_Name,
                        C_Patronymic = s.C_Patronymic,
                        B_Disabled = s.B_Disabled
                    }).ToArray();
                if (!data.Any()) { return null; }
                else { return data; }
            }
        }

        /// <summary>
        /// Вывод списка пользователей
        /// </summary>
        /// <returns></returns>
        public override UsersModel[] getUserList(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Userss
                    .Where(w => w.B_Deleted != true)
                    .Select(s => new UsersModel
                    {
                        Id = s.Id,
                        C_EMail = s.C_EMail,
                        F_Group = s.F_Group,
                        F_GroupName = s.F_GroupName,
                        C_Surname = s.C_Surname,
                        C_Name = s.C_Name,
                        C_Patronymic = s.C_Patronymic,
                        B_Disabled = s.B_Disabled
                    }).ToArray();
                if (!data.Any()) { return null; }
                else { return data; }
            }
        }
        public override UsersModel[] getUserList(string GroupName, string domain)
        {            
            using (var db = new CMSdb(_context))
            {
                var Users = db.SV_cms_Userss
                    .Where(w => (w.F_Group == GroupName || GroupName == null))
                    .Select(s => new UsersModel
                    {
                        Id = s.Id,
                        C_EMail = s.C_EMail,
                        F_Group = s.F_Group,
                        F_GroupName = s.F_GroupName,
                        C_Surname = s.C_Surname,
                        C_Name = s.C_Name,
                        C_Patronymic = s.C_Patronymic,
                        B_Disabled = s.B_Disabled
                    }).ToArray();
                if (!Users.Any()) { return null; }
                else { return getUsersByDomainHelper(Users, domain); }
            }
        }
        public override UsersModel[] getUsersList(string GroupName, string searchSurname, string searchName, string searchEmail, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var Users = db.SV_cms_Userss
                    .Where(w => (w.F_Group == GroupName || GroupName==null) 
                    && (searchSurname==null || w.C_Surname.ToLower().Contains(searchSurname))
                    &&(searchName==null || w.C_Name.ToLower().Contains(searchName)
                    &&(searchEmail==null || w.C_EMail.ToLower().Contains(searchEmail)))
                    )
                    .Select(s => new UsersModel
                    {
                        Id = s.Id,
                        C_EMail = s.C_EMail,
                        F_GroupName = s.F_GroupName,
                        C_Surname = s.C_Surname,
                        C_Name = s.C_Name,
                        C_Patronymic = s.C_Patronymic,
                        B_Disabled = s.B_Disabled,
                        B_Deleted = s.B_Deleted
                    }).ToArray();
                if (!Users.Any()) { return null; }
                else { return getUsersByDomainHelper(Users, domain); }
            }
        }

        // для проверки на привязанность к домену
        private UsersModel[] getUsersByDomainHelper(UsersModel[] Users, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var UserSiteLink = db.cms_UserSiteLinks
                        .Where(w => w.F_SiteId == domain)
                        .ToArray();

                UsersModel[] data = new UsersModel[UserSiteLink.Length];
                int i = 0;

                foreach (UsersModel user in Users)
                {
                    foreach (cms_UserSiteLink link in UserSiteLink)
                    {
                        if (link.F_UserId == user.Id)
                        {
                            data[i] = user;
                            i++;
                        }
                    }
                }
                data = data.Where(w => w != null).ToArray();
                if (data.Length == 0) { return null; }
                else return data;
            }
        }
       

        /// <summary>
        /// Вывод информации о конкретном пользователе 
        /// </summary>
        /// <returns></returns>
        public override UsersModel getUser(Guid? Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Userss
                    .Where(w => w.Id == Id)
                    .Select(s => new UsersModel
                    {
                        Id = s.Id,
                        C_EMail = s.C_EMail,
                        F_Group = s.F_Group,
                        F_GroupName = s.F_GroupName,
                        C_Post = s.C_Post,
                        C_Desc = s.C_Desc,
                        C_Keyw = s.C_Keyw,
                        C_Surname = s.C_Surname,
                        C_Name = s.C_Name,
                        C_Patronymic = s.C_Patronymic,
                        D_Birthday = s.D_Birthday,
                        B_Sex = s.B_Sex,
                        C_Photo = s.C_Photo,
                        C_Adres = s.C_Adres,
                        C_Phone = s.C_Phone,
                        C_Mobile = s.C_Mobile,
                        C_Contacts = s.C_Contacts,
                        B_Disabled = s.B_Disabled,
                        B_Deleted = s.B_Deleted
                    });
                if (Id != null && !data.Any())
                {
                    UsersModel newItem = new UsersModel();
                    return newItem;
                }
                else if (!data.Any())
                {
                    return null;
                }
                else
                {
                    return data.First();
                }
            }
        }
        public override UsersModel getUser(Guid? Id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var User = db.SV_cms_Userss.Where(w => w.Id == Id)
                    .Select(s => new UsersModel
                    {
                        Id = s.Id,
                        C_EMail = s.C_EMail,
                        F_Group = s.F_Group,
                        F_GroupName = s.F_GroupName,
                        C_Post = s.C_Post,
                        C_Desc = s.C_Desc,
                        C_Keyw = s.C_Keyw,
                        C_Surname = s.C_Surname,
                        C_Name = s.C_Name,
                        C_Patronymic = s.C_Patronymic,
                        D_Birthday = s.D_Birthday,
                        B_Sex = s.B_Sex,
                        C_Photo = s.C_Photo,
                        C_Adres = s.C_Adres,
                        C_Phone = s.C_Phone,
                        C_Mobile = s.C_Mobile,
                        C_Contacts = s.C_Contacts,
                        B_Disabled = s.B_Disabled,
                        B_Deleted = s.B_Deleted
                    });
                if (Id != null && !User.Any())
                {
                    UsersModel newItem = new UsersModel();
                    return newItem;
                }
                else if (!User.Any())
                {
                    return null;
                }
                else
                {
                    // проверка на привязанность к домену
                    var UserSiteLink = db.cms_UserSiteLinks
                        .Where(w => w.F_SiteId == domain)
                        .ToArray();

                    foreach (cms_UserSiteLink link in UserSiteLink)
                    {
                        // выборка
                        if (link.F_UserId == User.First().Id) { return User.First(); }
                    }
                    return null;
                }
            }
        }
        public override UsersModel getUser(string email)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Userss
                    .Where(w => (w.C_EMail == email) && (w.B_Deleted != true))
                    .Select(s => new UsersModel
                    {
                        Id = s.Id,
                        C_EMail = s.C_EMail,
                        F_Group = s.F_Group,
                        F_GroupName = s.F_GroupName,
                        C_Post = s.C_Post,
                        C_Surname = s.C_Surname,
                        C_Name = s.C_Name,
                        C_Patronymic = s.C_Patronymic,
                        D_Birthday = s.D_Birthday,
                        B_Sex = s.B_Sex,
                        C_Photo = s.C_Photo,
                        B_Disabled = s.B_Disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool updateUser(Guid Id, UsersModel obj)
        {
            using (var db = new CMSdb(_context))
            {                

                var data = db.cms_Userss.Where(w => w.Id == Id);
                if (data != null)
                {                    
                    string OldGroup = data.Select(x => x.F_Group).FirstOrDefault();                   
                    data
                                                
                        .Set(u => u.C_EMail, obj.C_EMail)
                        .Set(u => u.F_Group, obj.F_Group)
                        .Set(u => u.C_Post, obj.C_Post)
                        .Set(u => u.C_Desc, obj.C_Desc)
                        .Set(u => u.C_Keyw, obj.C_Keyw)
                        .Set(u => u.C_Surname, obj.C_Surname)
                        .Set(u => u.C_Name, obj.C_Name)
                        .Set(u => u.C_Patronymic, obj.C_Patronymic)
                        .Set(u => u.D_Birthday, obj.D_Birthday)
                        .Set(u => u.B_Sex, obj.B_Sex)
                        .Set(u => u.C_Photo, obj.C_Photo)
                        .Set(u => u.C_Adres, obj.C_Adres)
                        .Set(u => u.C_Phone, obj.C_Phone)
                        .Set(u => u.C_Mobile, obj.C_Mobile)
                        .Set(u => u.C_Contacts, obj.C_Contacts)
                        .Set(u => u.B_Disabled, obj.B_Disabled)
                        .Update();
                    if(OldGroup!= obj.F_Group)
                    {
                        #region выявление прав подлежащих изменению и их последующее назначение
                        var UpdateResolut = db.cms_ResolutionsTemplatess
                            .Where(w => w.F_UserGroupId == obj.F_Group)
                            .Join(db.cms_Resolutionss.Where(we => (we.C_UserId == Id && we.B_Importent == false)), md => md.F_MenuId, m => m.C_MenuId, (md, m) => new { md, m })
                            .Select(s => new ResolutionsTemplatesModel
                            {
                                MenuId = s.md.F_MenuId,
                                UserGroupId = s.md.F_UserGroupId,
                                Read = s.md.B_Read,
                                Write = s.md.B_Write,
                                Change = s.md.B_Change,
                                Delete = s.md.B_Delete
                            }).ToArray();                        
                        foreach (ResolutionsTemplatesModel r in UpdateResolut)
                        {
                            db.cms_Resolutionss
                                .Where(w=>(w.C_MenuId==r.MenuId && w.C_UserId== Id))                                
                                .Set(v => v.B_Read, r.Read)
                                .Set(v => v.B_Write, r.Write)
                                .Set(v => v.B_Change, r.Change)
                                .Set(v => v.B_Delete, r.Delete)                                
                                .Update();
                        }
                        #endregion
                    }

                    return true;
                }
                else return false;
            }
        }
        /// <summary>
        /// Проверка доступности Email (при создании нового пользователя)
        /// </summary>
        /// <returns></returns>
        public override bool isEmailFree(string email)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_cms_Userss
                    .Where(w => (w.B_Deleted != true)
                                && (w.C_EMail == email || email == null))
                    .Select(s => s.C_EMail)
                    .ToArray();
                if (!data.Any()) { return true; }
                else { return false; }
            }
        }
        public override bool createUser(Guid Id, UsersModel obj)
        {
            using (var db = new CMSdb(_context))
            {
                db.cms_Userss
                    .Value(v => v.Id, obj.Id)
                    .Value(v => v.C_Salt, obj.C_Salt)
                    .Value(v => v.C_Hash, obj.C_Hash)
                    .Value(v => v.C_EMail, obj.C_EMail)
                    .Value(v => v.F_Group, obj.F_Group)
                    .Value(v => v.C_Post, obj.C_Post)
                    .Value(v => v.C_Desc, obj.C_Desc)
                    .Value(v => v.C_Keyw, obj.C_Keyw)
                    .Value(v => v.C_Surname, obj.C_Surname)
                    .Value(v => v.C_Name, obj.C_Name)
                    .Value(v => v.C_Patronymic, obj.C_Patronymic)
                    .Value(v => v.D_Birthday, obj.D_Birthday)
                    .Value(v => v.B_Sex, obj.B_Sex)
                    .Value(v => v.C_Photo, obj.C_Photo)
                    .Value(v => v.C_Adres, obj.C_Adres)
                    .Value(v => v.C_Phone, obj.C_Phone)
                    .Value(v => v.C_Mobile, obj.C_Mobile)
                    .Value(v => v.C_Contacts, obj.C_Contacts)
                    .Value(v => v.B_Disabled, obj.B_Disabled)
                    .Value(v => v.B_Deleted, false)
                    .Insert();

                //назначение прав пользователю в зависимости от выбранной группы
                var ResolutGroup = db.cms_ResolutionsTemplatess.
                   Where(w => w.F_UserGroupId == obj.F_Group).
                   Select(s => new ResolutionsTemplatesModel
                   {
                       MenuId = s.F_MenuId,
                       UserGroupId = s.F_UserGroupId,
                       Read = s.B_Read,
                       Write = s.B_Write,
                       Change = s.B_Change,
                       Delete = s.B_Delete
                   }).ToArray();
                foreach (ResolutionsTemplatesModel r in ResolutGroup)
                {
                    db.cms_Resolutionss
                        .Value(v => v.C_MenuId, r.MenuId)
                        .Value(v => v.C_UserId, obj.Id)
                        .Value(v => v.B_Read, r.Read)
                        .Value(v => v.B_Write, r.Write)
                        .Value(v => v.B_Change, r.Change)
                        .Value(v => v.B_Delete, r.Delete)
                        .Value(v => v.B_Importent, false)
                        .Insert();
                }
                return true;
            }
        }
        public override bool createUser(Guid Id, string domain, UsersModel obj)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    db.cms_Userss
                        .Value(v => v.Id, obj.Id)
                        .Value(v => v.C_Salt, obj.C_Salt)
                        .Value(v => v.C_Hash, obj.C_Hash)
                        .Value(v => v.C_EMail, obj.C_EMail)
                        .Value(v => v.F_Group, obj.F_Group)
                        .Value(v => v.C_Post, obj.C_Post)
                        .Value(v => v.C_Desc, obj.C_Desc)
                        .Value(v => v.C_Keyw, obj.C_Keyw)
                        .Value(v => v.C_Surname, obj.C_Surname)
                        .Value(v => v.C_Name, obj.C_Name)
                        .Value(v => v.C_Patronymic, obj.C_Patronymic)
                        .Value(v => v.D_Birthday, obj.D_Birthday)
                        .Value(v => v.B_Sex, obj.B_Sex)
                        .Value(v => v.C_Photo, obj.C_Photo)
                        .Value(v => v.C_Adres, obj.C_Adres)
                        .Value(v => v.C_Phone, obj.C_Phone)
                        .Value(v => v.C_Mobile, obj.C_Mobile)
                        .Value(v => v.C_Contacts, obj.C_Contacts)
                        .Value(v => v.B_Disabled, obj.B_Disabled)
                        .Value(v => v.B_Deleted, false)
                        .Insert();

                    //назначение прав пользователю в зависимости от выбранной группы
                    var ResolutGroup = db.cms_ResolutionsTemplatess.
                       Where(w => w.F_UserGroupId == obj.F_Group).
                       Select(s => new ResolutionsTemplatesModel
                       {
                           MenuId = s.F_MenuId,
                           UserGroupId = s.F_UserGroupId,
                           Read = s.B_Read,
                           Write = s.B_Write,
                           Change = s.B_Change,
                           Delete = s.B_Delete
                       }).ToArray();
                    foreach (ResolutionsTemplatesModel r in ResolutGroup)
                    {
                        db.cms_Resolutionss
                            .Value(v => v.C_MenuId, r.MenuId)
                            .Value(v => v.C_UserId, obj.Id)
                            .Value(v => v.B_Read, r.Read)
                            .Value(v => v.B_Write, r.Write)
                            .Value(v => v.B_Change, r.Change)
                            .Value(v => v.B_Delete, r.Delete)
                            .Value(v => v.B_Importent, false)
                            .Insert();
                    }
                    // создаем связь нового пользователя и имени сайта
                    db.cms_UserSiteLinks
                        .Value(v => v.F_SiteId, domain)
                        .Value(v => v.F_UserId, obj.Id)
                        .Insert();
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool deleteUser(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_Userss.Where(w => w.Id == Id);
                if (data != null)
                {
                    data
                        .Where(w => w.Id == Id)
                        .Delete();
                    db.cms_Resolutionss
                        .Where(r => r.C_UserId == Id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }
        public override bool deleteUser(Guid Id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                  var data = db.cms_Userss.Where(w => w.Id == Id);

                if (data != null)
                {
                    // проверка на привязанность к домену
                    var UserSiteLink = db.cms_UserSiteLinks
                            .Where(w => w.F_SiteId == domain)
                            .ToArray();

                    foreach (cms_UserSiteLink link in UserSiteLink)
                    {
                        if (link.F_UserId == data.First().Id)
                        {
                            // удаление
                            data.Delete();
                            db.cms_Resolutionss
                                .Where(r => r.C_UserId == Id)
                                .Delete();
                            return true;
                        }
                    }
                }
                return false;
            }
        }


        /// <summary>
        /// Сайты, связанные с пользователем (только для админов портала)
        /// </summary>
        /// <returns></returns>
        public override SitesModel[] getUserSiteLinks(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var domains = db.cms_UserSiteLinks
                    .Where(w => w.F_UserId == userId)
                    .Select(s => new UserSiteLink
                    {
                        UserId = s.F_UserId,
                        SiteId = s.F_SiteId
                    })
                    .ToArray();

                SitesModel[] data = new SitesModel[db.main_Sitess.Count()];
                int i = 0;

                foreach(UserSiteLink link in domains)
                {
                    var sites = db.main_Sitess
                        .Where(w => w.C_Domain == link.SiteId)
                        .Select(s => new SitesModel
                        {
                            Id = s.Id,
                            C_Name = s.C_Name,
                            C_Domain = s.C_Domain
                        })
                        .ToArray();
                    foreach(SitesModel site in sites)
                    {
                        data[i] = site;
                        i++;
                    }
                }
                data = data.Where(w => w != null).ToArray();

                if (!data.Any()) { return null; }
                else { return data; }
            }
        }
        public override bool createUserSiteLink(UserSiteLink obj)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    db.cms_UserSiteLinks
                        .Value(v => v.F_SiteId, obj.SiteId)
                        .Value(v => v.F_UserId, obj.UserId)
                        .Insert();
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool deleteUserSiteLink(Guid userId, string siteId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_UserSiteLinks.Where(w => (w.F_UserId == userId) && (w.F_SiteId == siteId));
                if (data != null)
                {
                    data.Delete();
                    return true;
                }
                else return false;
            }
        }


        /// <summary>
        /// Список сайтов
        /// </summary>
        /// <returns></returns>
        public override SitesDomainModel getSiteId(string Domain)
        {
            using (var db = new CMSdb(_context))
            {
                string SiteId = String.Empty;

                var data = db.main_SitesDomainLists.
                    Where(w => w.C_Domain == Domain).
                    Select(s => new SitesDomainModel { SiteId = s.F_SiteId });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override SitesDomainModel[] getSiteDomains(string siteId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SitesDomainLists
                    .Where(w => w.F_SiteId == siteId)
                    .OrderBy(o => o.Num)
                    .Select(s => new SitesDomainModel {
                        Id = s.id,
                        Domain = s.C_Domain
                    })
                    .ToArray();

                if (!data.Any()) { return null; }
                else { return data; }
            }
        }
        public override bool createSiteDomain(SitesDomainModel obj)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    db.main_SitesDomainLists
                        .Value(v => v.id, obj.Id)
                        .Value(v => v.F_SiteId, obj.SiteId)
                        .Value(v => v.C_Domain, obj.Domain)
                        .Insert();
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool deleteSiteDomain(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SitesDomainLists.Where(w => w.id == id);
                if (data != null)
                {
                    data
                        .Where(w => w.id == id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }
        public override SitesModel[] getSiteList(string SearchLine)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Sitess
                    .Select(s => new SitesModel
                    {
                        Id = s.Id,
                        C_Name = s.C_Name,
                        C_Domain = s.C_Domain
                    })
                    .ToArray();

                if (SearchLine == null || SearchLine == "")
                {
                    if (!data.Any()) { return null; }
                    else { return data; }
                }
                else return data.Where(w => w.C_Name.ToLower().Contains(SearchLine)).ToArray();
            }
        }
        public override SitesModel getSite(Guid? Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Sitess.Where(w => w.Id == Id)
                    .Select(s => new SitesModel
                    {
                        Id = s.Id,
                        C_Name = s.C_Name,
                        C_Domain = s.C_Domain,
                        C_Adress = s.C_Adress,
                        C_Phone = s.C_Phone,
                        C_Fax = s.C_Fax,
                        C_Email = s.C_Email,
                        C_Site = s.C_Site,
                        C_Worktime = s.C_Worktime,
                        C_Logo = s.C_Logo,
                        C_Scripts = s.C_Scripts,
                        C_AdmEmail = s.C_AdmEmail,
                        C_AdmPhone = s.C_AdmPhone
                    });
                if (Id != null && !data.Any())
                {
                    SitesModel newItem = new SitesModel();
                    return newItem;
                }
                else if (data.Any())
                {
                    return data.First();
                }
                else return null;
            }
        }
        public override SitesModel getSite(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Sitess.Where(w => w.C_Domain == domain)
                    .Select(s => new SitesModel
                    {
                        Id = s.Id,
                        C_Name = s.C_Name,
                        C_Domain = s.C_Domain,
                        C_Adress = s.C_Adress,
                        C_Phone = s.C_Phone,
                        C_Fax = s.C_Fax,
                        C_Email = s.C_Email,
                        C_Site = s.C_Site,
                        C_Worktime = s.C_Worktime,
                        C_Logo = s.C_Logo,
                        C_Scripts = s.C_Scripts,
                        C_AdmEmail = s.C_AdmEmail,
                        C_AdmPhone = s.C_AdmPhone
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool createSite(Guid Id, SitesModel obj)
        {
            using (var db = new CMSdb(_context))
            {
                db.main_Sitess
                    .Value(v => v.Id, Id)
                    .Value(v => v.C_Name, obj.C_Name)
                    .Value(v => v.C_Domain, obj.C_Domain)
                    .Value(v => v.C_Adress, obj.C_Adress)
                    .Value(v => v.C_Phone, obj.C_Phone)
                    .Value(v => v.C_Fax, obj.C_Fax)
                    .Value(v => v.C_Email, obj.C_Email)
                    .Value(v => v.C_Site, obj.C_Site)
                    .Value(v => v.C_Worktime, obj.C_Worktime)
                    .Value(v => v.C_Logo, obj.C_Logo)
                    .Value(v => v.C_Scripts, obj.C_Scripts)
                    .Value(v => v.C_AdmEmail, obj.C_AdmEmail)
                    .Value(v => v.C_AdmPhone, obj.C_AdmPhone)
                    .Insert();

                return true;
            }
        }
        public override bool updateSite(Guid Id, SitesModel obj)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Sitess.Where(w => w.Id == Id);
                if (data != null)
                {
                    data
                        .Set(u => u.Id, Id)
                        .Set(u => u.C_Name, obj.C_Name)
                        .Set(u => u.C_Domain, obj.C_Domain)
                        .Set(u => u.C_Adress, obj.C_Adress)
                        .Set(u => u.C_Phone, obj.C_Phone)
                        .Set(u => u.C_Fax, obj.C_Fax)
                        .Set(u => u.C_Email, obj.C_Email)
                        .Set(u => u.C_Site, obj.C_Site)
                        .Set(u => u.C_Worktime, obj.C_Worktime)
                        .Set(u => u.C_Logo, obj.C_Logo)
                        .Set(u => u.C_Scripts, obj.C_Scripts)
                        .Set(u => u.C_AdmEmail, obj.C_AdmEmail)
                        .Set(u => u.C_AdmPhone, obj.C_AdmPhone)
                        .Update();

                    return true;
                }
                else return false;
            }
        }
        public override bool deleteSite(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Sitess.Where(w => w.Id == Id);
                if (data != null)
                {
                    data
                        .Where(w => w.Id == Id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }


        /// <summary>
        /// Фотоальбомы
        /// </summary>
        /// <returns></returns>
        public override PhotoAlbumsModel[] getPhotoAlbums(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PhotoAlbumss
                    .Where(w => w.F_Site == domain)
                    .OrderByDescending(p => p.D_Date)
                    .Select(s => new PhotoAlbumsModel
                    {
                        Id = s.Id,
                        Title = s.C_Title,
                        Desc = s.C_Desc,
                        Date = s.D_Date,
                        Author = s.C_Author,
                        Preview = s.C_Preview
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override PhotoAlbumsModel getPhotoAlbums(Guid id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PhotoAlbumss
                    .Where(w => w.Id == id && w.F_Site == domain)
                    .Select(s => new PhotoAlbumsModel
                    {
                        Id = s.Id,
                        Title = s.C_Title,
                        Desc = s.C_Desc,
                        Date = s.D_Date,
                        Author = s.C_Author
                        //Preview = s.C_Preview
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }

            }
        }
        public override bool insertPhotoAlbums(Guid id, PhotoAlbumsModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    db.main_PhotoAlbumss
                    .Value(v => v.Id, id)
                    .Value(v => v.F_Site, insert.SiteId)
                    .Value(v => v.C_Title, insert.Title)
                    .Value(v => v.C_Desc, insert.Desc)
                    .Value(v => v.D_Date, insert.Date)
                    .Value(v => v.C_Author, insert.Author)
                    .Value(v => v.C_Preview, insert.Preview)
                    .Insert();
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool updatePhotoAlbums(Guid id, PhotoAlbumsModel update)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PhotoAlbumss.Where(w => w.Id == id);
                if (data != null)
                {
                    data
                    .Where(w => w.Id == id)
                    .Set(u => u.F_Site, update.SiteId)
                    .Set(u => u.C_Title, update.Title)
                    .Set(u => u.C_Desc, update.Desc)
                    .Set(u => u.D_Date, update.Date)
                    .Set(u => u.C_Author, update.Author)
                    .Update();
                    return true;
                }
                else return false;
            }
        }
        public override bool updatePhotoAlbumPreview(Guid id, string preview)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PhotoAlbumss.Where(w => w.Id == id);
                if (data != null)
                {
                    data
                    .Where(w => w.Id == id)
                    .Set(u => u.C_Preview, preview)
                    .Update();
                    return true;
                }
                else return false;
            }
        }
        public override bool deletePhotoAlbums(Guid id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_PhotoAlbumss;
                if (data.Where(w => w.Id == id && w.F_Site == domain) != null)
                {
                    data
                        .Where(w => w.Id == id && w.F_Site == domain)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// Фотографии в фотоальбомах
        /// </summary>
        /// <returns></returns>
        public override PhotosModel[] getPhotos(Guid AlbumId)
        {
            using (var db = new CMSdb(_context))
            {
                try {
                    var data = db.main_Photoss
                    .OrderBy(o => o.C_Preview)
                    .Where(w => w.Album_Id == AlbumId)
                    .Select(s => new PhotosModel
                    {
                        Id = s.Id,
                        Album_Id = s.Album_Id,
                        Title = s.C_Title,
                        Date = s.D_Date,
                        Preview = s.C_Preview,
                        Photo = s.C_Photo
                    });
                    return data.ToArray();
                }
                catch { return null; }
            }
        }
        public override PhotosModel getPhoto(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    var data = db.main_Photoss
                        .Where(w => w.Id == Id)
                        .Select(s => new PhotosModel
                        {
                            Id = s.Id,
                            Album_Id = s.Album_Id,
                            Title = s.C_Title,
                            Date = s.D_Date,
                            Preview = s.C_Preview,
                            Photo = s.C_Photo
                        })
                        .First();
                    return data;
                }
                catch { return null; }
            }
        }
        public override bool insertPhotos(Guid AlbumId, PhotosModel[] insert)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    foreach(PhotosModel item in insert)
                    {
                        db.main_Photoss
                        .Value(v => v.Id, item.Id)
                        .Value(v => v.Album_Id, AlbumId)
                        .Value(v => v.C_Title, item.Title)
                        .Value(v => v.D_Date, item.Date)
                        .Value(v => v.C_Preview, item.Preview)
                        .Value(v => v.C_Photo, item.Photo)
                        .Insert();
                    }
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool deletePhotos(Guid AlbumId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Photoss;
                if (data.Where(w => w.Album_Id == AlbumId) != null)
                {
                    data
                        .Where(w => w.Album_Id == AlbumId)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }
        public override bool deletePhoto(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Photoss;
                if (data.Where(w => w.Id == Id) != null)
                {
                    data
                        .Where(w => w.Id == Id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// Видео
        /// </summary>
        /// <returns></returns>
        public override cmsVideoModel[] getVideo()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Videos
                    .OrderByDescending(o => o.D_Date)
                    .Select(s => new cmsVideoModel
                    {
                        Id=s.id,
                        Title=s.C_Title,
                        Date=s.D_Date,
                        Desc=s.C_Desc,
                        VideoUrl=s.C_Video,
                        PreviewUrl=s.C_Preview,
                        Convert=s.B_Convret                        
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }

        }
        public override cmsVideoModel getVideo(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data=db.main_Videos
                    .Where(w => w.id == id)
                    .Select(s => new cmsVideoModel
                    {
                        Id = s.id,
                        Title = s.C_Title,
                        Date = s.D_Date,
                        Desc = s.C_Desc,
                        VideoUrl = s.C_Video,
                        PreviewUrl = s.C_Preview,
                        Convert = s.B_Convret
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        /// <summary>
        /// Баннеры
        /// </summary>
        /// <returns></returns>
        public override BannersModel[] getBanners(string[] sections, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                BannersModel[] data = new BannersModel[sections.Length];

                for (int i=0; i < sections.Length; i++)
                {
                    data = db.SV_main_Bannerss
                           .Where(w => (w.F_Section.ToLower() == sections[i])
                                    && w.F_Site == domain)
                           .Select(s => new BannersModel
                           {
                               BannersList = getcmsBanners(s.F_Section, null, domain)
                           })
                           .ToArray();
                    //TODO: добавлять секции баннеров в data, а не перезаписывать (сейчас перезаписывается)
                }
                if (data.Length == 0) { return null; }
                else { return data; }


                //BannersModel[] data = new BannersModel[sections.Length];

                //for (int i=0; i < sections.Length; i++)
                //{
                //    var bannersList = db.SV_main_Bannerss
                //       .OrderByDescending(o => o.N_Permit)
                //       .Where(w => (w.F_Section.ToLower() == sections[i])
                //                && w.F_Site == domain)
                //       .Select(s => new cmsBannersModel
                //       {
                //           Id = s.id,
                //           Title = s.C_Title,
                //           Photo = s.C_Photo,
                //           Date = s.D_Date,
                //           Url = s.C_Url,
                //           Type = s.F_Type,
                //           TypeText = s.TypeText,
                //           Section = s.F_Section,
                //           SectionText = s.SectionText,
                //           Permit = s.N_Permit,
                //           Target = s.B_Target,
                //           Disabled = s.B_Disabled
                //       })
                //       .ToArray();
                    
                //    data[i].BannersList = bannersList;
                //    //data.SetValue(bannersList.ToArray(), i); //TODO: запихнуть bannersList.ToArray() в data
                //}

                //data = data.Where(w => w != null).ToArray();
            }
        }

        public override cmsBannersModel[] getcmsBanners(string Section, string Type, string domain) {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_main_Bannerss
                   .OrderByDescending(o => o.N_Permit)
                   .Where(w => (Section == null || w.F_Section.ToLower()==Section)
                        && (Type == null || w.F_Type.ToLower()==Type)
                        && w.F_Site == domain)
                   .Select(s => new cmsBannersModel
                   {
                       Id = s.id,
                       Title = s.C_Title,
                       Photo = s.C_Photo,
                       Date=s.D_Date,
                       Type=s.F_Type,
                       TypeText=s.TypeText,
                       Section=s.F_Section,
                       SectionText=s.SectionText,
                       Permit=s.N_Permit,                       
                       Disabled=s.B_Disabled                       
                   });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }

            }
        }
        public override cmsBannersModel getBanner(Guid id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Bannerss
                    .Where(w => w.id == id && w.F_Site == domain)
                    .Select(s => new cmsBannersModel
                    {
                        Id = s.id,
                        Title = s.C_Title,
                        Photo = s.C_Photo,
                        Url = s.C_Url,
                        Text = s.C_Text,
                        Date = s.D_Date,
                        Type = s.F_Type,
                        Section = s.F_Section,
                        Permit = s.N_Permit,
                        Target = s.B_Target,
                        Disabled = s.B_Disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool insertBanners(Guid id, cmsBannersModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    db.main_Bannerss
                    .Value(v => v.id, id)
                    .Value(v => v.F_Site, insert.SiteId)
                    .Value(v => v.C_Title, insert.Title)
                    .Value(v => v.C_Photo, insert.Photo)
                    .Value(v => v.C_Url, insert.Url)
                    .Value(v => v.C_Text, insert.Text)
                    .Value(v => v.D_Date, insert.Date)
                    .Value(v => v.F_Type, insert.Type)
                    .Value(v => v.F_Section, insert.Section)
                    .Value(v => v.N_Permit, insert.Permit)
                    .Value(v => v.B_Target, insert.Target)
                    .Value(v => v.B_Disabled, insert.Disabled)                    
                    .Insert();
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool updBanners(Guid id, cmsBannersModel update)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Bannerss.Where(w => w.id == id);
                if (data != null)
                {
                    data
                    .Where(w => w.id == id)
                    .Set(u => u.C_Title, update.Title)
                    .Set(u => u.C_Photo, update.Photo)
                    .Set(u => u.C_Url, update.Url)
                    .Set(u => u.C_Text, update.Text)
                    .Set(u => u.D_Date, update.Date)
                    .Set(u => u.F_Type, update.Type)
                    .Set(u => u.F_Section, update.Section)
                    .Set(u => u.B_Target, update.Target)
                    .Set(u => u.B_Disabled, update.Disabled)                  


                    .Update();
                    return true;
                }
                else return false;
            }

        }
        public override bool delBanners(Guid id, string domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Bannerss;
                if (data.Where(w => w.id == id && w.F_Site == domain) != null)
                {
                    data
                        .Where(w => w.id == id && w.F_Site == domain)
                        .Delete();
                    return true;
                }
                else return false;
            }

        }

        /// <summary>
        /// Модули сайтов
        /// </summary>
        /// <returns></returns>
        public override SiteModulesModel[] getSiteModules()
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    var data = db.main_SiteModuless
                    .OrderBy(o => o.C_Name)
                    .Select(s => new SiteModulesModel
                    {
                        Id = s.Id,
                        Name = s.C_Name,
                        Alias = s.C_Alias
                    });
                    return data.ToArray();
                }
                catch { return null; }
            }
        }
        public override SiteModulesModel getSiteModule(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {
                    var data = db.main_SiteModuless
                        .Where(w => w.Id == id)
                        .Select(s => new SiteModulesModel
                        {
                            Id = s.Id,
                            Name = s.C_Name,
                            Alias = s.C_Alias,
                            Full = s.B_Full,
                            Half = s.B_Half,
                            Third = s.B_Third,
                            TwoThird = s.B_TwoThird,
                            Fourth = s.B_Fourth,
                            ThreeFourth = s.B_ThreeFourth,
                        })
                        .First();
                    return data;
                }
                catch { return null; }
            }
        }
        public override bool createSiteModule(Guid id, SiteModulesModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                try
                {                    
                    db.main_SiteModuless
                    .Value(v => v.Id, insert.Id)
                    .Value(v => v.C_Name, insert.Name)
                    .Value(v => v.C_Alias, insert.Alias)
                    .Value(v => v.B_Full, insert.Full)
                    .Value(v => v.B_Half, insert.Half)
                    .Value(v => v.B_Third, insert.Third)
                    .Value(v => v.B_TwoThird, insert.TwoThird)
                    .Value(v => v.B_Fourth, insert.Fourth)
                    .Value(v => v.B_ThreeFourth, insert.ThreeFourth)
                    .Insert();
                    return true;
                }
                catch { return false; }
            }
        }
        public override bool updateSiteModule(Guid id, SiteModulesModel update)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SiteModuless.Where(w => w.Id == id);
                if (data != null)
                {
                    data
                    .Where(w => w.Id == id)
                    .Set(u => u.C_Name, update.Name)
                    .Set(u => u.C_Alias, update.Alias)
                    .Set(u => u.B_Full, update.Full)
                    .Set(u => u.B_Half, update.Half)
                    .Set(u => u.B_Third, update.Third)
                    .Set(u => u.B_TwoThird, update.TwoThird)
                    .Set(u => u.B_Fourth, update.Fourth)
                    .Set(u => u.B_ThreeFourth, update.ThreeFourth)
                    .Update();
                    return true;
                }
                else return false;
            }
        }
        public override bool deleteSiteModule(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SiteModuless;
                if (data.Where(w => w.Id == id) != null)
                {
                    data
                        .Where(w => w.Id == id)
                        .Delete();
                    return true;
                }
                else return false;
            }
        }
        #endregion


        // ----------------------- Для сайта -------------------------
        #region Для сайта

        public override pagePathModel[] getPagePath(string Url, string Domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SP_site_PagePath(Url, Domain)                    
                    .Select(s => new pagePathModel
                    {
                        Path = s.C_Path,
                        Alias = s.C_Alias,
                        Url = s.C_Url,
                        Title = s.C_Title
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override siteMapModel getPageInfo(string Path, string Alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_site_SiteMaps.
                    Where(w => w.C_Path == Path && (w.C_Alias == Alias || w.C_Alias == "*")).
                    Select(s => new siteMapModel
                    {
                        View = s.F_ViewUrl,
                        Path = s.C_Path,
                        Alias = s.C_Alias,
                        Menu=s.F_Menu,
                        Title = s.C_Title,
                        Logo = s.C_Logo,
                        File = s.C_File,
                        Url = s.C_Url,
                        Text = s.C_Text,
                        Desc = s.C_Desc,
                        Keyw = s.C_Keyw
                    });

                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override siteMapModel getPageInfo(string Path, string Alias, string Domain)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.SV_site_SiteMaps.
                    Where(w => w.C_Path == Path && (w.C_Alias == Alias) && (w.F_Site == Domain || w.F_Site == null)).
                    Select(s => new siteMapModel
                    {
                        View = s.F_ViewUrl,
                        Path = s.C_Path,
                        Alias = s.C_Alias,
                        Title = s.C_Title,
                        Logo = s.C_Logo,
                        File = s.C_File,
                        Url = s.C_Url,
                        Text = s.C_Text,
                        Desc = s.C_Desc,
                        Keyw = s.C_Keyw
                    });

                if (!data.Any()) {
                    return null; }
                else {
                    return data.First(); }
            }
        }

        public override siteMapModel[] getPageMenu(string Site)
        {
            using (var db=new CMSdb(_context))
            {
                var data = db.main_SiteMaps.
                Where(w => w.F_Site == Site).
                Select(s => new siteMapModel {
                    Alias = s.C_Alias,
                    Permit=s.N_Permit,
                    Title = s.C_Title,
                    Path = s.C_Path,
                    Menu = s.F_Menu,
                    Url = s.C_Url,
                    Disabled=s.B_Disabled
                }).OrderByDescending(o => o.Permit); ;
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override siteMapModel[] getPageChildElem(string Path,string Site)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_SiteMaps.
                Where(w => (w.F_Site == Site) && (w.C_Path.Contains(Path))).
                Select(s => new siteMapModel
                {
                    Alias = s.C_Alias,
                    Permit = s.N_Permit,
                    Title = s.C_Title,
                    Path = s.C_Path,
                    Menu = s.F_Menu,
                    Url = s.C_Url,
                    Disabled = s.B_Disabled
                }).OrderByDescending(o => o.Permit); ;
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override SitesModel[] getAllSitesInfo()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.main_Sitess.
                Select(s => new SitesModel
                {
                    Id = s.Id,
                    C_Name = s.C_Name,
                    C_Domain = s.C_Domain,
                    C_Adress = s.C_Adress,
                    C_Phone = s.C_Phone,
                    C_Fax = s.C_Fax,
                    C_Email = s.C_Email,
                    C_Site = s.C_Site,
                    C_Worktime = s.C_Worktime,
                    C_Logo = s.C_Logo,
                    C_Scripts = s.C_Scripts,
                    C_AdmEmail = s.C_AdmEmail,
                    C_AdmPhone = s.C_AdmPhone
                });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        #endregion
    }
}
