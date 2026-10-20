using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AOK.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Nykyinen salasana on pakollinen")]
        [DataType(DataType.Password)]
        [DisplayName("Nykyinen salasana")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Uusi salasana on pakollinen")]
        [DataType(DataType.Password)]
        [DisplayName("Uusi salasana")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vahvista uusi salasana")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Salasanat eivät täsmää")]
        [DisplayName("Vahvista uusi salasana")]
        public string ConfirmPassword { get; set; }
    }
}
