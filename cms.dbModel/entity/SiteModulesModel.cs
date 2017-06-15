using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class SiteModulesModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Name { get; set; }        
        [RegularExpression(@"^([a-zA-Z0-9-_]+)$", ErrorMessage = "Поле «Название на латинском» может содержать буквы латинского алфавита, цифры и символы: - (дефис) и _ (нижнее подчеркивание).")]
        public string Alias { get; set; }
        public bool Full { get; set; }
        public bool Half { get; set; }
        public bool Third { get; set; }
        public bool TwoThird { get; set; }
        public bool Fourth { get; set; }
        public bool ThreeFourth { get; set; }
    }
}