using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class UsersList 
    {
        public UsersModel[] Data;
        public Pager Pager;
    }

    public class UsersModel
    {
        [Required]
        public Guid Id { get; set; }
        public string EMail { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        [Required(ErrorMessage = "Поле «Группа» не должно быть пустым.")]
        public string Group { get; set; }
        public string GroupName { get; set; }
        public string Post { get; set; }
        public string Desc { get; set; }
        public string Keyw { get; set; }
        public string FIO { get; set; }
        [Required(ErrorMessage = "Поле «Фамилия» не должно быть пустым.")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Поле «Имя» не должно быть пустым.")]
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Sex { get; set; }
        public string Photo { get; set; }
        public string Adres { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Contacts { get; set; }
        [Required]
        public bool Disabled { get; set; }
        [Required]
        public bool Deleted { get; set; }

        public string FullName { get { return Surname + " " + Name; } }
    }
    
    public class PasswordModel
    {
        [Required(ErrorMessage = "Поле Пароль» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Пароль имеет не правильный формат")]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        [Required(ErrorMessage = "Поле «Подтверждение пароля» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Подтверждение пароля имеет не правильный формат")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public virtual string PasswordConfirm { get; set; }
    }

    public class ResolutionsModel
    {
        public Guid MenuId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Чтение")]
        public bool Read { get; set; }
        [Display(Name = "Запись")]
        public bool Write { get; set; }
        [Display(Name = "Изменение")]
        public bool Change { get; set; }
        [Display(Name = "Удаление")]
        public bool Delete { get; set; }
        public bool Importent { get; set; }
    }

    public class UsersGroupModel
    {
        public Guid id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9-_\.]{1,20}$", ErrorMessage = "Название группы должно быть написано латинскими буквами")]
        [Display(Name = "Название группы")]
        public string Alias { get; set; }
        [Required]
        [Display(Name = "Отображаемое имя")]
        public string GroupName { get; set; }
        public ResolutionsModel[] GroupResolutions { get; set; }
    }

    //public class ResolutionsModel
    //{
    //    public Guid Id { get; set; }
    //    public Guid MenuId { get; set; }
    //    public string MenuTitle { get; set; }
    //    public int Permit { get; set; }
    //    public string Section { get; set; }
    //    public Guid UserId { get; set; }
    //    public bool Read { get; set; }
    //    public bool Write { get; set; }
    //    public bool Change { get; set; }
    //    public bool Delete { get; set; }
    //    public bool Importent { get; set; }
    //}

    //public class ResolutionsTemplatesModel
    //{
    //    public Guid Id { get; set; }
    //    public Guid MenuId { get; set; }
    //    public string MenuTitle { get; set; }
    //    public int Permit { get; set; }
    //    public string Section { get; set; }
    //    public string UserGroupId { get; set; }
    //    public bool Read { get; set; }
    //    public bool Write { get; set; }
    //    public bool Change { get; set; }
    //    public bool Delete { get; set; }
    //}
    
    //public class UserSiteLink
    //{
    //    public string SiteId { get; set; }
    //    public Guid UserId { get; set; }
    //}
}
