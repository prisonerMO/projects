using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AOK.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Käyttäjätunnus on pakollinen")]
        [MinLength(3, ErrorMessage = "Käyttäjätunnuksen on oltava vähintään 3 merkkiä pitkä")]
        [MaxLength(40, ErrorMessage = "Käyttäjätunnus ei voi olla yli 40 merkkiä pitkä")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Käyttäjätunnus ei voi sisältää välilyöntejä")]
        [DisplayName("Käyttäjätunnus")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Syötä kelvollinen sähköpostiosoite")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Sähköposti")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Salasana on pakollinen")]
        [MinLength(6, ErrorMessage = "Salasanan on oltava vähintään 6 merkkiä pitkä")]
        [DataType(DataType.Password)]
        [DisplayName("Salasana")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Salasana uudestaan on pakollinen")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Salasanat eivät täsmää")]
        [DisplayName("Salasana uudestaan")]
        public string ConfirmPassword { get; set; }

        public bool Register { get; set; }
    }
}
