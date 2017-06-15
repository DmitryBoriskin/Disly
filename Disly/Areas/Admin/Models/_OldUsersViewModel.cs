using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class UsersViewModel : CoreViewModel
    {
        public UsersModel[] getUsersList { get; set; }
        public UsersModel User { get; set; }
        public UsersGroupModel[] Group { get; set; }
        public UsersGroupModel GroupItem { get; set; }
        public UserSex[] Sex { get; set; }        
        public UsersChangePass ChangePass { get; set; }

        //модели для прав
        public ResolutionsModel[] ResolutionsList { get; set; }
        public ResolutionsTemplatesModel[] ResolutionsTemplatesList { get; set; }


        //[Required(ErrorMessage = "Поле «Пароль» не должно быть пустым.")]
        public string pass { get; set; }
        public string ErrorInfo { get; set; }
        public string searchSurname { get; set; }
        public string searchName { get; set; }
        public string searchEmail { get; set; }

        //[Required(ErrorMessage = "Поле «Новый пароль» не должно быть пустым.")]
        //[StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        //[RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Пароль имеет не правильный формат")]
        //[DataType(DataType.Password)]
        //public virtual string Password { get; set; }

        //[Required(ErrorMessage = "Поле «Подтверждение пароля» не должно быть пустым.")]
        //[StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        //[RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Подтверждение пароля имеет не правильный формат")]
        //[Compare("Password", ErrorMessage = "Пароли не совпадают")]
        //[DataType(DataType.Password)]
        //public virtual string PasswordConfirm { get; set; }
    }
    
    public class UserSex
    {
        public bool? Value { get; set; }
        public string Label { get; set; }
    }
}
